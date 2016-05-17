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
// - Fichier créé le 4/20/2015 11:28:37 AM
// - Dernière modification le 4/20/2015 11:28:37 AM
//@END-HEADER
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class StepView
    /// </summary>
    public class StepView : MonoBehaviour
    {
		public void SetOverview(int currentStep, int stepNumber, string message)
		{
			for(int i = 0; i < m_circleContainer.childCount; ++i)
			{
				Transform son = m_circleContainer.GetChild(i);
				son.SetParent(null);
				GameObject.Destroy(son.gameObject);
			}
			for(int i = 0; i < stepNumber; ++i)
			{
				GameObject newImage = null;
				if(currentStep == i)
				{
					newImage = GameObject.Instantiate(m_currentStepImagePrefab) as GameObject;
				}
				else
				{
					newImage = GameObject.Instantiate(m_otherStepImagePrefab) as GameObject;
				}
				newImage.transform.SetParent(m_circleContainer);
			}
			if(m_title != null)
				m_title.text = message;
		}

		public void SetContent(string content)
		{
			m_content.text = content;
			m_leftButton.gameObject.SetActive(true);
			m_rightButton.gameObject.SetActive(true);
		}

		public void OnLeftButtonClick()
		{
			if(OnLeftButtonClickCallback != null)
				OnLeftButtonClickCallback();
		}

		public void OnRightButtonClick()
		{
			if(OnRightButtonClickCallback != null)
				OnRightButtonClickCallback();
		}

		public delegate void ButtonClickDelegate();
		public event ButtonClickDelegate OnLeftButtonClickCallback;
		public event ButtonClickDelegate OnRightButtonClickCallback;
		[SerializeField] private Text m_content;
		[SerializeField] private RectTransform m_circleContainer;
		[SerializeField] private Text m_title;
		[SerializeField] private GameObject m_currentStepImagePrefab;
		[SerializeField] private GameObject m_otherStepImagePrefab;
		[SerializeField] private GameObject m_messagePrefab;
		[SerializeField] private GameObject m_leftButton;
		[SerializeField] private GameObject m_rightButton;
	}
}
