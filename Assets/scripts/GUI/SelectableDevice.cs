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
// - Fichier créé le 5/29/2015 3:45:45 PM
// - Dernière modification le 5/29/2015 3:45:45 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class SelectableDevice
    /// </summary>
    public class SelectableDevice : MonoBehaviour
    {
		public void Init(string name, string address)
		{
			m_name = name;
			m_address = address;
		}

		public string Name
		{
			get{return m_name;}
		}

		public string Address
		{
			get{return m_address;}
		}
		private string m_name;
		private string m_address;
    }
}
