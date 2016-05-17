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
// - Fichier créé le 4/20/2015 5:52:53 PM
// - Dernière modification le 4/20/2015 5:52:53 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class RotateAround
    /// </summary>
    public class RotateAround : MonoBehaviour
    {
        // Update is called once per frame
        void Update ()
        {
			float angle = m_rotationSpeed * Time.deltaTime;
			transform.RotateAround(m_target.transform.position, m_axis, angle);
        }

		private float m_distance;
		[SerializeField] private GameObject m_target;
		[SerializeField] private Vector3 m_axis = new Vector3(0, 1, 0);
		[SerializeField] private float m_rotationSpeed = 30;
	}
}
