using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcretePadController
	{
		protected class ConnectedState : PadControllerState
		{
			public ConnectedState(ref ConcretePadController controller)
				: base(ref controller)
			{
			}

			public override void OnEnter()
			{
				m_controller.m_cxnManager.StopListeningNewConnections(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);

				m_controller.CloseAllNonValidConnections(m_controller.m_serverInfo);
			}

			public override void OnEmissionError(int clientId, int errorCode)
			{
				ControllerState newState = new DegradedState(ref m_controller);
				m_controller.ChangeState(ref newState);
			}

			#region IMessageVisitor implementation
			public override void HandleMessage(ReceptionError msg)
			{
				ControllerState newState = new DegradedState(ref m_controller);
				m_controller.ChangeState(ref newState);
			}

			public override void HandleMessage(SendAnnotationAck msg)
			{
				// Sends the command to the glasses.
				Debug.Log("Annotation Back to Glasses");

				m_controller.SendCommand(m_controller.m_glassConnectionInfo, msg.Cmd);

				// Simulates a reception of a AnnotationCmd message.
				m_controller.PushMessage(msg.Cmd);
			}

			public override void HandleMessage(ScenarioCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnLoadProject(cmd.ProjectName);
			}

			public override void HandleMessage(NextStepCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnNextStep();
			}

			public override void HandleMessage(PreviousStepCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnPreviousStep();
			}

			public override void HandleMessage(BookmarksCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnLoadBookmark(cmd.BookmarkIdx);
			}

			public override void HandleMessage(CurrentViewNextCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnCurrentViewNext();
			}

			public override void HandleMessage(CurrentViewPreviousCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnCurrentViewPrevious();
			}

			public override void HandleMessage(SwitchLocalizationCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchLocalization();
			}

			public override void HandleMessage(SwitchReferencesCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchReferences();
			}

			public override void HandleMessage(SwitchToolsCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchTools();
			}

			public override void HandleMessage(SwitchCommentsCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchComments();
			}

			public override void HandleMessage(SwitchAnnotationsCmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchAnnotation();
			}

			public override void HandleMessage(SwitchGUICmd cmd)
			{
				m_controller.m_glassCallbacks.CallOnSwitchGUI();
			}

			public override void HandleMessage(WatchScreenChangedCmd cmd)
			{
				m_controller.m_padCallbacks.CallOnWatchScreenChanged(cmd.ScreenIdx);
			}

			public override void HandleMessage(WatchStepPathChangedCmd cmd)
			{
				m_controller.m_padCallbacks.CallOnWatchStepPathChanged(cmd.StepPath);
			}

			public override void HandleMessage(DiagnosticCmd cmd)
			{
				Debug.Log("DiagnosticCmd result: " + cmd.Accept.ToString());
				m_controller.m_glassCallbacks.CallOnOpenReceivedAnnotation(cmd.Accept);
			}

			public override void HandleMessage(DisconnectionCmd cmd)
			{
				m_controller.SendCommand(m_controller.m_glassConnectionInfo, cmd);

				m_controller.CloseGlassesConnection();

				m_controller.CloseServer();

				// quit
				Application.Quit();
			}

			public override void HandleMessage(ReSynchronizationCmd cmd)
			{
				m_controller.m_glassCallbacks.CallSetScenarioState(cmd.State);
			}

			public override void HandleMessage(AnnotationCmd cmd)
			{
                Debug.Log("//////// connected State :" + cmd.StepPath); 
				m_controller.m_glassCallbacks.CallOnAnnotationReceived(cmd.StepPath, cmd.ImageContent);
			}

			public override void HandleMessage(CaptureCmd cmd)
			{
				m_controller.m_padCallbacks.CallOnCaptureReceived(cmd.StepPath, cmd.Image);
			}
			#endregion IMessageVisitor implementation
		}
	}
}