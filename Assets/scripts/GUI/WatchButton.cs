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
// - Fichier créé le 6/1/2015 5:21:18 PM
// - Dernière modification le 6/1/2015 5:21:18 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class Hideable
    /// </summary>
    public class WatchButton : MonoBehaviour
    {
		public void SimulateClick()
		{
			gameObject.SetActive(true);
		}

        public void Hide()
		{
			gameObject.SetActive (false);
		}
    }
}
