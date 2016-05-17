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
// - Fichier créé le 5/26/2015 10:48:06 AM
// - Dernière modification le 5/26/2015 10:48:06 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class ScaleCircle
    /// </summary>
    public class ScaleCircle : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
			m_baseLocalScale = transform.localScale;
        }
    
        // Update is called once per frame
        void Update ()
        {
			//float lerpFactor = Mathf.Sin(Time.realtimeSinceStartup) * 0.5f + 0.5f;
			float time = Time.realtimeSinceStartup * m_timeFactor;
			float lerpFactor = time - Mathf.Floor(time);
			transform.localScale = m_baseLocalScale * Mathf.Lerp(m_minScale, m_maxScale, lerpFactor);
        }

		private Vector3 m_baseLocalScale;
		[SerializeField] private float m_minScale = 0.7f;
		[SerializeField] private float m_maxScale = 1.0f;
		[SerializeField] private float m_timeFactor = 0.7f;
    }
}
