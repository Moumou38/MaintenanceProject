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
// - Fichier créé le 4/20/2015 10:37:02 AM
// - Dernière modification le 4/20/2015 10:37:02 AM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class LoadingScreen
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
			Object.DontDestroyOnLoad(gameObject);
			m_loader.LoadModulesAsync();
        }
    
        // Update is called once per frame
        void Update ()
        {
    		if(m_loader.Progress < 1)
			{
				m_loadingMessage.text = m_loader.CurrentMessage;
			}
			else
			{
				GameObject.Destroy(gameObject);
			}
        }

		[SerializeField] private ModuleLoader m_loader;
		[SerializeField] private Text m_loadingMessage;
    }
}
