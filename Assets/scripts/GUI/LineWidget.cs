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
// - Fichier créé le 4/27/2015 4:57:15 PM
// - Dernière modification le 4/27/2015 4:57:15 PM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class LineBehaviour
    /// </summary>
    public class LineWidget : MonoBehaviour
    {
        // Update is called once per frame
        void LateUpdate ()
        {
			RectTransform thisRectTransform = transform as RectTransform;
			Rect leftRect = RectTransformUtility.PixelAdjustRect(m_leftTarget, m_canvas);
			Rect rightRect = RectTransformUtility.PixelAdjustRect(m_rightTarget, m_canvas);
			leftRect.x += m_leftTarget.position.x; 
			leftRect.y += m_leftTarget.position.y;
			rightRect.x += m_rightTarget.position.x;
			rightRect.y += m_rightTarget.position.y;
			Vector2 leftAnchorPoint = new Vector2(leftRect.xMax, leftRect.center.y);
			Vector2 rightAnchorPoint = new Vector2(rightRect.xMin, rightRect.center.y);
			if(leftAnchorPoint.y > rightAnchorPoint.y)
			{
				float height = leftAnchorPoint.y - rightAnchorPoint.y;
				float width = Mathf.Abs(leftAnchorPoint.x - rightAnchorPoint.x);
				float halfWidth = width / 2;
				SetHeight(thisRectTransform, height);
				SetWidth(thisRectTransform, width);
				SetLeftTopPosition(thisRectTransform, new Vector2(0, 0));
				SetLeftTopPosition(m_leftBar, new Vector2(0, height));
				SetRightBottomPosition(m_rightBar, new Vector2(width, 0));
			}
			else
			{
				float height = rightAnchorPoint.y - leftAnchorPoint.y;
				float width = Mathf.Abs(leftAnchorPoint.x - rightAnchorPoint.x);
				float halfWidth = width / 2;
				SetHeight(thisRectTransform, height);
				SetWidth(thisRectTransform, width);
				SetLeftTopPosition(thisRectTransform, new Vector2(0, height));
				SetLeftTopPosition(m_leftBar, new Vector2(0, 2));
				SetRightBottomPosition(m_rightBar, new Vector2(width, height - 2));
			}
		}

		private static void SetSize(RectTransform trans, Vector2 newSize) {
			Vector2 oldSize = trans.rect.size;
			Vector2 deltaSize = newSize - oldSize;
			trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
			trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
		}

		private static void SetWidth(RectTransform trans, float newSize) {
			SetSize(trans, new Vector2(newSize, trans.rect.size.y));
		}

		private static void SetHeight(RectTransform trans, float newSize) {
			SetSize(trans, new Vector2(trans.rect.size.x, newSize));
		}

		private static void SetLeftBottomPosition(RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}
		private static void SetLeftTopPosition(RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}
		private static void SetRightBottomPosition(RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
		}
		private static void SetRightTopPosition(RectTransform trans, Vector2 newPos) {
			trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
		}

		[SerializeField] private Canvas m_canvas;
		[SerializeField] private RectTransform m_leftTarget;
		[SerializeField] private RectTransform m_rightTarget;
		[SerializeField] private RectTransform m_leftBar;
		[SerializeField] private RectTransform m_rightBar;
    }
}
