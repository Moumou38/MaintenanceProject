// Copyright 2015 Dassault
//
// - Description
//     
//
// - Namespace(s)
//    dassault
//
// - Auteurs
//    \author Michel de Verdelhan <mdeverdelhan@theoris.fr>
//
// - Fichier créé le 4/28/2015 2:08:15 PM
// - Dernière modification le 4/28/2015 2:08:15 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class GlassController
    /// </summary>
    public class GlassControllerCallbacks
    {
		// evenement issue de la montre connectée
		public delegate void WatchEvent();

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "next step" de la montre connectée
		/// </summary>
		public event WatchEvent OnNextStep;
		public void CallOnNextStep()
		{
			if(OnNextStep != null)
				OnNextStep();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "previous step" de la montre connectée
		/// </summary>
		public event WatchEvent OnPreviousStep;
		public void CallOnPreviousStep()
		{
			if(OnPreviousStep != null)
				OnPreviousStep();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "localisation" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchLocalization;
		public void CallOnSwitchLocalization()
		{
			if(OnSwitchLocalization != null)
				OnSwitchLocalization();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "élément précédent de la vue courante" sur la montre connectée
		/// </summary>
		public event WatchEvent OnCurrentViewPrevious;
		public void CallOnCurrentViewPrevious()
		{
			if(OnCurrentViewPrevious != null)
				OnCurrentViewPrevious();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "élément suivant de la vue courante" sur la montre connectée
		/// </summary>
		public event WatchEvent OnCurrentViewNext;
		public void CallOnCurrentViewNext()
		{
			if(OnCurrentViewNext != null)
				OnCurrentViewNext();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "affichage des references/images" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchReferences;
		public void CallOnSwitchReferences()
		{
			if(OnSwitchReferences != null)
				OnSwitchReferences();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "affichage des outils" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchTools;
		public void CallOnSwitchTools()
		{
			if(OnSwitchTools != null)
				OnSwitchTools();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "affichage des commentaires" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchComments;
		public void CallOnSwitchComments()
		{
			if(OnSwitchComments != null)
				OnSwitchComments();
		}

		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "affichage des annotations" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchAnnotation;
		public void CallOnSwitchAnnotation()
		{
			if(OnSwitchAnnotation != null)
				OnSwitchAnnotation();
		}
		
		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "affichage des annotations" de la montre connectée
		/// </summary>
		public event WatchEvent OnSwitchGUI;
		public void CallOnSwitchGUI()
		{
			if(OnSwitchGUI != null)
				OnSwitchGUI();
		}
		
		/// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton "demande d'aide" de la montre connectée
		/// </summary>
		public event WatchEvent OnTakeScreenshot;
		public void CallOnTakeScreenshot()
		{
			if(OnTakeScreenshot != null)
                OnTakeScreenshot();
        }
        
		public delegate void WatchEventOpenReceivedAnnotation(bool open);
        /// <summary>
		/// evenement appelé lorsque l'utilisateur clique sur le bouton d'ouverture de l'annotation reçu
		/// </summary>
		public event WatchEventOpenReceivedAnnotation OnOpenReceivedAnnotation;
		public void CallOnOpenReceivedAnnotation(bool open)
		{
			if(OnOpenReceivedAnnotation != null)
				OnOpenReceivedAnnotation(open);
        }
        
		public delegate void WatchEventLoadBookmark(int bookmarkIndex);
        /// <summary>
		/// evenement appelé lorsque la tablette demande au lunettes de charger un projet
		/// </summary>
		public event WatchEventLoadBookmark OnLoadBookmark;
		public void CallOnLoadBookmark(int bookmarkIndex)
		{
			if(OnLoadBookmark != null)
				OnLoadBookmark(bookmarkIndex);
		}

		// evenement issue de la tablette
        public delegate void PadEventAnnotationReceived(string stepPath, byte[] imageContent);
		/// <summary>
		/// evenement appelé lorsque les lunettes recoivent une annotation
		/// </summary>
		public event PadEventAnnotationReceived OnAnnotationReceived;
		public void CallOnAnnotationReceived(string stepPath, byte[] imageContent)
		{
			if(OnAnnotationReceived != null)
				OnAnnotationReceived(stepPath, imageContent);
		}

		public delegate void PadEventLoadProject(string projectName);
		/// <summary>
		/// evenement appelé lorsque la tablette demande au lunettes de charger un projet
		/// </summary>
		public event PadEventLoadProject OnLoadProject;
		public void CallOnLoadProject(string projectName)
		{
			if(OnLoadProject != null)
				OnLoadProject(projectName);
		}

		// evenement issue de la montre connectée
		public delegate bool GlassStateEvent();
		public GlassStateEvent IsLocalizationShown;
		public GlassStateEvent IsCommentsShown;
		public GlassStateEvent IsReferencesShown;
		public GlassStateEvent IsToolsShown;
		public GlassStateEvent IsAnnotationsShown;
		public GlassStateEvent IsGUIShown;
		public GlassStateEvent HasNextComment;
		public GlassStateEvent HasNextReference;
		public GlassStateEvent HasNextTool;
		public GlassStateEvent HasNextAnnotation;

		public delegate void ConnexionStatusDelegate(bool connected);
		public event ConnexionStatusDelegate SetWatchConnexionStatus;
		public void CallSetWatchConnexionStatus(bool connected)
		{
			if (SetWatchConnexionStatus != null)
				SetWatchConnexionStatus(connected);
		}
		public event ConnexionStatusDelegate SetPadConnexionStatus;
		public void CallSetPadConnexionStatus(bool connected)
		{
			if(SetPadConnexionStatus != null)
				SetPadConnexionStatus(connected);
		}

		public delegate ScenarioState ScenarioStateGetter();
		public ScenarioStateGetter GetScenarioState;

		public delegate void SetScenarioStateDelegate(ScenarioState state);
		public event SetScenarioStateDelegate SetScenarioState;
		public void CallSetScenarioState(ScenarioState state)
		{
			if (SetScenarioState != null)
				SetScenarioState(state);
		}

        // streaming server start request event
        public delegate bool streamingServerStartRequest(out string url, out int port);
        public event streamingServerStartRequest StartVideoStreamingServer;
        public bool CallStartVideoStreamingServer(out string url, out int port)
        {
            if (StartVideoStreamingServer != null)
            {
                return StartVideoStreamingServer(out url, out port);
            }
            url = "failed";
            port = 0;
            return false;
        }

        // download procedure package request event
        public delegate bool DownloadProcedureFromURLRequest(string name, string url);
        public event DownloadProcedureFromURLRequest DownloadProcedureFromURL;
        public bool CallOnDownloadProcedureFromURL(string name, string url)
        {
            if (DownloadProcedureFromURL != null)
            {
                return DownloadProcedureFromURL(name, url);
            }
            return false;
        }

    }
}
