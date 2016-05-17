using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcretePadController
	{
		protected class StartingState : PadControllerState
		{
			public StartingState(ref ConcretePadController controller)
				: base(ref controller)
			{ 
			}

			public override void Update()
			{
                BTServerParameters parameters = new BTServerParameters("PadServer", "9C6ABA4A-642D-47BD-BDCA-9E0A4123522A", -1);
				int ret = m_controller.m_cxnManager.StartServer(parameters);
				if (ret < 0)
				{
					Debug.LogError("Error while starting the server");

					m_controller.m_padCallbacks.CallOnOnServerCreated(false);
				}
				else
				{
					m_controller.m_serverInfo.id = ret;

					m_controller.m_padCallbacks.CallOnOnServerCreated(true);

					ControllerState newState = new ConnectingState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}
		}
	}
}