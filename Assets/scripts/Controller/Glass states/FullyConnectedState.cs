using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class FullyConnectedState : GlassControllerState
		{
			public FullyConnectedState(ref ConcreteGlassController controller)
				: base(ref controller)
			{
				firstDisconnectionMsg = true;
			}

			public override void OnEnter()
			{
                Debug.Log("entering FullyConnectedState");
				m_controller.m_cxnManager.StopListeningNewConnections(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);

				m_controller.CloseAllNonValidConnections(m_controller.m_serverInfo);

				m_controller.SendCommand(m_controller.m_watchConnectionInfo, new ConnectedStatus(-1));

				// Sends the current step path
				ScenarioState state = m_controller.m_callbacks.GetScenarioState();
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, new WatchStepPathChangedCmd(state.StepPath));
                m_controller.SendCommand(m_controller.m_padConnectionInfo, new WatchStepPathChangedCmd(state.StepPath));
                Debug.Log("Fully Connected State : " + state.StepPath); 
			}

			public override void OnEmissionError(int clientId, int errorCode)
			{
                /* --->>> TODO in the demo support distant context the "pad" is replaced by the "expert" to which the video is streamed
				ControllerState newState = null;
				if (clientId == m_controller.m_watchConnectionInfo.localToRemoteId)
				{
					// The pad can't work without the watch (no command is transmitted/received)
					// So when the watch is disconnected we disconnect the pad too.
					// We send a DisconnectionCmd and the acknowledge will be handle in the 
					// PadConnectedState state.
					m_controller.SendCommand(m_controller.m_padConnectionInfo, new DisconnectionCmd(-1));

					newState = new PadConnectedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
				else if (clientId == m_controller.m_padConnectionInfo.localToRemoteId)
				{
					newState = new WatchConnectedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
                 */
			}

			#region IMessageVisitor implementation
			public override void HandleMessage(ReceptionError msg)
			{
                // --->>> TODO in the demo support distant context the "pad" is replaced by the "expert" to which the video is streamed.
                //if (msg.ClientId == m_controller.m_watchConnectionInfo.remoteToLocalId)
                //{
                //    // The pad can't work without the watch (no command is transmitted/received)
                //    // So when the watch is disconnected we disconnect the pad too.
                //    // We send a DisconnectionCmd and the acknowledge will be handle in the 
                //    // PadConnectedState state.
                //    m_controller.SendCommand(m_controller.m_padConnectionInfo, new DisconnectionCmd(-1));

                //    ControllerState newState = new PadConnectedState(ref m_controller);
                //    m_controller.ChangeState(ref newState);
                //}
                //else if (msg.ClientId == m_controller.m_padConnectionInfo.remoteToLocalId)
                //{
                //    ControllerState newState = new WatchConnectedState(ref m_controller);
                //    m_controller.ChangeState(ref newState);
                //}
			}
            
            public override void HandleMessage(DownloadProcedureFromURLCmd cmd)
            {
                // the expert asked the glass to download a procedure from the server

                if (cmd == null)
                {
                    Debug.Log("/////////////// Command NUll");
                }
                else
                {
                    Debug.Log("/////////////// Command  : " + cmd.m_name + "  " +  cmd.m_url);
                    // GUI callback
                    m_controller.m_callbacks.CallOnDownloadProcedureFromURL(cmd.m_name, cmd.m_url);
                }


                
            }

            public override void HandleMessage(SendAnnotation msg)
			{
				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_expertConnectionInfo, msg.Cmd);
			}

            #region glass application gui events
			public override void HandleMessage(SendCurrentStep msg)
			{
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, msg.Cmd);
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

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(NextStepCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnNextStep();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(PreviousStepCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnPreviousStep();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(BookmarksCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnLoadBookmark(cmd.BookmarkIdx);

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(CurrentViewNextCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnCurrentViewNext();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(CurrentViewPreviousCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnCurrentViewPrevious();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchLocalizationCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchLocalization();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchReferencesCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchReferences();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchToolsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchTools();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchCommentsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchComments();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchAnnotationsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchAnnotation();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchGUICmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchGUI();

				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);

				if (m_controller.m_callbacks.IsGUIShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(WatchScreenChangedCmd cmd)
			{
				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}
            #endregion glass application gui events

            public override void HandleMessage(TakeScreenshotCmd cmd)
			{
				// "GUI" callback. It will call OnAnnotationRequest.
				m_controller.m_callbacks.CallOnTakeScreenshot();
			}

			public override void HandleMessage(DiagnosticCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnOpenReceivedAnnotation(cmd.Accept);
				if (cmd.Accept)
				{
					// sends back to watch
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchAnnotationsCmd());
				}

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(DisconnectionCmd cmd)
			{
				if (firstDisconnectionMsg)
				{
					firstDisconnectionMsg = false;
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
                    m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
				}

				if (m_controller.m_watchConnectionInfo.localToRemoteId == cmd.ConnectionId)
				{
					m_controller.CloseWatchConnection();
				}

                if (m_controller.m_expertConnectionInfo.localToRemoteId == cmd.ConnectionId)
				{
					m_controller.CloseExpertConnection();
				}

                if (m_controller.m_expertConnectionInfo.localToRemoteId == -1 && m_controller.m_watchConnectionInfo.localToRemoteId == -1)
				{
					Application.Quit();
				}
			}

            public override void HandleMessage(DisconnectionToSupportCmd cmd)
            {
                // --->>> TODO
                // forward the message to the expert application
                // trigger a disconnect on its connection
                // switch to the waiting pad connection state
            }

			public override void HandleMessage(AskReSynchronizationCmd cmd)
			{
				ScenarioState guiState = m_controller.m_callbacks.GetScenarioState();
				ReSynchronizationCmd reSyncCmd = new ReSynchronizationCmd(guiState);

				m_controller.SendCommand(m_controller.m_watchConnectionInfo, reSyncCmd);
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, reSyncCmd);
			}

			public override void HandleMessage(AnnotationCmd cmd)
			{

                Debug.Log("//////// fully connected State :" + cmd.StepPath); 
				// "GUI" callback.
				m_controller.m_callbacks.CallOnAnnotationReceived(cmd.StepPath, cmd.ImageContent);

				// Sends diagnostic notification to the watch.
				// Here the flags "Accept" of the diagnostic command is not used, it will be set by the watch.
				// Sends false by default.
				DiagnosticCmd diagCmd = new DiagnosticCmd(cmd.StepPath, false);
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, diagCmd);
			}

			public override void HandleMessage(CaptureCmd cmd)
			{
				// Sends the command to the pad.
                m_controller.SendCommand(m_controller.m_expertConnectionInfo, cmd);
			}
			#endregion IMessageVisitor implementation

			/// <summary>
			/// 
			/// </summary>
			private bool firstDisconnectionMsg;
		}
	}
}
