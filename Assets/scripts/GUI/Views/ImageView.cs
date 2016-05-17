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
// - Fichier créé le 4/23/2015 5:43:59 PM
// - Dernière modification le 4/23/2015 5:43:59 PM
//@END-HEADER
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace dassault
{
    /// <summary>
    /// description for class ImageView
    /// </summary>
    public class ImageView : ViewWithProgressBar
    {
		public void SetImage(Texture image)
		{
			m_image.texture = image;
			float aspectRatio = image.width / (float)image.height;
			Rect contentRect = this.Content.rect;
			if(aspectRatio < 1)
			{
				int width = (int)(contentRect.height / aspectRatio);
				int height = (int)(contentRect.height);
				Vector2 newSize = new Vector2(width, height);
				RectTransform transform = m_image.rectTransform;
				Vector2 oldSize = transform.rect.size;
				Vector2 deltaSize = newSize - oldSize;
				transform.offsetMin = transform.offsetMin - new Vector2(deltaSize.x * transform.pivot.x, deltaSize.y * transform.pivot.y);
				transform.offsetMax = transform.offsetMax + new Vector2(deltaSize.x * (1f - transform.pivot.x), deltaSize.y * (1f - transform.pivot.y));
			}
			else
			{
				int width = (int)(contentRect.height * aspectRatio);
				int height = (int)contentRect.height;
				Vector2 newSize = new Vector2(width, height);
				RectTransform transform = m_image.rectTransform;
				Vector2 oldSize = transform.rect.size;
				Vector2 deltaSize = newSize - oldSize;
				transform.offsetMin = transform.offsetMin - new Vector2(deltaSize.x * transform.pivot.x, deltaSize.y * transform.pivot.y);
				transform.offsetMax = transform.offsetMax + new Vector2(deltaSize.x * (1f - transform.pivot.x), deltaSize.y * (1f - transform.pivot.y));
			}

		}

		protected override void VirtualOnTransitionStart(bool visible)
		{
			ShowImage(false);
		}
		
		public void ShowImage(bool show)
		{
			m_image.gameObject.SetActive(show);
		}

		[SerializeField] private RawImage m_image;
	}
}
