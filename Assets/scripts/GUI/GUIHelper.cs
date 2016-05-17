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
// - Fichier créé le 4/20/2015 12:09:53 PM
// - Dernière modification le 4/20/2015 12:09:53 PM
//@END-HEADER
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class GUIHelper
    /// </summary>
    public class GUIHelper
    {
		public static Rect GetRectInViewportSpace(RectTransform rect, Canvas root)
		{
			Rect rectInPixelSize = RectTransformUtility.PixelAdjustRect(rect, root);
			rectInPixelSize.x += rect.position.x;
			rectInPixelSize.y += rect.position.y;
			Vector2 invScreenSize = new Vector2(1.0f /  Screen.width, 1.0f /  Screen.height);
			Vector2 centerInViewportSpace = Vector2.Scale(rectInPixelSize.center, invScreenSize);
			Vector2 sizeInViewportSpace = Vector2.Scale(rectInPixelSize.size, invScreenSize);
			Vector2 halfSizeInViewportSpace = sizeInViewportSpace * 0.5f;
			Rect result = new Rect(centerInViewportSpace.x - halfSizeInViewportSpace.x, centerInViewportSpace.y - halfSizeInViewportSpace.y, sizeInViewportSpace.x, sizeInViewportSpace.y);
			return result;

		}
    }
}
