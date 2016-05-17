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
// - Fichier créé le 4/20/2015 7:10:52 PM
// - Dernière modification le 4/20/2015 7:10:52 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class Reference
    /// </summary>
    public class Reference
    {
		public Reference(string name, string title, string type, string link)
		{
			m_name = name;
			m_title = title;
			m_type = type;
			m_link = link;
		}

		public string Name{get{return m_name;}}
		public string Title{get{return m_title;}}
		public string Type{get{return m_type;}}
		public string Link{get{return m_link;}}

		private string m_name;
		private string m_title;
		private string m_type;
		private string m_link;
    }
}
