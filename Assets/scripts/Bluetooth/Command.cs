using System;
namespace dassault
{
	/// <summary>
	/// Enum used to translate received data to a command.
	/// </summary>
	public enum CommandType : byte
	{
		Scenario = 1,   // demande de chargement de scenario (from watch)
		NextStep,
		PreviousStep,
		Bookmarks,
		CurrentViewNext,
		CurrentViewPrevious,
		SwitchLocalization, 
		SwitchReferences,
		SwitchTools,
		SwitchComments, // 10
		SwitchAnnotations, 
		SwitchGUI,
		WatchScreenChanged,
		WatchStepPathChanged,
		TakeScreenshot,
		Diagnostic,
		Connection,
		Disconnection,
		Connected,
		AskReSynchronization, // 20
		ReSynchronization, 
		ConnectionRefused,
		Annotation,
		Capture,
        ScenariiStatus, // 25 // mise a jour pour la montre de la liste des scenario disponibles (au debut de l'execution)
        ShowNewProcedure,   // ajout d'un scenario dans la liste des scenarios affichés sur la montre
        ConnectionToSupport,
        DisconnectionToSupport,
        SupportStatus,
        StreamParameters,
        ConnectedToStream,
        DownloadProcedureFromURL
    }
}

