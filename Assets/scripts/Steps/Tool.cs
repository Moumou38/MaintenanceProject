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
// - Fichier créé le 4/28/2015 11:18:11 AM
// - Dernière modification le 4/28/2015 11:18:11 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class Tool
    /// </summary>
    public class Tool
    {
		public Tool(string name, string image)
		{
			m_name = name;
			m_image = image;
		}
		
		public string Name{get{return m_name;}}
		public string Image{get{return m_image;}}
		
		private string m_name;
		private string m_image;
	}
}
