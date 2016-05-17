using UnityEngine;
using System.IO;
using System;

namespace dassault
{
    public partial class ConcreteGlassController
    {
        protected class WaitingPadConnectionState : GlassControllerState
        {
            public WaitingPadConnectionState(ref ConcreteGlassController controller)
                : base(ref controller)
            {
            }

            public override void Update()
            {
                int idMin = 0, idMax = 0;

                // watch for bluetooth connections
                if (m_controller.m_cxnManager.GetNewConnectionsId(m_controller.m_serverInfo.id, ref idMin, ref idMax, m_controller.m_serverInfo.cxnType) == 2)
                {
                    for (int i = idMin; i <= idMax; ++i)
                    {
                        m_controller.StartDataReception(m_controller.m_serverInfo, i);
                    }
                }

                // watch for tcp connections
                if (m_controller.m_cxnManager.GetNewConnectionsId(m_controller.m_tcpServerInfo.id, ref idMin, ref idMax, m_controller.m_tcpServerInfo.cxnType) == 2)
                {
                    for (int i = idMin; i <= idMax; ++i)
                    {
                        Debug.Log("starting data reception on TCP server");
                        m_controller.StartDataReception(m_controller.m_tcpServerInfo, i);
                    }
                }
            }

            public override void OnEmissionError(int clientId, int errorCode)
            {
                /* --->>> TODO
                ControllerState newState = null;
                if (clientId == m_controller.m_padConnectionInfo.localToRemoteId ||
                    clientId == m_controller.m_watchConnectionInfo.localToRemoteId)
                {
                    newState = new DegradedState(ref m_controller);
                    m_controller.ChangeState(ref newState);
                }
                */
            }

            public override void HandleMessage(ReceptionError msg)
            {
                if (msg.ClientId == m_controller.m_watchConnectionInfo.remoteToLocalId ||
                    msg.ClientId == m_controller.m_padConnectionInfo.remoteToLocalId)
                {
                    ControllerState newState = new DegradedState(ref m_controller);
                    m_controller.ChangeState(ref newState);
                }
            }

            // management of "expert support connection" request from the watch 
            public override void HandleMessage(ConnectionToSupportCmd cmd)
            {
                switch (cmd.m_type)
                {
                    case ConnectionType.BLUETOOTH:
                        {
                            // --->>> TODO
                            break;
                        }
                    case ConnectionType.TCPIP:
                        {
                            // start a TCP IP client with the expert support workstation:
                            // we expect a TCP address of the form: "xxx.xxx.xxx.xxx:#port" in order for the IP port to be configurable for the glass
                            string[] AddressAndPort = cmd.m_address.Split(new System.Char[] { ':' });
                            int port = Int32.Parse(AddressAndPort[1]);
                            TCPClientParameters supportTCPParams = new TCPClientParameters(AddressAndPort[0], "", port);
                            int ret = m_controller.m_cxnManager.StartClient(supportTCPParams);

                            if (ret < 0)
                            {
                                Debug.LogError("The client connection to " + cmd.m_expertNickname + "(" + cmd.m_address + ") failed");
                                m_controller.m_callbacks.CallSetPadConnexionStatus(false);

                                // notify the watch of the failure
                                SupportStatusCmd supportStatusCmd = new SupportStatusCmd(false);
                                m_controller.SendCommand(m_controller.m_watchConnectionInfo, supportStatusCmd);
                            }
                            else
                            {
                                m_controller.m_expertConnectionInfo.localToRemoteId = ret;

                                Debug.Log("Connected with " + cmd.m_expertNickname + " (" + cmd.m_expertNickname + ")");

                                switch (m_controller.m_expertConnectionInfo.type)
                                {
                                    case ConnectionType.BLUETOOTH:
                                        m_controller.SendCommand(m_controller.m_expertConnectionInfo, new ConnectionCmd(m_controller.m_btRadioDeviceName, m_controller.m_btRadioDeviceAddr, true, -1));
                                        break;
                                    case ConnectionType.TCPIP:
                                        m_controller.SendCommand(m_controller.m_expertConnectionInfo, new ConnectionCmd(m_controller.m_tcpDeviceName, m_controller.m_tcpDeviceAddr, true, -1));
                                        break;
                                }


                                // waiting for the confirmation of the connection from the expert workstation
                                m_waitingForExpertConnectedStatus = true;
                                m_waitingClientId = ret;
                            }
                            break;
                        }
                }
            }

            public override void HandleMessage(ConnectedStatus cmd)
            {
                // connection success -> go to fully connected state  
                Debug.Log("received ConnectedStatus message ... ");
                if (!m_waitingForExpertConnectedStatus)
                { 
                    Debug.LogError("received ConnectedStatus message ... we are not waiting for a connection confirmation ?");
                }
                else
                {
                    Debug.Log("connection to the support succeeded, going to fully connected state, and notify the watch of the successfull connection");

                    // notify the watch of the successfull connection
                    SupportStatusCmd supportStatusCmd = new SupportStatusCmd(true);
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, supportStatusCmd);

                    // ask the stream server to start and wait for the expert application to answer i order to go to "fully connected" state
                    string url;
                    int port;
                    if (m_controller.m_callbacks.CallStartVideoStreamingServer(out url, out port ) == true)
                    {
                        // success, send the stream parameters to the client, in order for it to connect back on the stream server
                        StreamParametersCmd streamParameters = new StreamParametersCmd(url, port);
                        m_controller.SendCommand(m_controller.m_expertConnectionInfo, streamParameters);
                    }
                    else
                    {
                        Debug.LogError("WaitingPadConnectionState: error while tryin to startup the video streaming server");
                        ControllerState newState = new DegradedState(ref m_controller);
                        m_controller.ChangeState(ref newState);
                    }

                }
            }

            public override void HandleMessage(ConnectedToStreamCmd cmd)
            {
                if (cmd.m_connected == true)
                {
                    // the expert successfully connected to the video streaming flow, we are fully connected (yeah!)
                    Debug.Log("expert connected to the streaming video!");

                    ControllerState newState = new FullyConnectedState(ref m_controller);
                    m_controller.ChangeState(ref newState);
                }
                else
                {
                    // connection failed, going to Degraded state
                    Debug.LogError("WaitingPadConnectionState: error on the expert side while trying to connect to the video stream flow");
                    ControllerState newState = new DegradedState(ref m_controller);
                    m_controller.ChangeState(ref newState);
                }
            }

            public override void HandleMessage(ConnectionRefusedStatus sts)
            {
                // connection to expert refused by expert.
                Debug.Log("connection to expert refused by expert");

                // notify the watch
                SupportStatusCmd supportStatusCmd = new SupportStatusCmd(false);
                m_controller.SendCommand(m_controller.m_watchConnectionInfo, supportStatusCmd);

                //disconnect the server
                m_controller.m_cxnManager.CloseServerConnection(sts.ConnectionId, m_waitingClientId, ConnectionType.TCPIP);

                m_waitingForExpertConnectedStatus = false;
                m_waitingClientId = -1;
            }
            
            public override void HandleMessage(ConnectionCmd cmd)
            {
                DeviceType type = m_controller.GetDeviceType(cmd.DeviceName);

                switch (type)
                {
                    case DeviceType.Pad:
                        {
                            BTClientParameters parameters = new BTClientParameters(10, cmd.DeviceAddr, cmd.DeviceName, "9C6ABA4A-642D-47BD-BDCA-9E0A4123522A", 0);
                            int ret = m_controller.m_cxnManager.StartClient(parameters);
                            if (ret < 0)
                            {
                                Debug.LogError("The connection to " + cmd.DeviceName + "(" + cmd.DeviceAddr + ") failed");

                                ControllerState newState = new DegradedState(ref m_controller);
                                m_controller.ChangeState(ref newState);
                            }
                            else
                            {
                                m_controller.m_padConnectionInfo.localToRemoteId = ret;
                                m_controller.m_padConnectionInfo.remoteToLocalId = cmd.ConnectionId;

                                Debug.Log("Connected with " + cmd.DeviceName + " (" + cmd.DeviceAddr + ")");

                                m_controller.SendCommand(m_controller.m_padConnectionInfo, new ConnectionCmd(m_controller.m_btRadioDeviceName, m_controller.m_btRadioDeviceAddr, false, -1));

                                //m_controller.m_callbacks.CallSetPadConnexionStatus(true);

                                //ControllerState newState = new FullyConnectedState(ref m_controller);
                                //m_controller.ChangeState(ref newState);
                            }
                            break;
                        }

                    default:
                        {
                            // only pad connection is allowed. Others are closed.
                            if (m_controller.m_cxnManager.CloseServerConnection(m_controller.m_serverInfo.id, cmd.ConnectionId, m_controller.m_serverInfo.cxnType) != 0)
                            {
                                Debug.LogError("Error while closing the undesired connection");
                            }
                            break;
                        }
                }
            }

            public override void OnEnter()
            {
                m_waitingForExpertConnectedStatus = false;
                m_waitingClientId = -1;

                // Sends the current step path
                ScenarioState state = m_controller.m_callbacks.GetScenarioState();
                m_controller.SendCommand(m_controller.m_watchConnectionInfo, new WatchStepPathChangedCmd(state.StepPath));

                Debug.Log("Waiting Connection State : " + state.StepPath); 
            }


            /// <summary> 
            /// flag indicating that we are waiting for the connection confirmation message (used when the glass connects to the expert support workstation
            /// </summary>
            bool m_waitingForExpertConnectedStatus = false;

            /// <summary>
            /// id of the client that is waiting for a connection answer
            /// </summary>
            int m_waitingClientId = -1;

            #region glass application gui events
            // those commands have been added to this state because the maintenance worker should be able to work, even if no expert is connected


            public override void HandleMessage(SendCurrentStep msg)
            {
                m_controller.SendCommand(m_controller.m_watchConnectionInfo, msg.Cmd);
            }

            public override void HandleMessage(CommentVisibility msg)
            {
                if (msg.Visible)
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchCommentsCmd());
                }
            }

            public override void HandleMessage(ToolVisibility msg)
            {
                if (msg.Visible)
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchToolsCmd());
                }
            }

            public override void HandleMessage(ReferenceVisibility msg)
            {
                if (msg.Visible)
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchReferencesCmd());
                }
            }

            public override void HandleMessage(AnnotationVisibility msg)
            {
                if (msg.Visible)
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchAnnotationsCmd());
                }
            }

            public override void HandleMessage(LocalizationVisibility msg)
            {
                if (msg.Visible)
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchLocalizationCmd());
                }
            }

            public override void HandleMessage(ScenarioCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnLoadProject(cmd.ProjectName);
           }

            public override void HandleMessage(NextStepCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnNextStep();
                            }

            public override void HandleMessage(PreviousStepCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnPreviousStep();
            }

            public override void HandleMessage(BookmarksCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnLoadBookmark(cmd.BookmarkIdx);
            }

            public override void HandleMessage(CurrentViewNextCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnCurrentViewNext();
            }

            public override void HandleMessage(CurrentViewPreviousCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnCurrentViewPrevious();
            }

            public override void HandleMessage(SwitchLocalizationCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchLocalization();
            }

            public override void HandleMessage(SwitchReferencesCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchReferences();
            }

            public override void HandleMessage(SwitchToolsCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchTools();
            }

            public override void HandleMessage(SwitchCommentsCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchComments();
            }

            public override void HandleMessage(SwitchAnnotationsCmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchAnnotation();
            }

            public override void HandleMessage(SwitchGUICmd cmd)
            {
                // GUI callback
                m_controller.m_callbacks.CallOnSwitchGUI();

                if (m_controller.m_callbacks.IsGUIShown())
                {
                    m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
                }
            }

            public override void HandleMessage(WatchScreenChangedCmd cmd)
            {
            }


            #endregion glass application gui events

        }
    }
}