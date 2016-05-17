using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcretePadController
	{
		protected class DegradedState : PadControllerState
		{
			public DegradedState(ref ConcretePadController controller)
				: base(ref controller)
			{
			}

			public override void OnEnter()
			{
				m_controller.CloseServer();
				m_controller.CloseGlassesConnection();
			}

			public override void Update()
			{
				//Application.Quit();
			}
		}
	}
}