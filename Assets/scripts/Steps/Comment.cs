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
// - Fichier créé le 4/20/2015 6:21:40 PM
// - Dernière modification le 4/20/2015 6:21:40 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class Comments
    /// </summary>
    public class Comment
    {
		public Comment(string type, string content)
		{
			m_type = type;
			m_content = content;
		}

		public string Type {get{return m_type;}}

		public string Content{get{return m_content;}}

		private string m_type;
		private string m_content;
    }
}
