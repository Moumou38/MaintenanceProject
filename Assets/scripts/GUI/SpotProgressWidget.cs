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
// - Fichier créé le 4/20/2015 3:09:53 PM
// - Dernière modification le 4/20/2015 3:09:53 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class SpotProgressWidget
    /// </summary>
    public class SpotProgressWidget : MonoBehaviour
    {
		public void Reset(int currentStep, int stepCount)
		{
			while(transform.childCount > 0)
			{
				Transform son = transform.GetChild(0);
				son.SetParent(null);
				GameObject.Destroy(son.gameObject);
			}
			for(int i = 0; i < stepCount; ++i)
			{
				GameObject newImage = null;
				if(currentStep == i)
				{
					newImage = GameObject.Instantiate(m_filledSpot) as GameObject;
				}
				else
				{
					newImage = GameObject.Instantiate(m_emptySpot) as GameObject;
				}
				newImage.transform.SetParent(transform);
			}
		}
		[SerializeField] private GameObject m_emptySpot;
		[SerializeField] private GameObject m_filledSpot;
    }
}
