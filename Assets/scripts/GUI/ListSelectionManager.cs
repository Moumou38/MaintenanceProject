// Copyright 2015 Dassault
//
// - Description
//	 -> 	public class ListSelectionManager : MonoBehaviour : TODO

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
//		\date   Fri Feb 20 13:34:57 2015 +0100
//@END-HEADER



using UnityEngine;
using System.Collections;

namespace dassault
{
	public class ListSelectionManager : MonoBehaviour
	{
		public delegate void OnSelectedItemChangedDelegate(ListItem newSelectedItem);
		public event OnSelectedItemChangedDelegate OnSelectedItemChangedCallback;
		public delegate void OnItemDoubleClickedDelegate(ListItem newSelectedItem);
		public event OnItemDoubleClickedDelegate OnItemDoubleClickedCallback;

		public void RegisterItemForSelection(ListItem item)
		{
			// on supprime les callback avant de les ajouter afin d'eviter les problèmes d'ajout doubles
			item.OnSelectedCallback -= OnItemSelectedCallback;
			item.OnSelectedCallback += OnItemSelectedCallback;
			item.OnDoubleClickedCallback -= OnItemDoubleClicked;
			item.OnDoubleClickedCallback += OnItemDoubleClicked;
		}

		public ListItem SelectedItem
		{
			set
			{
				OnItemSelectedCallback(value);
				if(m_selectedItem != null)
				{
					m_selectedItem.SetSelected(true);
				}
			}
			get{return m_selectedItem;}
		}

		private void OnItemSelectedCallback(ListItem item)
		{
			if(item != m_selectedItem)
			{
				if(m_selectedItem != null)
				{
					m_selectedItem.SetSelected(false);
				}
				m_selectedItem = item;
				if(OnSelectedItemChangedCallback != null)
					OnSelectedItemChangedCallback(m_selectedItem);
				// pas besoin de faire SetSelected(true), c'est déjà fait par l'item avant d'appeler la callback
			}
		}

		private void OnItemDoubleClicked(ListItem item)
		{
			if(OnItemDoubleClickedCallback != null)
			{
				OnItemDoubleClickedCallback(item);
			}
		}

		private ListItem m_selectedItem = null;
	}
}
