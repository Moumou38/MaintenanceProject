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
// - Fichier créé le 4/28/2015 5:15:35 PM
// - Dernière modification le 4/28/2015 5:15:35 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class ScenarioSelector
    /// </summary>
    public class ScenarioSelector : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
#if UNITY_ANDROID
			string scenarioFolder = "/mnt/sdcard/DispositifMainLibre/Scenarii/";
#else
			string scenarioFolder = Application.dataPath + "/../Datas/scenarii/";
#endif
			string[] scenarios = Directory.GetDirectories(scenarioFolder);
			foreach(string scenario in scenarios)
			{
				ListItemText item = m_list.AddItem(m_listItemPrefab) as ListItemText;
				item.Value = Path.GetFileName(scenario);
			}
		}
    
        public void OnValidated()
		{
			if(m_selectionManager.SelectedItem != null)
			{
				if(OnScenarioLoadedCallback != null)
				{
					ListItemText typedItem = m_selectionManager.SelectedItem as ListItemText;
					OnScenarioLoadedCallback(typedItem.Value);
					m_title.text = typedItem.Value;
				}
			}
		}

		[SerializeField] private Text m_title;
		[SerializeField] private List m_list;
		[SerializeField] private GameObject m_listItemPrefab;
		[SerializeField] private ListSelectionManager m_selectionManager;

		public delegate void OnScenarioLoadedDelegate(string scenarioName);
		public event OnScenarioLoadedDelegate OnScenarioLoadedCallback;
    }
}
