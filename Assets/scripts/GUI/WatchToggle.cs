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
// - Fichier créé le 6/1/2015 5:35:46 PM
// - Dernière modification le 6/1/2015 5:35:46 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class WatchToggle
    /// </summary>
    public class WatchToggle : MonoBehaviour
    {
        public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}
