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
using System.Collections.Generic;

namespace dassault
{
	/// <summary>
	/// classe contenant les instance de modules de l'application
	/// </summary>
	public class ModuleRepository
	{
		public ModuleRepository()
		{
			m_instances = new Dictionary<string, ModuleInstance>();
		}

		public void AddInstance(ModuleInstance instance)
		{
			m_instances.Add(instance.name, instance);
		}

		public ModuleInstance Get(string moduleName)
		{
			return m_instances[moduleName];
		}

		public bool Contains(string moduleName)
		{
			return m_instances.ContainsKey(moduleName);
		}

		public void OnAllModuleInstantiated(List<string> modulesInitializationOrder)
		{
			foreach(string moduleName in modulesInitializationOrder)
			{
				ModuleInstance moduleInstance = m_instances[moduleName];
				HashSet<string> moduleDependencies = moduleInstance.GetModuleDependencies();
				ModuleRepository subModules = CreateSubset(moduleDependencies);
				moduleInstance.OnAllModuleLoaded(subModules);
			}
		}

		private ModuleRepository CreateSubset(HashSet<string> modulesToInclude)
		{
			ModuleRepository result = new ModuleRepository();
			foreach(string moduleName in modulesToInclude)
			{
				if(!Contains(moduleName))
				{
					Debug.LogError("unable to find module " + moduleName + " while creating sub module repository");
				}
				result.AddInstance(Get (moduleName));
			}
			return result;
		}

		private Dictionary<string, ModuleInstance> m_instances;
		
	}
}
