using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class DegradedState : GlassControllerState
		{
			public DegradedState(ref ConcreteGlassController controller)
				: base(ref controller)
			{
			}

			public override void OnEnter()
			{
				m_controller.m_cxnManager.StopListeningNewConnections(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);

				m_controller.CloseAllNonValidConnections(m_controller.m_serverInfo);

				m_controller.CloseWatchConnection();
				m_controller.ClosePadConnection();

                m_controller.m_cxnManager.StopServer(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);
				m_controller.m_serverInfo.id = -1;
			}

			#region IMessageVisitor implementation

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
			}

			public override void HandleMessage(AnnotationCmd cmd)
			{
				// "GUI" callback.
                Debug.Log("//////// Degraded State :" + cmd.StepPath); 
				m_controller.m_callbacks.CallOnAnnotationReceived(cmd.StepPath, cmd.ImageContent);
			}

			#endregion IMessageVisitor implementation
		}
	}
}