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
	/// classe de base décrivant une instance de module.
	/// </summary>
	public abstract class ModuleInstance : MonoBehaviour
	{
		public void Awake()
		{
			Object.DontDestroyOnLoad(gameObject);
		}

		public void Activate(bool activate)
		{
			gameObject.SetActive(activate);
		}

		public bool IsActive()
		{
			return gameObject.activeSelf;
		}

		public virtual void OnAllModuleLoaded(ModuleRepository repository) {}

		public abstract HashSet<string> GetModuleDependencies();

		public string Name
		{
			get{return m_name;}
		}

		[SerializeField] private string m_name;
	}
}
