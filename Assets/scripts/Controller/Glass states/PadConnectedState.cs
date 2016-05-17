using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class PadConnectedState : GlassControllerState
		{
			public PadConnectedState(ref ConcreteGlassController controller)
				: base(ref controller)
			{
			}

			public override void OnEnter()
			{
				m_controller.m_cxnManager.StopListeningNewConnections(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);

				m_controller.CloseAllNonValidConnections(m_controller.m_serverInfo);

				m_controller.CloseWatchConnection();

                // Sends the current step path
                ScenarioState state = m_controller.m_callbacks.GetScenarioState();
                m_controller.SendCommand(m_controller.m_watchConnectionInfo, new WatchStepPathChangedCmd(state.StepPath));

                Debug.Log("Pad Connected State : " + state.StepPath); 
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

			#region IMessageVisitor implementation
			public override void HandleMessage(ReceptionError msg)
			{
				if (msg.ClientId == m_controller.m_padConnectionInfo.remoteToLocalId)
				{
					ControllerState newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}

			public override void HandleMessage(SendAnnotation msg)
			{
				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, msg.Cmd);
			}

			public override void HandleMessage(SendCurrentStep msg)
			{
				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, msg.Cmd);
			}

			public override void HandleMessage(NextStepCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnNextStep();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(PreviousStepCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnPreviousStep();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(BookmarksCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnLoadBookmark(cmd.BookmarkIdx);

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(CurrentViewNextCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnCurrentViewNext();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(CurrentViewPreviousCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnCurrentViewPrevious();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchLocalizationCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchLocalization();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchReferencesCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchReferences();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchToolsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchTools();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchCommentsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchComments();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchAnnotationsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchAnnotation();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(SwitchGUICmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchGUI();

				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(WatchScreenChangedCmd cmd)
			{
				// Sends the command to the pad.
				m_controller.SendCommand(m_controller.m_padConnectionInfo, cmd);
			}

			public override void HandleMessage(AnnotationCmd cmd)
			{
				// "GUI" callback.
                Debug.Log("//////// pad connected State :" + cmd.StepPath); 
				m_controller.m_callbacks.CallOnAnnotationReceived(cmd.StepPath, cmd.ImageContent);
			}

			public override void HandleMessage(DisconnectionCmd cmd)
			{
				if (m_controller.m_padConnectionInfo.remoteToLocalId == cmd.ConnectionId)
				{
					// The pad connection will be closed when entering in the degraded state.
					ControllerState newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}
			#endregion IMessageVisitor implementation
		}
	}
}
