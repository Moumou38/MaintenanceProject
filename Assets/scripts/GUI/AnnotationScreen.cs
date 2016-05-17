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
// - Fichier créé le 4/21/2015 1:35:20 PM
// - Dernière modification le 4/21/2015 1:35:20 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class AnnotationScreen
    /// </summary>
    public class AnnotationScreen : MonoBehaviour
    {
		public void Show(bool show)
		{
            if(show)
            {
                StartCoroutine(ResetValues());
            }
			gameObject.SetActive(show);
		}

		private IEnumerator ResetValues()
		{
			yield return new WaitForEndOfFrame();
			m_redColorOpaqueToggle.isOn = true;
			m_smallLineToggle.isOn = true;
			if(OnColorChangedCallback != null)
				OnColorChangedCallback(Color.red);
			if(OnLineSizeChangedCallback != null)
				OnLineSizeChangedCallback(m_smallLineValue);
		}

		public void OnOpaqueColorToggleChanged(bool active)
		{
			if(active && OnColorChangedCallback != null)
			{
				Color color = Color.white;
				if(m_redColorOpaqueToggle.isOn)
					color = Color.red;
				if(m_greenColorOpaqueToggle.isOn)
					color = Color.green;
				if(m_blueColorOpaqueToggle.isOn)
					color = Color.blue;
				if(m_yellowColorOpaqueToggle.isOn)
					color = Color.yellow;
				if(m_blackColorOpaqueToggle.isOn)
					color = Color.black;
				OnColorChangedCallback(color);
			}
		}
		
		public void OnTransparentColorToggleChanged(bool active)
		{
			if(active && OnColorChangedCallback != null)
			{
				Color color = Color.white;
				if(m_redColorTransparentToggle.isOn)
					color = Color.red;
				if(m_greenColorTransparentToggle.isOn)
					color = Color.green;
				if(m_blueColorTransparentToggle.isOn)
					color = Color.blue;
				if(m_yellowColorTransparentToggle.isOn)
					color = Color.yellow;
				if(m_blackColorTransparentToggle.isOn)
					color = Color.black;
				color.a = 0.5f;
				OnColorChangedCallback(color);
			}
		}
		
		public void OnSizeChanged(bool active)
		{
			if(active && OnLineSizeChangedCallback != null)
			{
				if(m_largeLineToggle.isOn)
					OnLineSizeChangedCallback(m_largeLineValue);
				if(m_smallLineToggle.isOn)
					OnLineSizeChangedCallback(m_smallLineValue);
			}
		}
		
		public void OnValidate()
		{
			if(OnValidateCallback != null)
			{
				OnValidateCallback();
			}
		}
		
		public void OnCancel()
		{
			if(OnCancelCallback != null)
			{
				OnCancelCallback();
			}
		}
		
		public Rect GetViewport()
		{
			return GUIHelper.GetRectInViewportSpace(m_cameraViewport, m_rootCanvas);
		}

		
		[SerializeField] private Toggle m_redColorOpaqueToggle;
		[SerializeField] private Toggle m_greenColorOpaqueToggle;
		[SerializeField] private Toggle m_blueColorOpaqueToggle;
		[SerializeField] private Toggle m_yellowColorOpaqueToggle;
		[SerializeField] private Toggle m_whiteColorOpaqueToggle;
		[SerializeField] private Toggle m_blackColorOpaqueToggle;
		[SerializeField] private Toggle m_redColorTransparentToggle;
		[SerializeField] private Toggle m_greenColorTransparentToggle;
		[SerializeField] private Toggle m_blueColorTransparentToggle;
		[SerializeField] private Toggle m_yellowColorTransparentToggle;
		[SerializeField] private Toggle m_whiteColorTransparentToggle;
		[SerializeField] private Toggle m_blackColorTransparentToggle;
		[SerializeField] private Toggle m_largeLineToggle;
		[SerializeField] private Toggle m_smallLineToggle;
		[SerializeField] private float m_largeLineValue = 0.01f;
		[SerializeField] private float m_smallLineValue = 0.05f;
		[SerializeField] private RectTransform m_cameraViewport;
		[SerializeField] private Canvas m_rootCanvas;

		public delegate void OnColorChangedEvent(Color color);
		public event OnColorChangedEvent OnColorChangedCallback;
		public delegate void OnLineSizeChangedEvent(float lineSize);
		public event OnLineSizeChangedEvent OnLineSizeChangedCallback;
		public delegate void OnTransparencyChangedEvent(bool transparent);
		public event OnTransparencyChangedEvent OnTransparencyChangedCallback;
		public delegate void OnButtonEvent();
		public event OnButtonEvent OnValidateCallback;
		public event OnButtonEvent OnCancelCallback;

	}
}
