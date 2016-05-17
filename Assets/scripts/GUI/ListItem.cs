// Copyright 2015 Dassault
//
// - Description
//	 -> 	public class ListItem : MonoBehaviour, IPointerClickHandler : TODO

//
// - Namespace(s)
//	namespace dassault

//
// - Auteurs
//	\author Michel de Verdelhan <mdeverdelhan@theoris.fr>

//
// - Fichier créé le 
//		\date   Thu Feb 12 14:20:32 2015 +0100
// - Dernière modification le 
//		\date   Fri Feb 20 13:07:55 2015 +0100
//@END-HEADER



using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace dassault
{
	public class ListItem : MonoBehaviour, IPointerClickHandler
	{
		public delegate void OnSelectedDelegate(ListItem item);
		public event OnSelectedDelegate OnSelectedCallback;
		public delegate void OnDoubleClickedDelegate(ListItem item);
		public event OnDoubleClickedDelegate OnDoubleClickedCallback;

		void Start()
		{
			SetSelected(false);
		}

		/// <summary>
		/// fonction virtuelle permettant de refiner le comportement de l'item dans une sous classe
		/// </summary>
		/// <param name="selected">If set to <c>true</c> selected.</param>
		public virtual void OnSelected(bool selected)
		{
		}

		public void SetSelected(bool selected)
		{
			Button button = GetComponent<Button>();
			if(selected)
			{
				if(button != null)
				{
					button.colors = m_selectedColor;
				}
				if(OnSelectedCallback != null)
					OnSelectedCallback(this);
			}
			else
			{
				if(button != null)
				{
					button.colors = m_normalColor;
				}
			}
			OnSelected(selected);
		}

		public void OnPointerClick (PointerEventData eventData)
		{
			if(eventData.button == PointerEventData.InputButton.Left)
			{
				if(eventData.clickCount == 1)
				{
					SetSelected (true);
				}
				else if(eventData.clickCount == 2)
				{
					if(OnDoubleClickedCallback != null)
						OnDoubleClickedCallback(this);
				}
			}
		}

		[SerializeField] ColorBlock m_normalColor = ColorBlock.defaultColorBlock;
		[SerializeField] ColorBlock m_selectedColor = ColorBlock.defaultColorBlock;
	}
}
