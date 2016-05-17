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
// - Fichier créé le 4/24/2015 1:40:41 PM
// - Dernière modification le 4/24/2015 1:40:41 PM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class AnimationModule
    /// </summary>
    public class AnimationModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			Activate(false);
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			return result;
		}
		
		private new void Awake()
		{
			base.Awake();			
		}

		public void SetCameraViewport(Rect viewport)
		{
			m_camera.rect = viewport;
		}

		public void PlayAnimation(string category, string animationName)
		{
            m_animations.RemoveAll(item => item == null);
			foreach(AnimationDescriptor descriptor in m_animations)
			{
				if(descriptor.Category == category)
				{
					descriptor.Show(true);
					descriptor.PlayAnimation(animationName, m_camera);
				}
				else
				{
					descriptor.Show(false);
				}
			}
		}

        public void loadDescriptors()
        {
            Activate(true);
            m_animations = new List<AnimationDescriptor>();
            AnimationDescriptor[] descriptors = GetComponentsInChildren<AnimationDescriptor>();
            foreach (AnimationDescriptor descriptor in descriptors)
            {
                m_animations.Add(descriptor);
            }
            Activate(false);
        }

		private List<AnimationDescriptor> m_animations;
		[SerializeField] private Camera m_camera;
	}
}
