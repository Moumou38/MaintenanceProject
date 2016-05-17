namespace dassault
{
	public partial class ConcretePadController
	{
		protected class PadControllerState : ControllerState
		{
			public PadControllerState(ref ConcretePadController controller)
			{
				m_controller = controller;
			}

			protected ConcretePadController m_controller;
		}
	}
}
