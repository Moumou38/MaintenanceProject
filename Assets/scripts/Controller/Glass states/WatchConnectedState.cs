using UnityEngine;
using System.IO;

namespace dassault
{
	public partial class ConcreteGlassController
	{
		protected class WatchConnectedState : GlassControllerState
		{
			public WatchConnectedState(ref ConcreteGlassController controller)
				: base(ref controller)
			{
			}

			public override void OnEnter()
			{
				m_controller.m_cxnManager.StopListeningNewConnections(m_controller.m_serverInfo.id, m_controller.m_serverInfo.cxnType);

				m_controller.CloseAllNonValidConnections(m_controller.m_serverInfo);

				m_controller.ClosePadConnection();

				m_controller.SendCommand(m_controller.m_watchConnectionInfo, new ConnectedStatus(-1));

				// Sends the current step path
				ScenarioState state = m_controller.m_callbacks.GetScenarioState();
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, new WatchStepPathChangedCmd(state.StepPath));
			}

			public override void OnEmissionError(int clientId, int errorCode)
			{
				ControllerState newState = null;
				if (clientId == m_controller.m_watchConnectionInfo.localToRemoteId)
				{
					newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}

			#region IMessageVisitor implementation
			public override void HandleMessage(ReceptionError msg)
			{
				if (msg.ClientId == m_controller.m_watchConnectionInfo.remoteToLocalId)
				{
					ControllerState newState = new DegradedState(ref m_controller);
					m_controller.ChangeState(ref newState);
				}
			}

			public override void HandleMessage(SendCurrentStep msg)
			{
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, msg.Cmd);
			}

			public override void HandleMessage(CommentVisibility msg)
			{
				if (msg.Visible)
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchCommentsCmd());
				}
			}

			public override void HandleMessage(ToolVisibility msg)
			{
				if (msg.Visible)
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchToolsCmd());
				}
			}

			public override void HandleMessage(ReferenceVisibility msg)
			{
				if (msg.Visible)
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchReferencesCmd());
				}
			}

			public override void HandleMessage(AnnotationVisibility msg)
			{
				if (msg.Visible)
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchAnnotationsCmd());
				}
			}

			public override void HandleMessage(LocalizationVisibility msg)
			{
				if (msg.Visible)
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchLocalizationCmd());
				}
			}

			public override void HandleMessage(ScenarioCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnLoadProject(cmd.ProjectName);
			}

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

				if (m_controller.m_callbacks.IsLocalizationShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(SwitchReferencesCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchReferences();

				if (m_controller.m_callbacks.IsReferencesShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(SwitchToolsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchTools();

				if (m_controller.m_callbacks.IsToolsShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(SwitchCommentsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchComments();

				if (m_controller.m_callbacks.IsCommentsShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(SwitchAnnotationsCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchAnnotation();

				if (m_controller.m_callbacks.IsAnnotationsShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(SwitchGUICmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnSwitchGUI();

				if (m_controller.m_callbacks.IsGUIShown())
				{
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);
				}
			}

			public override void HandleMessage(TakeScreenshotCmd cmd)
			{
				// "GUI" callback. It will call OnAnnotationRequest.
				m_controller.m_callbacks.CallOnTakeScreenshot();
			}

			public override void HandleMessage(DiagnosticCmd cmd)
			{
				// GUI callback
				m_controller.m_callbacks.CallOnOpenReceivedAnnotation(cmd.Accept);
				if (cmd.Accept)
				{
					// sends back to watch
					m_controller.SendCommand(m_controller.m_watchConnectionInfo, new SwitchAnnotationsCmd());
				}
			}

			public override void HandleMessage(DisconnectionCmd cmd)
			{
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, cmd);

				m_controller.CloseWatchConnection();

				Application.Quit();
			}

			public override void HandleMessage(AskReSynchronizationCmd cmd)
			{
				ScenarioState guiState = m_controller.m_callbacks.GetScenarioState();
				ReSynchronizationCmd reSyncCmd = new ReSynchronizationCmd(guiState);

				m_controller.SendCommand(m_controller.m_watchConnectionInfo, reSyncCmd);
			}

			public override void HandleMessage(AnnotationCmd cmd)
			{
				// "GUI" callback.
                Debug.Log("//////// watch State :" + cmd.StepPath); 
				m_controller.m_callbacks.CallOnAnnotationReceived(cmd.StepPath, cmd.ImageContent);

				// Sends diagnostic notification to the watch.
				// Here the flags "Accept" of the diagnostic command is not used, it will be set by the watch.
				// Sends false by default.
				DiagnosticCmd diagCmd = new DiagnosticCmd(cmd.StepPath, false);
				m_controller.SendCommand(m_controller.m_watchConnectionInfo, diagCmd);
			}
			#endregion IMessageVisitor implementation
		}
	}
}
