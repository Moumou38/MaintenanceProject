// Copyright 2015 Dassault
//
// - Description
//   -- Description synthétique du contenu du fichier
//
// - Namespace(s)
//   -- Préciser le ou les namespaces utilisés dans ce fichier
//
// - Auteurs
//   -- de Verdelhan(Theoris)
//
// - Fichier créé le XXX
// - Dernière modification le XXX
//
using UnityEngine;
using System.Collections;

namespace dassault
{
	/// <summary>
	/// classe servant à décrire un module de l'application
	/// </summary>
	public class ModuleDescriptor : MonoBehaviour
	{

		public void Load()
		{
			StartCoroutine(LoadModule());
		}

		private IEnumerator LoadModule()
		{
			m_progress = 0.0f;
			AsyncOperation operation = Application.LoadLevelAsync(m_sceneToLoad);
			while(!operation.isDone)
			{
				m_progress = operation.progress;
				yield return new WaitForEndOfFrame();
			}
			m_loaded = true;
		}

		/// <summary>
		/// return instance created during loading
		/// </summary>
		/// <returns>The instance.</returns>
		public ModuleInstance GetCreatedInstance()
		{
			GameObject go = GameObject.Find(m_moduleInstanceName);
			if(go != null)
			{
				ModuleInstance result = go.GetComponentsInChildren<ModuleInstance>(true)[0];
				if(result == null)
				{
					Debug.LogError("unable to find ModuleInstance component for module " + name);
				}
				return result;
			}
			else
			{
				Debug.LogError("unable to find module instance game object " + m_moduleInstanceName + " for module " + name);
				return null;
			}
		}

		public bool Loaded
		{
			get{ return m_loaded;}
		}

		public float Progress
		{
			get{return m_progress;}
		}

		public string Message
		{
			get{return m_message;}
		}

		public bool LastModuleToLoad
		{
			get{return m_lastModuleToLoad;} 
		}

		public string ModuleName
		{
			get{return m_moduleInstanceName;}
		}

		private bool m_loaded = false;
		private float m_progress = 0.0f;
		[SerializeField] private string m_sceneToLoad;
		[SerializeField] private string m_moduleInstanceName;
		[SerializeField] private string m_message;
		[SerializeField] private bool m_lastModuleToLoad = false;

	}
}
