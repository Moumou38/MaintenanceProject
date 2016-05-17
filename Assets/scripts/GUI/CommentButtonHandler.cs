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
// - Fichier créé le 4/21/2015 9:39:51 AM
// - Dernière modification le 4/21/2015 9:39:51 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class CommentButtonHandler
    /// </summary>
    public class CommentButtonHandler : MonoBehaviour
    {
		public void Show(bool show)
		{
			gameObject.SetActive(show);
		}

		public void SetCommentType(string type)
		{
			m_warningImage.SetActive(type == "warning");
			m_cautionImage.SetActive(type == "caution");
			m_noteImage.SetActive(type == "note");
		}

		[SerializeField] private GameObject m_warningImage;
		[SerializeField] private GameObject m_cautionImage;
		[SerializeField] private GameObject m_noteImage;
	}
}
