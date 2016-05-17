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
// - Fichier créé le 4/20/2015 10:20:27 AM
// - Dernière modification le 4/20/2015 10:20:27 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class GUIModule
    /// </summary>
    public class GUIModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			m_stepScreen.gameObject.SetActive(true);
            m_buttonContainer.SetActive(false);
            m_extractingDataMessage.SetActive(false);
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			return result;
		}

		private new void Awake()
		{
			base.Awake();
			m_stepScreen.Show(false);
			m_stepScreen.gameObject.SetActive(false);
			m_captureFeedbackScreen.Show(false);
            
		}

		public StepScreen StepScreen {get{return m_stepScreen;}}
        public GameObject ButtonContainer { get { return m_buttonContainer; } }
        public GameObject ExtractingData { get { return m_extractingDataMessage; } }
		public CaptureFeedbackScreen CaptureFeedbackScreen {get{return m_captureFeedbackScreen;}}

        [SerializeField] private GameObject m_buttonContainer;
        [SerializeField] private GameObject m_extractingDataMessage; 
		[SerializeField] private StepScreen m_stepScreen;
		[SerializeField] private CaptureFeedbackScreen m_captureFeedbackScreen;
	}
}
