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
// - Fichier créé le 4/23/2015 5:44:05 PM
// - Dernière modification le 4/23/2015 5:44:05 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class TextView
    /// </summary>
    public class TextView : ViewWithProgressBar
    {
		public void SetText(string comment, string type)
		{
			m_label.text = comment;
			m_noteImage.SetActive (type == "note");
			m_cautionImage.SetActive (type == "caution");
			m_warningImage.SetActive (type == "warning");
			if(type == "note")
				m_label.color = m_noteTextColor;
			else if(type == "caution")
				m_label.color = m_cautionTextColor;
			else
				m_label.color = m_warningTextColor;
		}

		[SerializeField] private Text m_label;
		[SerializeField] private GameObject m_noteImage;
		[SerializeField] private GameObject m_cautionImage;
		[SerializeField] private GameObject m_warningImage;
		[SerializeField] private Color m_noteTextColor;
		[SerializeField] private Color m_cautionTextColor;
		[SerializeField] private Color m_warningTextColor;
	}
}
