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
// - Fichier créé le 5/26/2015 9:53:48 AM
// - Dernière modification le 5/26/2015 9:53:48 AM
//@END-HEADER
using UnityEngine;
using System.Collections;
using System.IO;

namespace dassault
{
    /// <summary>
    /// description for class TestAutoGlassController
    /// </summary>
	public class TestAutoGlassController : GlassController
    {
		private enum State
		{
			CONNEXION,
			LOAD_SCENARIO,
			START_STEP,
			SHOW_LOCALIZATION,
			SHOW_COMMENTS,
			SHOW_TOOLS,
			SHOW_TOOLS_NEXT,
			SHOW_REFERENCES,
			SHOW_ANNOTATION,
			TAKE_SCREENSHOT,
			RECEIVE_ANNOTATION,
			NEXT_STEP
		}

		private void Start()
		{
			m_switchState = false;
			m_currentState = State.CONNEXION;
			StartCoroutine(WaitToSwitchStateCoroutine(5));
		}

		protected override void PreUpdate()
		{
			if(m_switchState)
			{
				SwitchState();
				m_switchState = false;
			}
		}

		private void SwitchState()
		{
			switch(m_currentState)
			{
			case State.CONNEXION:
				Debug.Log("Change state CONNEXION => LOAD_SCENARIO");
				m_currentState = State.LOAD_SCENARIO;
				m_callbacks.CallSetWatchConnexionStatus(true);
				m_callbacks.CallSetPadConnexionStatus(true);
				StartCoroutine(WaitToSwitchStateCoroutine(1));
				break;
			case State.LOAD_SCENARIO:
				Debug.Log("Change state LOAD_SCENARIO => START_STEP");
				m_currentState = State.START_STEP;
				m_callbacks.CallOnLoadProject("scenario1");
				StartCoroutine(WaitToSwitchStateCoroutine(2));
				break;
			case State.START_STEP:
				Debug.Log("Change state START_STEP => SHOW_LOCALIZATION");
				m_currentState = State.SHOW_LOCALIZATION;
				m_callbacks.CallOnSwitchLocalization();
				StartCoroutine(WaitToSwitchStateCoroutine(6));
				break;
			case State.SHOW_LOCALIZATION:
				if(m_callbacks.IsCommentsShown())
				{
					Debug.Log("Change state SHOW_LOCALIZATION => SHOW_COMMENTS");
					m_currentState = State.SHOW_COMMENTS;
					m_callbacks.CallOnSwitchComments();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else if(m_callbacks.IsToolsShown())
				{
					Debug.Log("Change state SHOW_LOCALIZATION => SHOW_TOOLS");
					m_currentState = State.SHOW_TOOLS;
					m_callbacks.CallOnSwitchTools();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else if(m_callbacks.IsReferencesShown())
				{
					Debug.Log("Change state SHOW_LOCALIZATION => SHOW_REFERENCES");
					m_currentState = State.SHOW_REFERENCES;
					m_callbacks.CallOnSwitchReferences();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else
				{
					Debug.Log("Change state SHOW_LOCALIZATION => TAKE_SCREENSHOT");
					m_currentState = State.TAKE_SCREENSHOT;
					m_callbacks.CallOnSwitchLocalization();
					m_callbacks.CallOnTakeScreenshot();
					StartCoroutine(WaitToSwitchStateCoroutine(1.0f));
				}
				break;
			case State.SHOW_COMMENTS:
				if(m_callbacks.HasNextComment())
				{
					m_callbacks.CallOnCurrentViewNext();
					StartCoroutine(WaitToSwitchStateCoroutine(2.0f));
				}
				else if(m_callbacks.IsToolsShown())
				{
					Debug.Log("Change state SHOW_COMMENTS => SHOW_TOOLS");
					m_currentState = State.SHOW_TOOLS;
					m_callbacks.CallOnSwitchTools();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else if(m_callbacks.IsReferencesShown())
				{
					Debug.Log("Change state SHOW_COMMENTS => SHOW_REFERENCES");
					m_currentState = State.SHOW_REFERENCES;
					m_callbacks.CallOnSwitchReferences();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else
				{
					Debug.Log("Change state SHOW_COMMENTS => TAKE_SCREENSHOT");
					m_currentState = State.TAKE_SCREENSHOT;
					m_callbacks.CallOnSwitchComments();
					m_callbacks.CallOnTakeScreenshot();
					StartCoroutine(WaitToSwitchStateCoroutine(1.0f));
				}
				break;
			case State.SHOW_TOOLS:
				if(m_callbacks.HasNextTool())
				{
					m_callbacks.CallOnCurrentViewNext();
					StartCoroutine(WaitToSwitchStateCoroutine(2.0f));
				}
				else if(m_callbacks.IsReferencesShown())
				{
					Debug.Log("Change state SHOW_TOOLS => SHOW_REFERENCES");
					m_currentState = State.SHOW_REFERENCES;
					m_callbacks.CallOnSwitchReferences();
					StartCoroutine(WaitToSwitchStateCoroutine(3));
				}
				else
				{
					Debug.Log("Change state SHOW_TOOLS => TAKE_SCREENSHOT");
					m_currentState = State.TAKE_SCREENSHOT;
					m_callbacks.CallOnSwitchTools();
					m_callbacks.CallOnTakeScreenshot();
					StartCoroutine(WaitToSwitchStateCoroutine(1.0f));
				}
				break;
			case State.SHOW_REFERENCES:
				if(m_callbacks.HasNextReference())
				{
					m_callbacks.CallOnCurrentViewNext();
					StartCoroutine(WaitToSwitchStateCoroutine(2.0f));
				}
				else
				{
					Debug.Log("Change state SHOW_REFERENCES => TAKE_SCREENSHOT");
					m_currentState = State.TAKE_SCREENSHOT;
					m_callbacks.CallOnSwitchReferences();
					m_callbacks.CallOnTakeScreenshot();
					StartCoroutine(WaitToSwitchStateCoroutine(1.0f));
				}
				break;
			case State.TAKE_SCREENSHOT:
				Debug.Log("Change state TAKE_SCREENSHOT => RECEIVE_ANNOTATION");
				m_currentState = State.RECEIVE_ANNOTATION;
				byte[] image = SimulateImageReceptionFromServer();
				if(image != null)
					m_callbacks.CallOnAnnotationReceived(m_currentStepPath, image);
				StartCoroutine(WaitToSwitchStateCoroutine(3.0f));
				break;
			case State.RECEIVE_ANNOTATION:
				Debug.Log("Change state RECEIVE_ANNOTATION => SHOW_ANNOTATION");
				m_currentState = State.SHOW_ANNOTATION;
				m_callbacks.CallOnOpenReceivedAnnotation(false);
				m_callbacks.CallOnSwitchAnnotation();
				StartCoroutine(WaitToSwitchStateCoroutine(3.0f));
				break;
			case State.SHOW_ANNOTATION:
				Debug.Log("Change state SHOW_ANNOTATION => NEXT_STEP");
				m_currentState = State.NEXT_STEP;
				m_callbacks.CallOnNextStep();
				StartCoroutine(WaitToSwitchStateCoroutine(3.0f));
				break;
			case State.NEXT_STEP:
				Debug.Log("Change state NEXT_STEP => START_STEP");
				m_currentState = State.START_STEP;
				StartCoroutine(WaitToSwitchStateCoroutine(3.0f));
				break;
			}
		}

		private IEnumerator WaitToSwitchStateCoroutine(float duration)
		{
			yield return new WaitForSeconds(duration);
			m_switchState = true;
		}

		private byte[] SimulateImageReceptionFromServer()
		{
#if UNITY_ANDROID
			string file = Application.persistentDataPath + "/test_auto_annotation.png";
#else
			string file = "c:\\temp\\test_auto_annotation.png";
#endif
			if(File.Exists(file))
			{
				byte[] fileContent = File.ReadAllBytes(file);
				return fileContent;
			}
			return null;
		}

		public override void OnAnnotationRequest(string stepPath, byte[] image)
		{
#if UNITY_ANDROID
			string file = Application.persistentDataPath + "/test_auto_annotation.png";
#else
			string file = "c:\\temp\\test_auto_annotation.png";
#endif
			File.WriteAllBytes(file, image);
		}
		
		public override void OnCurrentStepChanged(string stepPath)
		{
			m_currentStepPath = stepPath;
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

		private State m_currentState = State.START_STEP;
		private string m_currentStepPath;
		private bool m_switchState = false;
	}
}
