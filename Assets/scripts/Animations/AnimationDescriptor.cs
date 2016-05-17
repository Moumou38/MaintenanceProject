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
// - Fichier créé le 4/24/2015 1:35:33 PM
// - Dernière modification le 4/24/2015 1:35:33 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class AnimationDescriptor
    /// </summary>
    public class AnimationDescriptor : MonoBehaviour
    {
		public void Show(bool show)
		{
			gameObject.SetActive(show);
		}

		public void PlayAnimation(string animationName, Camera camera)
		{
			if(m_animation != null)
			{
				if(m_previousCoroutine != null)
				{
					StopCoroutine(m_previousCoroutine);
					m_animation.Stop();
				}
				m_previousCoroutine = StartCoroutine(WaitToPlayAnimation(animationName, camera));
			}
		}

		private IEnumerator WaitToPlayAnimation(string animationName, Camera camera)
		{   
			m_camera = camera;             
			m_animation[animationName].speed = m_animationSpeedFactor;
			m_animation.Play(animationName);
			yield return new WaitForEndOfFrame();
			m_animation.Stop();
			yield return new WaitForSeconds(2);
			m_animation.Play(animationName);
		}

		private void Update()
		{
			if(m_camera != null)
			{
				m_camera.transform.position = m_cameraPosition.position;
				m_camera.transform.rotation = m_cameraPosition.rotation;
			}
		}

		public string Category
		{
			get{return gameObject.name;}
		}

        public void setCameraPosition(Transform iPosition)
        {
            m_cameraPosition = iPosition; 
        }

        public void setAnimation(Animation iAnimation)
        {
            m_animation = iAnimation;
        }

		private Camera m_camera;
		private Coroutine m_previousCoroutine;
		[SerializeField] private Transform m_cameraPosition;
		[SerializeField] private Animation m_animation;
		[SerializeField] private float m_animationSpeedFactor = 1.0f;
    }
}
