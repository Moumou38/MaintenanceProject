using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class WaitingWatchConnectionState : GlassControllerState
		{
			public WaitingWatchConnectionState(ref ConcreteGlassController controller)
				: base(ref controller)
			{
			}

			public override void Update()
			{
				int idMin = 0, idMax = 0;

				if (m_controller.m_cxnManager.GetNewConnectionsId(m_controller.m_serverInfo.id, ref idMin, ref idMax, m_controller.m_serverInfo.cxnType) == 2)
				{
					for (int i = idMin; i <= idMax; ++i)
					{
						m_controller.StartDataReception(m_controller.m_serverInfo, i);
					}
				}
			}

			public override void OnEmissionError(int clientId, int errorCode)
			{
				ControllerState newState = null;
				if (clientId == m_controller.m_padConnectionInfo.localToRemoteId)
				{
					newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}

			public override void HandleMessage(ReceptionError msg)
			{
                if (msg.ClientId == m_controller.m_watchConnectionInfo.remoteToLocalId)
				{
					ControllerState newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}

			public override void HandleMessage(ConnectionCmd cmd)
			{
				DeviceType type = m_controller.GetDeviceType(cmd.DeviceName);

				switch (type)
				{
					case DeviceType.Watch:
						{
							m_controller.m_watchConnectionInfo.remoteToLocalId = cmd.ConnectionId;

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
                                m_controller.m_watchConnectionInfo.localToRemoteId = ret;

								Debug.Log("Connected with " + cmd.DeviceName + " (" + cmd.DeviceAddr + ")");
								Debug.Log("Should connect with tab: " + cmd.ConnectWithTab.ToString());

								m_controller.m_callbacks.CallSetWatchConnexionStatus(true);

                                //if (cmd.ConnectWithTab)
                                //{
									ControllerState newState = new WaitingPadConnectionState(ref m_controller);
									m_controller.ChangeState(ref newState);
                                //}
                                //else
                                //{
                                //    ControllerState newState = new WatchConnectedState(ref m_controller);
                                //    m_controller.ChangeState(ref newState);
                                //}
							}
							break;
						}

					case DeviceType.Pad:
						{
							// pad connections are not yet accepted
							//try to connect to the tab to send the ConnectionRefusedStatus
                            BTClientParameters parameters = new BTClientParameters(10, cmd.DeviceAddr, cmd.DeviceName, "9C6ABA4A-642D-47BD-BDCA-9E0A4123522A", 0);
							int ret = m_controller.m_cxnManager.StartClient(parameters);
							if (ret >= 0)
							{
                                ConnectionInfo cxnInfo = new ConnectionInfo(parameters.cxnType);
                                cxnInfo.localToRemoteId = ret;
								m_controller.SendCommand(cxnInfo, new ConnectionRefusedStatus(-1));
								m_controller.m_cxnManager.StopClient(ret, parameters.cxnType);
							}

							// Closes the connection anyway
                            if (m_controller.m_cxnManager.CloseServerConnection(m_controller.m_serverInfo.id, cmd.ConnectionId, m_controller.m_serverInfo.cxnType) != 0)
							{
								Debug.LogError("Error while closing the undesired connection");
							}
							break;
						}

					default:
						{
							// only watch connection is allowed. Others are closed.
							if (m_controller.m_cxnManager.CloseServerConnection(m_controller.m_serverInfo.id, cmd.ConnectionId, m_controller.m_serverInfo.cxnType) != 0)
							{
								Debug.LogError("Error while closing the undesired connection");
							}
							break;
						}
				}
			}
		}
	}
}
