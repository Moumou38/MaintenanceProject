
namespace dassault
{
	public partial class ApplicationController
	{
		protected class ControllerState : IMessageVisitor
		{
			#region IMessageVisitor implementation
			public virtual void HandleMessage(ReceptionError msg) { }
			public virtual void HandleMessage(ConnectTo msg) { }
			public virtual void HandleMessage(SendAnnotation msg) { }
			public virtual void HandleMessage(SendCurrentStep msg) { }
			public virtual void HandleMessage(SendAnnotationAck msg) { }
			public virtual void HandleMessage(CommentVisibility msg) { }
			public virtual void HandleMessage(ToolVisibility msg) { }
			public virtual void HandleMessage(ReferenceVisibility msg) { }
			public virtual void HandleMessage(AnnotationVisibility msg) { }
			public virtual void HandleMessage(LocalizationVisibility msg) { }
			public virtual void HandleMessage(ScenarioCmd cmd) { }
			public virtual void HandleMessage(NextStepCmd cmd) { }
			public virtual void HandleMessage(PreviousStepCmd cmd) { }
			public virtual void HandleMessage(BookmarksCmd cmd) { }
			public virtual void HandleMessage(CurrentViewNextCmd cmd) { }
			public virtual void HandleMessage(CurrentViewPreviousCmd cmd) { }
			public virtual void HandleMessage(SwitchLocalizationCmd cmd) { }
			public virtual void HandleMessage(SwitchReferencesCmd cmd) { }
			public virtual void HandleMessage(SwitchToolsCmd cmd) { }
			public virtual void HandleMessage(SwitchCommentsCmd cmd) { }
			public virtual void HandleMessage(SwitchAnnotationsCmd cmd) { }
			public virtual void HandleMessage(SwitchGUICmd cmd) { }
			public virtual void HandleMessage(WatchScreenChangedCmd cmd) { }
			public virtual void HandleMessage(WatchStepPathChangedCmd cmd) { }
			public virtual void HandleMessage(TakeScreenshotCmd cmd) { }
			public virtual void HandleMessage(DiagnosticCmd cmd) { }
			public virtual void HandleMessage(ConnectionCmd cmd) { }
			public virtual void HandleMessage(DisconnectionCmd cmd) { }
			public virtual void HandleMessage(ConnectedStatus sts) { }
			public virtual void HandleMessage(AskReSynchronizationCmd cmd) { }
			public virtual void HandleMessage(ReSynchronizationCmd cmd) { }
			public virtual void HandleMessage(ConnectionRefusedStatus sts) { }
			public virtual void HandleMessage(AnnotationCmd cmd) { }
            public virtual void HandleMessage(CaptureCmd cmd) { }
            public virtual void HandleMessage(ScenariiStatusCmd cmd) { }
            public virtual void HandleMessage(ShowNewProcedureCmd cmd) { }
            public virtual void HandleMessage(ConnectionToSupportCmd cmd) { }
            public virtual void HandleMessage(DisconnectionToSupportCmd cmd) { }
            public virtual void HandleMessage(SupportStatusCmd cmd) { }
            public virtual void HandleMessage(StreamParametersCmd cmd){ }
            public virtual void HandleMessage(ConnectedToStreamCmd cmd){ }
            public virtual void HandleMessage(DownloadProcedureFromURLCmd cmd) { }
            #endregion IMessageVisitor implementation

            #region Public Methods
            public virtual void Update() { }
			public virtual void OnEnter() { }
			public virtual void OnExit() { }
			public virtual void OnEmissionError(int clientId, int errorCode) { }
			#endregion Public Methods
		}
	}
}
