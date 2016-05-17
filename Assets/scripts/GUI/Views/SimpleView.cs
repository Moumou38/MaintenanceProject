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
// - Fichier créé le 4/23/2015 5:43:27 PM
// - Dernière modification le 4/23/2015 5:43:27 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class SimpleView
    /// </summary>
    public class SimpleView : MonoBehaviour
    {

		public void Show(bool show)
		{
			if(m_animator == null)
			{
				VirtualOnTransitionStart(IsShown());
				gameObject.SetActive(show);
				if(OnTransitionFinishedCallback != null)
					OnTransitionFinishedCallback(show);
				VirtualOnTransitionEnd(IsShown());
			}
			else
			{
				bool isShown = IsShown();
				if(show != isShown)
				{
					if(isShown)
					{
						// on va fermer la vue
						OnViewOpenedCallback = null;
						OnViewClosedCallback = LaunchTransitionFinishedCallback;
					}
					else
					{
						// on va ouvrir la vue
						OnViewOpenedCallback = LaunchTransitionFinishedCallback;
						OnViewClosedCallback = null;
					}
					gameObject.SetActive(true);
					VirtualOnTransitionStart(isShown);
					m_animator.SetBool("IsOpen", show);
                }
            }
		}

		public bool IsShown()
		{
			if(m_animator == null)
			{
				return gameObject.activeSelf;
			}
			else
			{
				return m_animator.GetBool("IsOpen");
			}
		}

		public bool IsInTransition()
		{
			if(m_animator == null)
				return false;
			else
			{
				float time = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				bool result = m_animator.IsInTransition(0) || (time > 0 && time < 1);
				/*if(result)
					Debug.Log ("####### is in transition");*/
				return result;
			}
		}

		protected virtual void VirtualOnTransitionStart(bool visible)
		{
			// a surcharger dans les classes filles
		}

		protected virtual void VirtualOnTransitionEnd(bool visible)
		{
			// a surcharger dans les classes filles
		}

		private void LaunchTransitionFinishedCallback()
		{
			bool shown = IsShown();
			if(OnTransitionFinishedCallback != null)
			{
				OnTransitionFinishedCallback(shown);
				OnTransitionFinishedCallback = null;
			}
			VirtualOnTransitionEnd(shown);
			/*if(!shown)
			{
				gameObject.SetActive(false);
			}*/
		}
		
		public void OnViewOpenedEvent()
		{
			if(OnViewOpenedCallback != null)
			{
				OnViewOpenedCallback();
				OnViewOpenedCallback = null;
			}
		}

		public void OnViewClosedEvent()
		{
			if(OnViewClosedCallback != null)
			{
				OnViewClosedCallback();
				OnViewClosedCallback = null;
			}
		}

		public RectTransform Content
		{
			get{return m_content;}
		}

		private bool m_wasOpen = false;
		[SerializeField] private RectTransform m_content;
		[SerializeField] private Animator m_animator;

		public delegate void OnAnimationEvent();
		private event OnAnimationEvent OnViewOpenedCallback;
		private event OnAnimationEvent OnViewClosedCallback;
		public delegate void OnTransitionFinishedEvent(bool show);
		public event OnTransitionFinishedEvent OnTransitionFinishedCallback;
	}
}
