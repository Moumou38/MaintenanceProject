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
// - Fichier créé le 4/23/2015 9:08:55 AM
// - Dernière modification le 4/23/2015 9:08:55 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class PlateformSelectorModule
    /// </summary>
	public class PlateformSelectorModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			Activate(false);
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();

            // On passe par un booleen pour pouvoir faire des test du mode lunettes sur PC
            // BL: dans le cadre de la Demo Support Distant cette application est toujours l'application glass,
            //  l'applciation hostée sur la tablette est une application différentes
            //bool useGlassApplication = false;
            //#if UNITY_ANDROID
            //			useGlassApplication = true;
            //#endif

            bool useGlassApplication = true;

			if(useGlassApplication)
			{
				result.Add ("GlassApplicationModule");
			}
			else
			{
				result.Add("PadApplicationModule");
			}
			return result;
		}
		
		private new void Awake()
		{
			base.Awake();
		}
	}
}
