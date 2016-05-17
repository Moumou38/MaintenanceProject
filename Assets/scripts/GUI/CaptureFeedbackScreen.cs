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
// - Fichier créé le 6/5/2015 10:52:56 AM
// - Dernière modification le 6/5/2015 10:52:56 AM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class CaptureFeedbackScreen
    /// </summary>
    public class CaptureFeedbackScreen : MonoBehaviour
    {
		public void SetCameraTexture(Texture texture)
		{
			m_cameraImage.texture = texture;
		}
		
		public void Show(bool show)
		{
			gameObject.SetActive(show);
		}
		
		public void SetTimer(int time)
		{
			m_timerText.text = time.ToString();
		}

		public void StartFlash()
		{
			m_flash.SetActive(true);
		}
		
		[SerializeField] private Text m_timerText;
		[SerializeField] private RawImage m_cameraImage;
		[SerializeField] private GameObject m_flash;
	}
}
