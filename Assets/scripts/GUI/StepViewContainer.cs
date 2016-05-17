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
// - Fichier créé le 4/20/2015 11:39:01 AM
// - Dernière modification le 4/20/2015 11:39:01 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class StepViewContainer
    /// </summary>
    public class StepViewContainer : MonoBehaviour
    {
		public StepView CreateStepView()
		{
			GameObject newStepViewGameObject = GameObject.Instantiate(m_stepViewPrefab) as GameObject;
			int offsetX = m_offsetX * m_currentViews.Count;
			int offsetY = m_offsetY * m_currentViews.Count;
			newStepViewGameObject.transform.SetParent(transform);
			newStepViewGameObject.transform.localPosition = new Vector3(offsetX, offsetY, 0);
			StepView stepView = newStepViewGameObject.GetComponent<StepView>();
			m_currentViews.Add(stepView, newStepViewGameObject);
			return stepView;
		}

		public void RemoveStepView(StepView toRemove)
		{
			GameObject go = m_currentViews[toRemove];
			GameObject.Destroy(go);
			m_currentViews.Remove(toRemove);
		}

		public void Clear()
		{
			foreach(KeyValuePair<StepView, GameObject> keyValue in m_currentViews)
			{
				GameObject.Destroy(keyValue.Value);
			}
			m_currentViews.Clear();
		}

		private Dictionary<StepView, GameObject> m_currentViews = new Dictionary<StepView, GameObject >();
		[SerializeField] private GameObject m_stepViewPrefab;
		[SerializeField] private int m_offsetX = 23;
		[SerializeField] private int m_offsetY = 23;
	}
}
