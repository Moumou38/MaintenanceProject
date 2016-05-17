using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcretePadController
	{
		protected class WaitingGlassConnectionState : PadControllerState
		{
			public WaitingGlassConnectionState(ref ConcretePadController controller)
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

			public override void HandleMessage(ConnectionCmd cmd)
			{
				DeviceType type = m_controller.GetDeviceType(cmd.DeviceName);

				type = DeviceType.Glasses;

				switch (type)
				{
					case DeviceType.Glasses:
						{
							if (string.Compare(cmd.DeviceAddr, m_controller.m_connectToRqt.DeviceAddress, true) != 0)
							{
								Debug.LogError("These glasses are not the expected ones");

								// there are not the expected glasses. We close the connection
                                if (m_controller.m_cxnManager.CloseServerConnection(m_controller.m_serverInfo.id, cmd.ConnectionId, m_controller.m_serverInfo.cxnType) != 0)
								{
									Debug.LogError("Error while closing the undesired connection between server " + m_controller.m_serverInfo.id + " and client " + cmd.ConnectionId);
								}
							}
							else
							{
								m_controller.m_glassConnectionInfo.remoteToLocalId = cmd.ConnectionId;
								Debug.Log("Connected with " + cmd.DeviceName + " (" + cmd.DeviceAddr + ")");

								m_controller.m_padCallbacks.CallOnOnOnConnectionResult(true);

								ControllerState newState = new ConnectedState(ref m_controller);
								m_controller.ChangeState(ref newState);
							}

							break;
						}

					default:
						{
							// only glasses connections are allowed. We close the connection.
                            if (m_controller.m_cxnManager.CloseServerConnection(m_controller.m_serverInfo.id, cmd.ConnectionId, m_controller.m_serverInfo.cxnType) != 0)
							{
								Debug.LogError("Error while closing the undesired connection");
							}

							break;
						}
				}
			}

			public override void HandleMessage(ConnectionRefusedStatus sts)
			{
				m_controller.m_glassConnectionInfo.localToRemoteId = sts.ConnectionId;
				m_controller.CloseGlassesConnection();

				ControllerState newState = new ConnectingState(ref m_controller);
				m_controller.ChangeState(ref newState);

				m_controller.m_padCallbacks.CallOnOnOnConnectionResult(false);
			}
		}
	}
}