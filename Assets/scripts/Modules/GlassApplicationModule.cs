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
// - Fichier créé le 4/20/2015 10:23:15 AM
// - Dernière modification le 4/20/2015 10:23:15 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using loadingPackageModule; 

namespace dassault
{
    /// <summary>
    /// description for class ApplicationModule
    /// </summary>
    public class GlassApplicationModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			m_GuiModule = repository.Get("GUIModuleGlass") as GUIModule;
			PlaneViewModule planeViewModule = repository.Get("PlaneViewModule") as PlaneViewModule;
			AnimationModule animationModule = repository.Get("AnimationModule") as AnimationModule;

            StepScreen stepScreen = m_GuiModule.StepScreen;
            m_captureFeedbackScreen = m_GuiModule.CaptureFeedbackScreen;

            m_StreamingServerModule = repository.Get("StreamingServerModule") as StreamingServerModule;

            m_loadingPackageModule = repository.Get("LoadingPackageModule") as LoadingPackageModule;
            m_loadingPackageModule.onLoadingDataState += onExtraction; 

            stepScreen.ResetOriginalPosition();


            m_stepViewDriver = new StepViewDriver(planeViewModule, animationModule, stepScreen, true);

			m_stepViewDriver.OnStepChangedCallback += OnCurrentStepChanged;
			m_stepViewDriver.OnCommentViewVisibilityChangedCallback += OnCommentViewVisibilityChanged;
			m_stepViewDriver.OnToolViewVisibilityChangedCallback += OnToolViewVisibilityChanged;
			m_stepViewDriver.OnReferenceViewVisibilityChangedCallback += OnReferenceViewVisibilityChanged;
			m_stepViewDriver.OnAnnotationViewVisibilityChangedCallback += OnAnnotationViewVisibilityChanged;
			m_stepViewDriver.OnLocalizationViewVisibilityChangedCallback += OnLocalizationViewVisibilityChanged;

            m_loadingPackageModule.onAssetBundleDownloaded += addScenarioToList; 
            
            m_cameraTexture = new WebCamTexture(640, 480);
			m_captureFeedbackScreen.SetCameraTexture(m_cameraTexture);

			GlassControllerCallbacks callbacks = new GlassControllerCallbacks();
			callbacks.OnNextStep += OnNextStep;
			callbacks.OnPreviousStep += OnPreviousStep;
			callbacks.OnCurrentViewPrevious += OnCurrentViewPrevious;
			callbacks.OnCurrentViewNext += OnCurrentViewNext;
			callbacks.OnSwitchLocalization += OnSwitchLocalization;
			callbacks.OnSwitchReferences += OnSwitchReferences;
			callbacks.OnSwitchTools += OnSwitchTools;
			callbacks.OnSwitchComments += OnSwitchComments;
			callbacks.OnSwitchAnnotation += OnSwitchAnnotation;
			callbacks.OnSwitchGUI += OnSwitchGUI;
			callbacks.OnLoadBookmark += OnLoadBookmark;
			callbacks.OnTakeScreenshot += RequestHelp;
			callbacks.OnAnnotationReceived += ReceiveAnnotation;
			callbacks.OnOpenReceivedAnnotation += OnOpenReceivedAnnotation;
			callbacks.OnLoadProject += LoadProject;
			callbacks.IsLocalizationShown = IsLocalizationShown;
			callbacks.IsCommentsShown = IsCommentsShown;
			callbacks.IsReferencesShown = IsReferencesShown;
			callbacks.IsToolsShown = IsToolsShown;
			callbacks.IsAnnotationsShown = IsAnnotationsShown;
			callbacks.IsGUIShown = IsGUIShown;
			callbacks.HasNextComment = HasNextComment;
			callbacks.HasNextReference = HasNextReference;
			callbacks.HasNextTool = HasNextTool;
			callbacks.HasNextAnnotation = HasNextAnnotation;
			callbacks.SetWatchConnexionStatus += SetWatchConnexionStatus;
			callbacks.SetPadConnexionStatus += SetPadConnexionStatus;
			callbacks.GetScenarioState = GetScenarioState;
			callbacks.SetScenarioState += SetScenarioState;
            callbacks.StartVideoStreamingServer += startVideoStreaming;
            callbacks.DownloadProcedureFromURL += DownloadProcedureFromURL;

			SetWatchConnexionStatus(false);
			SetPadConnexionStatus(false);
            IBTServices btServices = null;

#if UNITY_ANDROID
			btServices = BluetoothDataExchange.GetInstance();
#else
			btServices = new BTServicesWindows();
#endif

#if UNITY_ANDROID
			m_controller = gameObject.AddComponent<ConcreteGlassController>().Init(callbacks, btServices);
#else
			m_controller = gameObject.AddComponent<ConcreteGlassController>().Init(callbacks, btServices);
			//m_controller = gameObject.AddComponent<DebugGlassController>().Init(callbacks);
#endif
            
            
		}

        void onExtraction(loadingPackageModule.LoadingPackageModule.LoadingState iState)
        {
            if (iState == loadingPackageModule.LoadingPackageModule.LoadingState.STOP)
            {
                m_GuiModule.ButtonContainer.SetActive(true);
                m_GuiModule.ExtractingData.SetActive(false); 
            }

            if (iState == loadingPackageModule.LoadingPackageModule.LoadingState.START)
            {
                m_GuiModule.ExtractingData.SetActive(true); 
            }
        }
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			result.Add("GUIModuleGlass");
			result.Add("PlaneViewModule");
			result.Add("AnimationModule");
            result.Add("StreamingServerModule");
            result.Add("LoadingPackageModule");
            result.Add("AssetDispatchingModule"); 
			return result;
		}

		private new void Awake()
		{
			base.Awake();

            Debug.Log("forcing screen resolution to 960x540 (resolution for which the gui is designed)");
            Screen.SetResolution(960, 540, false);
		}

		private void Update()
		{
			if(m_stepViewDriver != null)
			{
				m_stepViewDriver.UpdateViewports();
			}
		}

		private void OnNextStep()
		{
			m_stepViewDriver.NextStep();
		}
		
		private void OnPreviousStep()
		{
			m_stepViewDriver.PreviousStep();
		}

		private void OnCurrentViewPrevious()
		{
			m_stepViewDriver.ShowCurrentViewPrevInfo();
		}
		
		private void OnCurrentViewNext()
		{
			m_stepViewDriver.ShowCurrentViewNextInfo();
		}

		private void OnSwitchLocalization()
		{
			m_stepViewDriver.SwitchLocalizationView();
		}
		
		private void OnSwitchComments()
		{
			m_stepViewDriver.SwitchCommentView();
		}
		
		private void OnSwitchReferences()
		{
			m_stepViewDriver.SwitchImageView();
		}
		
		private void OnSwitchTools()
		{
			m_stepViewDriver.SwitchToolsView();
		}
		
		private void OnSwitchAnnotation()
		{
			m_stepViewDriver.SwitchAnnotationView();
		}

		private void OnSwitchGUI()
		{
			m_stepViewDriver.SwitchVisibility();
		}
		
		private void OnLoadBookmark(int bookmarkIndex)
		{
			m_stepViewDriver.SetCurrentStepAsBookmark(bookmarkIndex);
        }

		private bool IsLocalizationShown()
		{
			return m_stepViewDriver.IsLocalizationShown();
		}

		private bool IsCommentsShown()
		{
			return m_stepViewDriver.IsCommentsShown();
		}
		
		private bool IsReferencesShown()
		{
			return m_stepViewDriver.IsReferencesShown();
		}
		
		private bool IsToolsShown()
		{
			return m_stepViewDriver.IsToolsShown();
		}
		
		private bool IsAnnotationsShown()
		{
			return m_stepViewDriver.IsAnnotationsShown();
		}

		private void OnCommentViewVisibilityChanged(bool visible)
		{
			m_controller.OnCommentViewVisibilityChanged(visible);
		}
		
		private void OnToolViewVisibilityChanged(bool visible)
		{
			m_controller.OnToolViewVisibilityChanged(visible);
        }
		
		private void OnReferenceViewVisibilityChanged(bool visible)
		{
			m_controller.OnReferenceViewVisibilityChanged(visible);
        }
		
		private void OnAnnotationViewVisibilityChanged(bool visible)
		{
			m_controller.OnAnnotationViewVisibilityChanged(visible);
        }
		
		private void OnLocalizationViewVisibilityChanged(bool visible)
        {
			m_controller.OnLocalizationViewVisibilityChanged(visible);
        }
        
        private bool HasNextComment()
        {
            return m_stepViewDriver.HasNextComment();
		}
		
		private bool HasNextReference()
		{
			return m_stepViewDriver.HasNextReference();
		}
		
		private bool HasNextTool()
		{
			return m_stepViewDriver.HasNextTool();
		}
		
		private bool HasNextAnnotation()
		{
			return m_stepViewDriver.HasNextAnnotation();
		}
		
		private bool IsGUIShown()
		{
			return m_stepViewDriver.IsGuiShown();
		}

		private void RequestHelp()
		{
			if(m_screenshotCoroutine != null)
				StopCoroutine(m_screenshotCoroutine);
			m_screenshotCoroutine = StartCoroutine(DeferredRequestHelp());
		}

		private bool IsBlack(Color32[] buffer)
		{
			int[] indicesToTest = new int[10];
			indicesToTest[0] = 0;
			indicesToTest[1] = buffer.Length / 2;
			indicesToTest[2] = buffer.Length - 1;
			for(int i = 3; i < 10; ++i)
			{
				indicesToTest[i] = Random.Range(0, buffer.Length);
			}
			foreach(int index in indicesToTest)
			{
				if(buffer[index] != Color.black)
					return false;
			}
			return true;
		}

		private IEnumerator DeferredRequestHelp()
		{
			m_cameraTexture.Play();
			int frameCount = 0;
			bool hasPixels = false;
			while(IsBlack(m_cameraTexture.GetPixels32()))
			{
				yield return new WaitForEndOfFrame();
				frameCount++;
			}
			Debug.Log ("Frame count to wait : " + frameCount);
			m_captureFeedbackScreen.Show(true);
			for(int i = 5; i > 0; --i)
			{
				m_captureFeedbackScreen.SetTimer(i);
				yield return new WaitForSeconds(1);
			}
			m_captureFeedbackScreen.SetTimer(0);
			m_captureFeedbackScreen.StartFlash();
			yield return new WaitForSeconds(1.0f);
			byte[] image = TakeCameraShot();
			string currentStepPath = m_stepViewDriver.GetCurrentStepPath();
			m_controller.OnAnnotationRequest(currentStepPath, image); 
			m_cameraTexture.Stop();
			m_captureFeedbackScreen.Show(false);
			m_screenshotCoroutine = null;
		}

		private byte[] TakeCameraShot()
		{
			Texture2D tex = new Texture2D(m_cameraTexture.width, m_cameraTexture.height, TextureFormat.ARGB32, false);
			Color32[] pixels = m_cameraTexture.GetPixels32();
			tex.SetPixels32(pixels);
			tex.Apply();
			byte[] result = tex.EncodeToPNG();
			return result;
		}

		private void ReceiveAnnotation(string stepPath, byte[] imageAsPng)
		{
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(imageAsPng);
			m_stepViewDriver.AddAnnotationToStep(tex, stepPath);
		}

		private void OnOpenReceivedAnnotation(bool open)
		{
			if(open)
				m_stepViewDriver.OpenReceivedAnnotationStep();
			else
				m_stepViewDriver.HideAnnotationReceptionView();
		}

		private void LoadProject(string projectName)
		{
            m_loadingPackageModule.onStartInspection(projectName);
             
			//m_stepViewDriver.LoadScenario(projectName);
			//m_controller.OnCurrentStepChanged(m_stepViewDriver.GetCurrentStepPath());
		}

		private void SetWatchConnexionStatus(bool connected)
		{
			m_stepViewDriver.SetWatchConnexionStatus(connected);
            if (connected == true)
            {
                List<string> BundleList = m_loadingPackageModule.initializeLocalAssetBundleList();

                if (BundleList != null)
                {
                    // update the list of available scenario, based on the scenario list computed by the LoadingPackageModule
                    m_controller.OnScenariiStatus(BundleList);
                }
            }
		}
		
		private void SetPadConnexionStatus(bool connected)
		{
			m_stepViewDriver.SetPadConnexionStatus(connected);
		}

		private void OnCurrentStepChanged(string stepPath)
		{
			m_controller.OnCurrentStepChanged(stepPath);
		}

		private ScenarioState GetScenarioState()
		{
			return m_stepViewDriver.GetCurrentState();
		}

		private void SetScenarioState(ScenarioState state)
		{         
			m_stepViewDriver.SetCurrentState(state);
		}

        private bool startVideoStreaming(out string url, out int port)
        {
             if (m_StreamingServerModule.startStreaming(out url, out port))
             {
                 // streaming server successfully started, send the connection information
                 return true;
             }
             else
             {
                 return false;  
             }
        }

        public void setScenario(TextAsset iText, Dictionary<string, Object> iObjects)
        {
            if (m_stepViewDriver != null)
            {
                m_stepViewDriver.initObjects(iText, iObjects);
            }
        }

        public bool DownloadProcedureFromURL(string name, string url)
        {
            // this method is called by the expert to trigger a download on an url
            if (m_loadingPackageModule != null)
            {
                m_loadingPackageModule.requestAssetBundle(name, url);
            }

            // on sait pas , donc ca doit etre bon...
            return true;
        }

        // public method used to add a scenario on the watch

        public void addScenarioToList(string scenario)
        {
            m_controller.addScenarioToList(scenario);
        }

        private GUIModule m_GuiModule;
        private StepViewDriver m_stepViewDriver;
		private CaptureFeedbackScreen m_captureFeedbackScreen;
		private GlassController m_controller;
		private Coroutine m_screenshotCoroutine;
		[SerializeField] private WebCamTexture m_cameraTexture;

        private StreamingServerModule m_StreamingServerModule;
        private LoadingPackageModule m_loadingPackageModule;
    }
}
