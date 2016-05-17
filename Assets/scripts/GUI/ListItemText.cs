// Copyright 2015 Dassault
//
// - Description
//	 -> 	public class ListItemText : ListItem : TODO

//
// - Namespace(s)
//	namespace dassault

//
// - Auteurs
//	\author Michel de Verdelhan <mdeverdelhan@theoris.fr>

//
// - Fichier créé le 
//		\date   Thu Feb 12 14:20:32 2015 +0100
// - Dernière modification le 
//		\date   Thu Feb 12 14:20:32 2015 +0100
//@END-HEADER



using UnityEngine;
using System.Collections;

namespace dassault
{
	public class ListItemText : ListItem
	{
		public override void OnSelected(bool selected)
		{
			if(selected)
				m_text.color = m_selectedForegroundColor;
			else
				m_text.color = m_normalForegroundColor;
		}

		public string Value
		{
			get{return m_text.text;}
			set{m_text.text = value;}
		}

		[SerializeField] private UnityEngine.UI.Text m_text;
		[SerializeField] private Color m_normalForegroundColor;
		[SerializeField] private Color m_selectedForegroundColor;
	}
}
