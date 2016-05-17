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
// - Fichier créé le 4/23/2015 10:16:28 AM
// - Dernière modification le 4/23/2015 10:16:28 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml;

namespace dassault
{
    /// <summary>
    /// description for class PadApplicationModule
    /// </summary>
	public class PadApplicationModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			GUIModule guiModule = repository.Get("GUIModuleGlass") as GUIModule;
			m_guiModulePad = repository.Get("GUIModulePad") as GUIModulePad;
			PlaneViewModule planeViewModule = repository.Get("PlaneViewModule") as PlaneViewModule;
			AnimationModule animationModule = repository.Get("AnimationModule") as AnimationModule;
            StepScreen stepScreen = guiModule.StepScreen;
			m_watchScreen = m_guiModulePad.WatchScreen;
			m_connectionScreen = m_guiModulePad.ConnectionScreen;

			m_guiModulePad.AddToGlassViewport(stepScreen.transform as RectTransform);
			stepScreen.ResetOriginalPosition();
			stepScreen.ShowStatusBar(false);
			
			m_stepViewDriver = new StepViewDriver(planeViewModule, animationModule, stepScreen, false);
			m_stepViewDriver.Show(false);
			m_stepViewDriver.OnCommentViewVisibilityChangedCallback += OnCommentViewVisibilityChanged;
			m_stepViewDriver.OnToolViewVisibilityChangedCallback += OnToolViewVisibilityChanged;
			m_stepViewDriver.OnReferenceViewVisibilityChangedCallback += OnReferenceViewVisibilityChanged;
			m_stepViewDriver.OnAnnotationViewVisibilityChangedCallback += OnAnnotationViewVisibilityChanged;
			m_stepViewDriver.OnLocalizationViewVisibilityChangedCallback += OnLocalizationViewVisibilityChanged;
            
			m_annotationModule = repository.Get("AnnotationModule") as AnnotationModule;
			
			m_annotationScreen = m_guiModulePad.AnnotationScreen;
			m_annotationScreen.OnColorChangedCallback += OnAnnotationColorChanged;
			m_annotationScreen.OnLineSizeChangedCallback += OnAnnotationLineSizeChanged;
			m_annotationScreen.OnValidateCallback += OnAnnotationValidated;	
			m_annotationScreen.OnCancelCallback += OnAnnotationCanceled;	

			m_connectionScreen.OnDeviceSelectedCallback += OnDeviceToConnectSelected;

			PadControllerCallbacks padCallbacks = new PadControllerCallbacks();
			padCallbacks.OnCaptureReceived += OnCaptureReceived;
			padCallbacks.OnWatchScreenChanged += SetCurrentWatchView;
			padCallbacks.OnWatchStepPathChanged += OnWatchStepPathChanged;
			padCallbacks.OnServerCreated += OnBluetoothServerStarted;
			padCallbacks.OnConnectionResult += OnBluetoothConnectionResult;

			GlassControllerCallbacks glassCallbacks = new GlassControllerCallbacks();
			glassCallbacks.OnNextStep += OnNextStep;
			glassCallbacks.OnPreviousStep += OnPreviousStep;
			glassCallbacks.OnCurrentViewPrevious += OnCurrentViewPrevious;
			glassCallbacks.OnCurrentViewNext += OnCurrentViewNext;
			glassCallbacks.OnSwitchLocalization += OnSwitchLocalization;
			glassCallbacks.OnSwitchReferences += OnSwitchReferences;
			glassCallbacks.OnSwitchTools += OnSwitchTools;
			glassCallbacks.OnSwitchComments += OnSwitchComments;
			glassCallbacks.OnSwitchAnnotation += OnSwitchAnnotation;
			glassCallbacks.OnSwitchGUI += OnSwitchGUI;
			glassCallbacks.OnLoadBookmark += OnLoadBookmark;
			glassCallbacks.OnAnnotationReceived += ReceiveAnnotation;
			glassCallbacks.OnOpenReceivedAnnotation += OnOpenReceivedAnnotation;
			glassCallbacks.OnLoadProject += LoadProject;
			glassCallbacks.IsLocalizationShown = IsLocalizationShown;
            glassCallbacks.IsCommentsShown = IsCommentsAvailable;
			glassCallbacks.IsReferencesShown = IsReferencesAvailable;
			glassCallbacks.IsToolsShown = IsToolsAvailable;
			glassCallbacks.IsAnnotationsShown = IsAnnotationsAvailable;
			glassCallbacks.SetWatchConnexionStatus += SetWatchConnexionStatus;
			glassCallbacks.SetPadConnexionStatus += SetPadConnexionStatus;
			glassCallbacks.GetScenarioState = GetScenarioState;
			glassCallbacks.SetScenarioState += SetScenarioState;

			SetWatchConnexionStatus(false);
			SetPadConnexionStatus(false);

#if UNITY_ANDROID
			string deviceFilePath = "/mnt/sdcard/DispositifMainLibre/devices.xml";
#else
			string deviceFilePath = Application.dataPath + "/../Datas/devices.xml";
#endif
			ReadDeviceFile(deviceFilePath);
			m_connectionScreen.Show(true);
			m_connectionScreen.ShowList(false);
			m_connectionScreen.SetFeedbackMessage("Démarrage du serveur bluetooth");
			m_connectionScreen.SetErrorMessage("");

			IBTServices btServices = null;

#if UNITY_ANDROID
			btServices = BluetoothDataExchange.GetInstance();
#else
			btServices = new BTServicesWindows();
#endif

			m_controller = gameObject.AddComponent<ConcretePadController>().Init(padCallbacks, glassCallbacks, btServices);
		}

		private void ReadDeviceFile(string path)
		{
			if(File.Exists(path))
			{
				XmlDocument document = new XmlDocument();
				document.Load(path);
				XmlNode root = document.FirstChild;
				XmlNodeList nodes = document.GetElementsByTagName("device");
				foreach(XmlNode node in nodes)
				{
					XmlElement element = node as XmlElement;
					string name = element.GetAttribute("name");
					string address = element.GetAttribute("address");
					m_connectionScreen.AddSelectableDevice(name, address);
				}
			}
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			result.Add("GUIModuleGlass");
			result.Add("GUIModulePad");
			result.Add("PlaneViewModule");
			result.Add("AnimationModule");
			result.Add("AnnotationModule");
			return result;
		}
		
		private new void Awake()
		{
			base.Awake();
		}

		private void Update()
		{
			if(m_stepViewDriver != null)
			{
				m_stepViewDriver.UpdateViewports();
			}
			DebugKeyShortcuts();
		}

		private void OnAnnotationColorChanged(Color color)
		{
			m_annotationModule.SetLineColor(color);
		}
		
		private void OnAnnotationLineSizeChanged(float lineSize)
		{
			m_annotationModule.SetLineSize(lineSize);
		}
		
		private void OnAnnotationValidated()
		{
			byte[] image = m_annotationModule.TakeScreenshot();
			m_annotationScreen.Show(false);
			m_annotationModule.Activate(false);
			m_stepViewDriver.Show(true);
			m_controller.SendAnnotationBackToGlasses(m_currentAnnotationStep, image);
			m_guiModulePad.ShowGlassViewport(true);
			m_guiModulePad.ShowWatchScreen(true);
		}
		
		private void OnAnnotationCanceled()
		{
			m_annotationScreen.Show(false);
			m_annotationModule.Activate(false);
			m_stepViewDriver.Show(true);
			m_guiModulePad.ShowGlassViewport(true);
			m_guiModulePad.ShowWatchScreen(true);
		}
		
		private void OnCaptureReceived(string step, byte[] annotationImage)
		{
			m_annotationModule.Activate(true);
			m_annotationScreen.Show(true);
			m_annotationModule.SetCameraViewport(m_annotationScreen.GetViewport());
			m_annotationModule.SetTexture(annotationImage);
			m_currentAnnotationStep = step;
			m_guiModulePad.ShowGlassViewport(false);
			m_guiModulePad.ShowWatchScreen(false);
		}

		private void SetCurrentWatchView(int index)
		{
			m_watchScreen.SetCurrentView(index);
		}

		private void OnWatchStepPathChanged(string stepPath)
		{
			m_watchScreen.SetCurrentStepPath(stepPath);
		}

		private void ReceiveAnnotation(string stepPath, byte[] imageAsPng)
		{
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(imageAsPng);
			m_stepViewDriver.AddAnnotationToStep(tex, stepPath);
		}

		private void OnNextStep()
		{
			m_stepViewDriver.NextStep();
			m_watchScreen.SimulateNextStepClick();
		}
		
		private void OnPreviousStep()
		{
			m_stepViewDriver.PreviousStep();
			m_watchScreen.SimulatePreviousStepClick();
		}
		
		private void OnCurrentViewPrevious()
		{
			m_stepViewDriver.ShowCurrentViewPrevInfo();
			m_watchScreen.SimulatePreviousElementClick();
		}
		
		private void OnCurrentViewNext()
		{
			m_stepViewDriver.ShowCurrentViewNextInfo();
			m_watchScreen.SimulateNextElementClick();
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
			m_watchScreen.SetGUIVisibility(m_stepViewDriver.IsGuiShown());
		}
		
		private void OnCommentViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Comment view visibility " + visible);
			m_watchScreen.SetCommentsVisibility(visible);
        }
		
		private void OnToolViewVisibilityChanged(bool visible)
		{
			// rien
		}
		
		private void OnReferenceViewVisibilityChanged(bool visible)
		{
			Debug.Log ("reference view visibility " + visible);
			m_watchScreen.SetReferencesVisibility(visible);
        }
		
		private void OnAnnotationViewVisibilityChanged(bool visible)
		{
			Debug.Log ("annotation view visibility " + visible);
			m_watchScreen.SetAnnotationVisibility(visible);
        }
		
		private void OnLocalizationViewVisibilityChanged(bool visible)
        {
			Debug.Log ("localization view visibility " + visible);
			m_watchScreen.SetLocalizationVisibility(visible);
        }
        
		private void OnLoadBookmark(int bookmarkIndex)
		{
			m_stepViewDriver.SetCurrentStepAsBookmark(bookmarkIndex);
			switch(bookmarkIndex)
			{
			case 0:
				m_watchScreen.SimulateHomeClick();
				break;
			case 1:
				m_watchScreen.SimulateBookmark1Click();
				break;
			case 2:
				m_watchScreen.SimulateBookmark2Click();
				break;
			case 3:
				m_watchScreen.SimulateBookmark3Click();
				break;
			}
		}

		private bool IsLocalizationShown()
		{
			return m_stepViewDriver.IsLocalizationShown();
		}
		
		private bool IsCommentsAvailable()
		{
			return m_stepViewDriver.IsCommentsShown();
		}
		
		private bool IsReferencesAvailable()
		{
			return m_stepViewDriver.IsReferencesShown();
		}
		
		private bool IsToolsAvailable()
		{
			return m_stepViewDriver.IsToolsShown();
		}
		
		private bool IsAnnotationsAvailable()
		{
			return m_stepViewDriver.IsAnnotationsShown();
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
			//m_stepViewDriver.LoadScenario(projectName);
		}

		private void DebugKeyShortcuts()
		{
			if(Input.GetKeyDown(KeyCode.W))
			{
				m_stepViewDriver.SetCurrentStepByPath("1/1>4/5>1/2>6/11");
			}
		}

		private void SetWatchConnexionStatus(bool connected)
		{
			m_stepViewDriver.SetWatchConnexionStatus(connected);
		}
		
		private void SetPadConnexionStatus(bool connected)
		{
			m_stepViewDriver.SetPadConnexionStatus(connected);
		}

		private void OnDeviceToConnectSelected(string deviceName, string deviceAddress)
		{
			StartCoroutine(WaitAndConnect(deviceName, deviceAddress));
		}

		private IEnumerator WaitAndConnect(string deviceName, string deviceAddress)
		{
			m_connectionScreen.SetFeedbackMessage("Connexion aux lunettes " + deviceName);
			yield return new WaitForSeconds(2);
			m_controller.ConnectToDevice(deviceName, deviceAddress);
		}

		private void OnBluetoothServerStarted(bool started)
		{
			m_connectionScreen.SetFeedbackMessage("");
			if(started)
			{
				m_connectionScreen.SetErrorMessage("");
				m_connectionScreen.ShowList(true);
			}
			else
			{
				m_connectionScreen.SetErrorMessage("Erreur lors de la création du serveur bluetooth");
				// TODO : afficher la popup pour quitter
				Debug.Log ("server creation failed");
			}
		}

		private void OnBluetoothConnectionResult(bool connected)
		{
			if(connected)
			{
				m_connectionScreen.Show(false);
				m_watchScreen.Show(true);
				m_stepViewDriver.Show(true);
			}
			else
			{
				m_connectionScreen.SetFeedbackMessage("");
				m_connectionScreen.SetErrorMessage("Erreur lors de la connexion");
			}
		}

		private ScenarioState GetScenarioState()
		{
			return m_stepViewDriver.GetCurrentState();
		}
		
		private void SetScenarioState(ScenarioState state)
		{
			m_stepViewDriver.SetCurrentState(state);
		}

		private StepViewDriver m_stepViewDriver;
		private AnnotationScreen m_annotationScreen;
		private AnnotationModule m_annotationModule;
		private WatchScreen m_watchScreen;
		private ConnectionScreen m_connectionScreen;
		private PadController m_controller;
		private GUIModulePad m_guiModulePad;
		private string m_currentAnnotationStep;
	}
}
