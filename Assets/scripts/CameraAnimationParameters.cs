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
// - Fichier créé le 4/22/2015 11:02:32 AM
// - Dernière modification le 4/22/2015 11:02:32 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class CameraAnimationParameters
    /// </summary>
    public class CameraAnimationParameters : MonoBehaviour
    {
		public AnimationCurve PositionCurve;
		public AnimationCurve PositionYCurve;
		public AnimationCurve OrientationCurve;
		public float RotationDuration = 10;
		public float MoveToDestinationDuration = 20;
		public float RemainingAngleToStartAnimation = 30;
	}
}
