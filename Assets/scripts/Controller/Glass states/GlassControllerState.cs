
namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class GlassControllerState : ControllerState
		{
			public GlassControllerState(ref ConcreteGlassController controller)
			{
				m_controller = controller;
			}

			protected ConcreteGlassController m_controller;
		}
	}
}
