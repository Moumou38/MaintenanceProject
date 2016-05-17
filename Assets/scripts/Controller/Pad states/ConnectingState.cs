using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcretePadController
	{
		protected class ConnectingState : PadControllerState
		{
			public ConnectingState(ref ConcretePadController controller)
				: base(ref controller)
			{
			}

			public override void OnEmissionError(int clientId, int errorCode)
			{
				ControllerState newState = new DegradedState(ref m_controller);
				m_controller.ChangeState(ref newState);
			}

			public override void HandleMessage(ConnectTo msg)
			{
                BTClientParameters parameters = new BTClientParameters(10, msg.DeviceAddress, msg.DeviceName, "9C6ABA4A-642D-47BD-BDCA-9E0A4123522A", 0);
				int ret = m_controller.m_cxnManager.StartClient(parameters);

				if (ret < 0)
				{
					Debug.LogError("The connection to " + msg.DeviceName + "(" + msg.DeviceAddress + ") failed");
					m_controller.m_padCallbacks.CallOnOnOnConnectionResult(false);
				}
				else
				{
                    m_controller.m_glassConnectionInfo.localToRemoteId = ret;
					Debug.Log("Connected with " + msg.DeviceName + " (" + msg.DeviceAddress + ")");

                    m_controller.SendCommand(m_controller.m_glassConnectionInfo, new ConnectionCmd(m_controller.m_btRadioDeviceName, m_controller.m_btRadioDeviceAddr, false, -1));

					m_controller.m_connectToRqt = msg;

					ControllerState newState = new WaitingGlassConnectionState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}
		}
	}
}