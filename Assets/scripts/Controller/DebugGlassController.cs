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
// - Fichier créé le 4/28/2015 2:10:52 PM
// - Dernière modification le 4/28/2015 2:10:52 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using System.IO;

namespace dassault
{
    /// <summary>
    /// description for class DebugGlassController
    /// </summary>
    public class DebugGlassController : GlassController
    {
        // Update is called once per frame
		protected override void PreUpdate()
        {
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				m_callbacks.CallOnPreviousStep();
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				m_callbacks.CallOnNextStep();
			}
			else if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				m_callbacks.CallOnSwitchLocalization();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad4))
			{
				m_callbacks.CallOnCurrentViewPrevious();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad6))
			{
				m_callbacks.CallOnCurrentViewNext();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad8))
			{
				m_callbacks.CallOnSwitchReferences();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad5))
			{
				m_callbacks.CallOnSwitchTools();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad2))
			{
				m_callbacks.CallOnSwitchComments();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad7))
			{
				m_callbacks.CallOnTakeScreenshot();
			}
			else if(Input.GetKeyDown(KeyCode.Keypad9))
			{
				m_callbacks.CallOnSwitchAnnotation();
			}
			else if(Input.GetKeyDown(KeyCode.G))
			{
				m_callbacks.CallOnSwitchGUI();
			}
			else if(Input.GetKeyDown(KeyCode.L))
			{
				m_callbacks.CallOnLoadProject("scenario1");
			}
			else if(Input.GetKeyDown(KeyCode.D))
			{
				Debug.Log ("Comments : " + m_callbacks.IsCommentsShown());
				Debug.Log ("References : " + m_callbacks.IsReferencesShown());
				Debug.Log ("Tools : " + m_callbacks.IsToolsShown());
				Debug.Log ("Annotations : " + m_callbacks.IsAnnotationsShown());
			}
			else if(Input.GetKeyDown(KeyCode.R))
			{
				byte[] image = SimulateImageReceptionFromServer();
				if(image != null)
				{
					string stepPath = m_callbacks.GetScenarioState().StepPath;
					m_callbacks.CallOnAnnotationReceived(stepPath, image);
				}
			}
			else if(Input.GetKeyDown(KeyCode.T))
			{
				m_callbacks.CallOnOpenReceivedAnnotation(true);
			}
			else if(Input.GetKeyDown(KeyCode.Y))
			{
				m_callbacks.CallOnOpenReceivedAnnotation(false);
			}
			else if(Input.GetKeyDown(KeyCode.O))
			{
				m_callbacks.CallSetWatchConnexionStatus(true);
			}
			else if(Input.GetKeyDown(KeyCode.P))
			{
				m_callbacks.CallSetPadConnexionStatus(true);
			}
			else if(Input.GetKeyDown(KeyCode.B))
			{
				m_scenarioState = m_callbacks.GetScenarioState();
			}
			else if(Input.GetKeyDown(KeyCode.N))
			{
				if(m_scenarioState != null)
				{
					m_callbacks.CallSetScenarioState(m_scenarioState);
				}
			}
			else if(Input.GetKeyDown(KeyCode.Alpha7))
			{
				m_callbacks.CallOnLoadBookmark(0);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha8))
			{
				m_callbacks.CallOnLoadBookmark(1);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha9))
			{
				m_callbacks.CallOnLoadBookmark(2);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha0))
            {
				m_callbacks.CallOnLoadBookmark(3);
            }
        }

		private byte[] SimulateImageReceptionFromServer()
		{
			string file = "c:\\temp\\annotation.png";
			if(File.Exists(file))
			{
				byte[] fileContent = File.ReadAllBytes(file);
				return fileContent;
			}
			return null;
		}

		public override void OnAnnotationRequest(string stepPath, byte[] image)
		{
			File.WriteAllBytes("c:\\temp\\cameraCapture.png", image);
		}

		public override void OnCurrentStepChanged(string stepPath)
		{
			//throw new System.NotImplementedException();
		}

		public override void OnCommentViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Comment view visibility : " + visible);
        }
        
        public override void OnToolViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Tool view visibility : " + visible);
        }
        
        public override void OnReferenceViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Reference view visibility : " + visible);
        }
        
        public override void OnAnnotationViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Annotation view visibility : " + visible);
        }
        
        public override void OnLocalizationViewVisibilityChanged(bool visible)
		{
			Debug.Log ("Localization view visibility : " + visible);
        }

        public override void OnStreamingServerStart(string url, int port)
        {
            Debug.Log("OnStreamingServerStart");
        }

        public override void addScenarioToList(string scenario)
        {
            Debug.Log("addScenarioToList");
        }

        public override void OnScenariiStatus(System.Collections.Generic.List<string> scenarii)
        {
            Debug.Log("OnScenariiStatus");
        }

		private ScenarioState m_scenarioState;
    }
}

