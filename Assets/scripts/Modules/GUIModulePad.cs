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
// - Fichier créé le 4/22/2015 5:44:14 PM
// - Dernière modification le 4/22/2015 5:44:14 PM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class GUIModulePad
    /// </summary>
	public class GUIModulePad : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			m_glassViewportInitialPosition = m_glassViewport.position;
			RectTransform t = m_watchScreen.transform as RectTransform;
			m_watchScreenInitialPosition = t.position;
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			return result;
		}
		
		private new void Awake()
		{
			base.Awake();
			AnnotationScreen.Show(false);
			ConnectionScreen.Show(false);
		}

		public void AddToGlassViewport(RectTransform transformToMove)
		{
			transformToMove.parent = m_glassViewport;
			transformToMove.anchorMin = Vector2.zero;
			transformToMove.anchorMax = Vector2.one;
			transformToMove.offsetMin = Vector2.zero;
			transformToMove.offsetMax = Vector2.zero;
		}

		public void ShowGlassViewport(bool show)
		{
			if(show)
			{
				m_glassViewport.position = m_glassViewportInitialPosition;
			}
			else
			{
				m_glassViewport.position = m_glassViewport.position + new Vector3(2000, 0, 0);
			}
		}

		public void ShowWatchScreen(bool show)
		{
			RectTransform t = m_watchScreen.transform as RectTransform;
			if(show)
			{
				t.position = m_watchScreenInitialPosition;
			}
			else
			{
				t.position = t.position + new Vector3(2000, 0, 0);
			}
		}

		public AnnotationScreen AnnotationScreen {get{return m_annoationScreen;}}
		public WatchScreen WatchScreen {get{return m_watchScreen;}}
		public ConnectionScreen ConnectionScreen {get{return m_connectionScreen;}}

		private Vector3 m_glassViewportInitialPosition;
		private Vector3 m_watchScreenInitialPosition;
		[SerializeField] private RectTransform m_glassViewport;
		[SerializeField] private AnnotationScreen m_annoationScreen;
		[SerializeField] private WatchScreen m_watchScreen;
		[SerializeField] private ConnectionScreen m_connectionScreen;
	}
}
