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
// - Fichier créé le 5/29/2015 12:07:33 PM
// - Dernière modification le 5/29/2015 12:07:33 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class HideScrollBar
    /// </summary>
    public class HideScrollBar : MonoBehaviour
    {
		private void OnRectTransformDimensionsChange()
		{
			m_scrollBar.SetActive(m_container.rect.height < m_content.rect.height);
		}
		[SerializeField] private RectTransform m_content;
		[SerializeField] private RectTransform m_container;
		[SerializeField] private GameObject m_scrollBar;
	}
}
