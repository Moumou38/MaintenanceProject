// Copyright 2015 dassault
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
using System.Collections.Generic;

namespace dassault
{
	/// <summary>
	/// classe gerant le chargement des modules de l'application
	/// </summary>
	public class ModuleLoader : MonoBehaviour
	{
		void Awake()
		{
			m_fullRepository = new ModuleRepository();
		}
		
		public void LoadModulesAsync()
		{
			StartCoroutine(LoadAllModulesCoroutine());
		}

		private ModuleDescriptor PickModuleToLoad(HashSet<ModuleDescriptor> allModulesToLoad)
		{
			foreach(ModuleDescriptor module in allModulesToLoad)
			{
				if(!module.LastModuleToLoad || allModulesToLoad.Count == 1)
				{
					allModulesToLoad.Remove(module);
					return module;
				}
			}
			return null;
		}

		private ModuleDescriptor FindModuleDescriptorByName(string moduleName)
		{
			foreach(ModuleDescriptor descriptor in m_availableSubModules)
			{
				if(descriptor.ModuleName == moduleName)
					return descriptor;
			}
			return null;
		}

		private void AddDependenciesNotAlreadyLoaded(HashSet<ModuleDescriptor> modulesToLoad, HashSet<string> dependencies)
		{
			foreach(string moduleName in dependencies)
			{
				if(!m_fullRepository.Contains(moduleName))
				{
					ModuleDescriptor descriptor = FindModuleDescriptorByName(moduleName);
					if(descriptor != null)
					{
						modulesToLoad.Add(descriptor);
					}
				}
			}
		}

		private IEnumerator LoadAllModulesCoroutine()
		{
			HashSet<ModuleDescriptor> modulesToLoad = new HashSet<ModuleDescriptor>();
			List<string> orderedDependencies = new List<string>();
			orderedDependencies.Add(m_mainModule.ModuleName);
			modulesToLoad.Add(m_mainModule);
			int currentModuleIndex = 0;
			while(modulesToLoad.Count > 0)
			{
				ModuleDescriptor moduleToLoad = PickModuleToLoad(modulesToLoad);
				float progressStart = (float)(currentModuleIndex) / (m_availableSubModules.Count+2); // +1 car il faut compter le module main
				float progressEnd = (float)(currentModuleIndex+1) / (m_availableSubModules.Count+2);
				progressEnd = Mathf.Min(progressEnd, 0.99f);
				m_progress = Mathf.Lerp(progressStart, progressEnd, moduleToLoad.Progress);
				m_currentMessage = moduleToLoad.Message;
				moduleToLoad.Load();
				do
				{
					yield return new WaitForEndOfFrame();
					int progressPrecent = (int)(moduleToLoad.Progress * 100);
					m_currentMessage = moduleToLoad.Message + " (" + progressPrecent + "%)";
					m_progress = Mathf.Lerp(progressStart, progressEnd, moduleToLoad.Progress);
				} while(!moduleToLoad.Loaded);
				ModuleInstance instance = moduleToLoad.GetCreatedInstance();
				if(instance != null)
				{
					m_fullRepository.AddInstance(instance);
					HashSet<string> dependencies = instance.GetModuleDependencies();
					foreach(string dependency in dependencies)
					{
						// on met la dependance à la in du conteneur (en la supprimer puis ajoutant, on peut faire mieux en faisant un "bubble sort")
						orderedDependencies.Remove(dependency);
						orderedDependencies.Add(dependency);
					}
					AddDependenciesNotAlreadyLoaded(modulesToLoad, dependencies);
				}
				else
				{
					Debug.LogError("failed to load module " + moduleToLoad.ModuleName);
				}

				currentModuleIndex++;
			}
			orderedDependencies.Reverse();
			m_currentMessage = "Edition des liens entre les modules";
			m_progress = 0.99f;
			yield return new WaitForEndOfFrame();
			m_fullRepository.OnAllModuleInstantiated(orderedDependencies);
			yield return new WaitForEndOfFrame();
			m_progress = 1.0f;

		}

		public string CurrentMessage
		{
			get{return m_currentMessage;}
		}

		public float Progress
		{
			get{return m_progress;}
		}

		private string m_currentMessage = "";
		private float m_progress = 0.0f;
		private ModuleRepository m_fullRepository;
		[SerializeField] private ModuleDescriptor m_mainModule;
		[SerializeField] private List<ModuleDescriptor> m_availableSubModules;
	}

}
