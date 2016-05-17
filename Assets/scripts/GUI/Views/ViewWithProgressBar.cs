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
// - Fichier créé le 4/23/2015 5:43:52 PM
// - Dernière modification le 4/23/2015 5:43:52 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class ViewWithProgressBar
    /// </summary>
    public class ViewWithProgressBar : SimpleView
    {
		public void SetProgress(int currentSteIndex, int stepCount)
		{
			m_progressBar.Reset(currentSteIndex, stepCount);
			m_buttons.SetActive(stepCount > 1);
		}
		
		public void SetTitle(string title)
		{
			if(m_title != null)
				m_title.text = title;
		}
		
		[SerializeField] private GameObject m_buttons;
		[SerializeField] private SpotProgressWidget m_progressBar;
		[SerializeField] private Text m_title;
	}
}
