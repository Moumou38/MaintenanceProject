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
// - Fichier créé le 5/29/2015 12:20:09 PM
// - Dernière modification le 5/29/2015 12:20:09 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class ConnexionScreen
    /// </summary>
    public class ConnectionScreen : MonoBehaviour
    {

		public void Show(bool show)
		{
			gameObject.SetActive(show);
		}

		private void Awake()
		{
			m_list.SelectionManager.OnSelectedItemChangedCallback += OnItemSelected;
			m_list.SelectionManager.OnItemDoubleClickedCallback += OnItemDoubleClicked;
        }
        
        public void AddSelectableDevice(string name, string address)
		{
			ListItemText item = m_list.AddItem(m_listItemPrefab) as ListItemText;
			item.Value = name;
			item.gameObject.AddComponent<SelectableDevice>().Init(name, address);
		}

		private void OnItemSelected(ListItem item)
		{
			m_connectButton.SetActive(item != null);
		}

		private void OnItemDoubleClicked(ListItem item)
		{
			if(item != null)
			{
				ListItemText typedItem = item as ListItemText;
				SelectableDevice device = typedItem.gameObject.GetComponent<SelectableDevice>();
				if(OnDeviceSelectedCallback != null)
				{
					OnDeviceSelectedCallback(device.Name, device.Address);
					m_list.SelectionManager.SelectedItem = null;
				}
			}
		}

		public void OnConnectClick()
		{
			OnItemDoubleClicked(m_list.SelectionManager.SelectedItem);
		}

		public void ShowList(bool show)
		{
			m_listRoot.gameObject.SetActive(show);
		}

		public void SetFeedbackMessage(string message)
		{
			if(string.IsNullOrEmpty(message))
			{
				m_connectionFeedback.SetActive(false);
			}
			else
			{
				m_connectionFeedback.SetActive(true);
				m_feedbackText.text = message;
			}
		}

		public void SetErrorMessage(string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				m_errorMessage.gameObject.SetActive(false);
			}
			else
			{
				m_errorMessage.gameObject.SetActive(true);
				m_errorMessage.text = text;
			}
		}

		public void QuitApplication()
		{
			Debug.Log ("Fin de l'application");
			Application.Quit();
		}

		[SerializeField] GameObject m_listItemPrefab;
		[SerializeField] List m_list;
		[SerializeField] GameObject m_listRoot;
		[SerializeField] GameObject m_connectButton;
		[SerializeField] GameObject m_connectionFeedback;
		[SerializeField] Text m_feedbackText;
		[SerializeField] Text m_errorMessage;

		public delegate void OnDeviceSelectedDelegate(string name, string address);
		public event OnDeviceSelectedDelegate OnDeviceSelectedCallback;
    }
}
