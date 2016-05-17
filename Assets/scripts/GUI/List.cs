// Copyright 2015 Dassault
//
// - Description
//	 -> 	public class List : MonoBehaviour : TODO

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
//		\date   Fri Feb 20 13:07:55 2015 +0100
//@END-HEADER



using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
	public class List : MonoBehaviour
	{
		void Awake()
		{
			if(m_listSelectionManager == null)
			{
				m_listSelectionManager = gameObject.AddComponent<ListSelectionManager>();
			}
		}

		void Start()
		{
			foreach(RectTransform son in m_content)
			{
				SetupGameObject(son.gameObject);
			}
		}

		private ListItem SetupGameObject(GameObject go)
		{
			ListItem item = go.GetComponent<ListItem>();
			if(item != null)
			{
				m_listSelectionManager.RegisterItemForSelection(item);
			}
			if(go.transform.parent != m_content)
			{
				go.transform.SetParent(m_content, false);
			}
			return item;
		}

		public ListItem AddItem(GameObject prefab)
		{
			GameObject go = GameObject.Instantiate(prefab) as GameObject;
			ListItem result = SetupGameObject(go);
			return result;
		}

		public void Clear()
		{
			foreach(RectTransform son in m_content)
			{
				GameObject.Destroy(son.gameObject);
			}
		}

		public void SetSelectedItem(int index)
		{
			ListItem selectedItem = null;
			if(index >= 0 && index < m_content.childCount)
			{
				GameObject go = m_content.GetChild(index).gameObject;
				selectedItem = go.GetComponent<ListItem>();
			}
			m_listSelectionManager.SelectedItem = selectedItem;
		}

		public ListItem SelectedItem()
		{
			return m_listSelectionManager.SelectedItem;
		}

		public ListSelectionManager SelectionManager
		{
			get{return m_listSelectionManager;}
		}

		[SerializeField] private RectTransform m_content;
		[SerializeField] private ListSelectionManager m_listSelectionManager;
	}
}
