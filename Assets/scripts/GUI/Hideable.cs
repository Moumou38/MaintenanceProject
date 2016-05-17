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
// - Fichier créé le 6/5/2015 11:18:53 AM
// - Dernière modification le 6/5/2015 11:18:53 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class Hideable
    /// </summary>
    public class Hideable : MonoBehaviour
    {
        public void Hide()
		{
			gameObject.SetActive(false);
		}
    }
}
