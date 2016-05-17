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
// - Fichier crÃ©Ã© le 4/22/2015 9:14:37 AM
// - DerniÃ¨re modification le 4/22/2015 9:14:37 AM
//@END-HEADER
using UnityEngine;
using System.Collections;

namespace dassault
{
    /// <summary>
    /// description for class CameraAnimation
    /// </summary>
    public class CameraAnimation : MonoBehaviour
    {
		public CameraAnimation Init(Transform startPosition, Bounds target, Vector3 orientation, float distanceOffset, CameraAnimationParameters parameters)
		{
			m_startPosition = startPosition;
			m_target = target;
			m_elapsedTime = 0;
			m_parameters = parameters;

			if(m_target.center != Vector3.zero)
			{
				Vector3 targetPos = target.center;
				Vector3 targetDirection = orientation;
				if(orientation.magnitude < 0.6f)
				{
					targetDirection = targetPos.normalized;
					Vector3 extent = m_target.extents.normalized;
					if(Mathf.Abs (extent.x) < 0.1)
					{
						targetDirection.x += Mathf.Sign(targetDirection.x);
					}
					if(Mathf.Abs (extent.y) < 0.1)
					{
						targetDirection.y += Mathf.Sign(targetDirection.y);
					}
					float dotResult = Vector3.Dot (targetDirection.normalized, Vector3.forward);
					if(Mathf.Abs(dotResult) > 0.9)
					{
						targetDirection.x += Mathf.Sign(targetDirection.x);
					}
				}
				ComputeEndAnimationPos(target, targetPos, targetDirection.normalized, distanceOffset);
			}
			return this;
		}

		public void Stop()
		{
			GameObject.Destroy(this);
		}

        // Update is called once per frame
        void Update ()
        {
			m_elapsedTime += Time.deltaTime;
			if(m_target.center != Vector3.zero)
			{
				if(m_elapsedTime < m_parameters.RotationDuration && !m_angleReached)
				{
					RotationAnimation(m_elapsedTime / m_parameters.RotationDuration);
				}
				else
				{
					MoveToDestinationAnimation();
				}
			}
			else
			{
				Vector3 cameraPositionForRotation;
				Quaternion cameraOrientationForRotation;
				float factor = m_elapsedTime / m_parameters.RotationDuration;
				ComputeRotationAroundZero(factor, out cameraPositionForRotation, out cameraOrientationForRotation);
				transform.position = cameraPositionForRotation;
				transform.rotation = cameraOrientationForRotation;
			}
        }

		private void RotationAnimation(float factor)
		{
			Vector3 cameraPositionForRotation;
			Quaternion cameraOrientationForRotation;
			ComputeRotationAroundZero(factor, out cameraPositionForRotation, out cameraOrientationForRotation);
			transform.position = cameraPositionForRotation;
			transform.rotation = cameraOrientationForRotation;

			Vector3 endPositionOnPlane = new Vector3(m_endPosition.x, 0, m_endPosition.z);
			Vector3 rotationPositionOnPlane = new Vector3(cameraPositionForRotation.x, 0, cameraPositionForRotation.z);
			float angle = FullAngle(endPositionOnPlane, rotationPositionOnPlane, Vector3.up) * Mathf.Rad2Deg;
			if(angle <= m_parameters.RemainingAngleToStartAnimation)
			{
				m_angleReached = true;
				m_elapsedTimeAtAngleReached = m_elapsedTime;
				//m_endRotationPosition = cameraPositionForRotation;
				//m_endRotationOrientation = cameraOrientationForRotation;
			}
		}

		private void MoveToDestinationAnimation()
		{
			float animationElapsedTime = m_elapsedTime - m_elapsedTimeAtAngleReached;
			float animationFactor = animationElapsedTime / m_parameters.MoveToDestinationDuration;
			float rotationFactor = (m_elapsedTimeAtAngleReached + animationElapsedTime * (1-animationFactor)) / m_parameters.RotationDuration;
			Vector3 cameraPositionForRotation;
			Quaternion cameraOrientationForRotation;
			ComputeRotationAroundZero(rotationFactor, out cameraPositionForRotation, out cameraOrientationForRotation);


			float yLerpFactor = m_parameters.PositionYCurve.Evaluate(animationFactor);
			float posLerpFactor = m_parameters.PositionCurve.Evaluate(animationFactor);
			float orientationLerpFactor = m_parameters.OrientationCurve.Evaluate(animationFactor);
			float y = Mathf.Lerp(cameraPositionForRotation.y, m_endPosition.y, yLerpFactor);
			Vector3 cameraPosition = Vector3.Lerp(cameraPositionForRotation, m_endPosition, posLerpFactor);
			cameraPosition.y = y;
			Quaternion cameraOrientation = Quaternion.Lerp(cameraOrientationForRotation, m_endOrientation, orientationLerpFactor);
			transform.position = cameraPosition;
			transform.rotation = cameraOrientation;
			if(animationFactor >= 1)
			{
				Stop ();
			}
		}

		private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
		{
			v1.Normalize();
			v2.Normalize();
			n.Normalize();
			return Mathf.Atan2(
				Vector3.Dot(n, Vector3.Cross(v1, v2)),
				Vector3.Dot(v1, v2));
		}

		private float FullAngle(Vector3 v1, Vector3 v2, Vector3 n)
		{
			float angle = AngleSigned(v1, v2, n);
			if(angle < 0)
				angle += 2*Mathf.PI;
			return angle;
		}

		private void ComputeRotationAroundZero(float rotationFactor, out Vector3 resultPos, out Quaternion resultOrientation)
		{
			Vector2 startPositionOnPlane = new Vector2(m_startPosition.position.x, m_startPosition.position.z);
			float baseAngle = Vector2.Angle(Vector2.right, startPositionOnPlane) * Mathf.Deg2Rad;
			float distanceOnXZ = startPositionOnPlane.magnitude;
			float x = Mathf.Cos(-baseAngle + Mathf.PI * 2 * rotationFactor) * distanceOnXZ;
			float z = Mathf.Sin(-baseAngle + Mathf.PI * 2 * rotationFactor) * distanceOnXZ;
			resultPos = new Vector3(x, m_startPosition.position.y, z);
			resultOrientation = Quaternion.LookRotation(-resultPos.normalized);
		}

		private void ComputeEndAnimationPos(Bounds bounds, Vector3 targetPosition, Vector3 targetNormal, float distanceOffset)
		{
			float radius = bounds.extents.magnitude;
			float distance = radius / (Mathf.Sin(GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad / 2f)) + distanceOffset;
			m_endPosition = targetPosition + targetNormal * distance;
			m_endOrientation = Quaternion.LookRotation(-targetNormal);
		}

		private float m_elapsedTime;
		private bool m_angleReached;
		private float m_elapsedTimeAtAngleReached;
		private Transform m_startPosition;
		private Bounds m_target;
		private Vector3 m_endPosition;
		private Quaternion m_endOrientation;
		private CameraAnimationParameters m_parameters;
	}
}
