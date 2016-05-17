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
// - Fichier créé le 4/20/2015 12:32:12 PM
// - Dernière modification le 4/20/2015 12:32:12 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class RotateObject
    /// </summary>
    public class RotateObject : MonoBehaviour
    {
        // Update is called once per frame
        void Update ()
        {
			float angle = m_rotationSpeed * Time.deltaTime;
			transform.Rotate(m_rotationAxis, angle);
        }

		[SerializeField] private Vector3 m_rotationAxis = new Vector3(0, 1, 0);
		[SerializeField] private float m_rotationSpeed;
    }
}
