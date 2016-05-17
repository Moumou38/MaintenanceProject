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
// - Fichier créé le 4/24/2015 12:05:14 PM
// - Dernière modification le 4/24/2015 12:05:14 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

namespace dassault
{
    /// <summary>
    /// description for class WatchScreen
    /// </summary>
    public class WatchScreen : MonoBehaviour
    {
		public void Show(bool show)
		{
			gameObject.SetActive (show);
		}

		public void SetCurrentView(int index)
		{
			for(int i = 0; i < m_views.Length; ++i)
			{
				m_views[i].SetActive(i == index);
			}
		}

		public void SetCurrentStepPath(string stepPath)
		{
			string[] steps = stepPath.Split('>');
			StringBuilder sb = new StringBuilder();
			for(int i = 1; i < steps.Length; ++i)
			{
				sb.Append(steps[i]);
				if(i < steps.Length - 1)
					sb.Append(" > ");
			}
			m_currentStepPath.text = sb.ToString();
		}

		private void ResetToggles()
		{
			m_annotationsToggle.SetActive(false);
			m_referencesToggle.SetActive(false);
			m_commentsToggle.SetActive(false);
			m_localizationToggle.SetActive(false);
			m_guiVisibilityToggle.SetActive(false);
		}

		public void SimulatePreviousStepClick()
		{
			m_previousStepButton.SimulateClick();
			//ResetToggles();
		}

		public void SimulateNextStepClick()
		{
			m_nextStepButton.SimulateClick();
			//ResetToggles();
		}

		public void SimulateHomeClick()
		{
			m_homeButton.SimulateClick();
		}

		public void SimulateBookmark1Click()
		{
			m_bookmark1Button.SimulateClick();
		}

		public void SimulateBookmark2Click()
		{
			m_bookmark2Button.SimulateClick();
		}

		public void SimulateBookmark3Click()
		{
			m_bookmark3Button.SimulateClick();
		}

		public void SimulatePreviousElementClick()
		{
			m_previousElementButton.SimulateClick();
		}
		
		public void SimulateNextElementClick()
		{
			m_nextElementButton.SimulateClick();
		}

		public void SetAnnotationVisibility(bool visible)
		{
			m_annotationsToggle.SetActive(visible);
		}
		
		public void SetReferencesVisibility(bool visible)
		{
			m_referencesToggle.SetActive(visible);
		}
		
		public void SetCommentsVisibility(bool visible)
		{
			m_commentsToggle.SetActive(visible);
		}
		
		public void SetLocalizationVisibility(bool visible)
		{
			m_localizationToggle.SetActive(visible);
		}
		
		public void SetGUIVisibility(bool visible)
		{
			m_guiVisibilityToggle.SetActive(visible);
		}
		
		[SerializeField] private GameObject[] m_views;
		[SerializeField] private Text m_currentStepPath;
		[SerializeField] private WatchButton m_previousStepButton;
		[SerializeField] private WatchButton m_nextStepButton;
		[SerializeField] private WatchButton m_homeButton;
		[SerializeField] private WatchButton m_bookmark1Button;
		[SerializeField] private WatchButton m_bookmark2Button;
		[SerializeField] private WatchButton m_bookmark3Button;
		[SerializeField] private WatchButton m_previousElementButton;
		[SerializeField] private WatchButton m_nextElementButton;
		[SerializeField] private WatchButton m_captureButton;
		[SerializeField] private WatchToggle m_annotationsToggle;
		[SerializeField] private WatchToggle m_referencesToggle;
		[SerializeField] private WatchToggle m_commentsToggle;
		[SerializeField] private WatchToggle m_localizationToggle;
		[SerializeField] private WatchToggle m_guiVisibilityToggle;
    }
}
