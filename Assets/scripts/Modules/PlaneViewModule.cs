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
// - Fichier crÃ©Ã© le 4/20/2015 12:20:05 PM
// - DerniÃ¨re modification le 4/20/2015 12:20:05 PM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
    /// <summary>
    /// description for class PlaneViewModule
    /// </summary>
    public class PlaneViewModule : ModuleInstance
    {
		public override void OnAllModuleLoaded(ModuleRepository repository)
		{
			
		}
		
		public override HashSet<string> GetModuleDependencies()
		{
			HashSet<string> result = new HashSet<string>();
			return result;
		}

		private new void Awake()
		{
			base.Awake();
			ShowPerspectiveView(false);
			ShowOrthoView(false);
			m_highlightedObject = new List<GameObject>();
			Renderer[] allRenderers = m_planeRoot.GetComponentsInChildren<Renderer>();
			foreach(Renderer r in allRenderers)
			{
				if(r.gameObject != m_fullPlaneMesh)
					r.gameObject.SetActive(false);
			}
		}

		public void ShowPerspectiveView(bool activate)
		{
			m_perspectiveCamera.gameObject.SetActive(activate);
			if(activate)
			{
				m_perspectiveCamera.transform.position = m_cameraAnimationStartPosition.transform.position;
				m_perspectiveCamera.transform.rotation = m_cameraAnimationStartPosition.transform.rotation;
			}
			else
			{
				CameraAnimation anim = m_perspectiveCamera.GetComponent<CameraAnimation>();
				if(anim != null)
					anim.Stop ();
			}
		}

		public void SetPerspectiveCameraViewport(Rect viewport)
		{
			m_perspectiveCamera.rect = viewport;
		}

		public void ShowOrthoView(bool activate)
		{
			m_orthoCamera.gameObject.SetActive(activate);
		}

		public void SetOrthoCameraViewport(Rect viewport)
		{
			m_orthoCamera.rect = viewport;
		}

		public void SetHighlightPart(string partNameList)
		{
			if(m_highlightedObject.Count > 0)
			{
				foreach(GameObject obj in m_highlightedObject)
				{
					if(obj == m_fullPlaneMesh)
					{
						Material[] previousMaterials = obj.GetComponent<Renderer>().materials;
						Material[] materialsMinusOne = new Material[previousMaterials.Length-1];
						for(int i = 0; i < materialsMinusOne.Length; ++i)
						{
							materialsMinusOne[i] = previousMaterials[i];
                        }
                        obj.GetComponent<Renderer>().materials = materialsMinusOne;
					}
					else
					{
						obj.SetActive(false);
					}
				}
			}
			m_highlightedObject.Clear();
			if(!string.IsNullOrEmpty(partNameList))
			{
				HashSet<string> partNames = new HashSet<string>(partNameList.Split(';'));
				if(partNames.Contains(m_fullPlaneMesh.name))
				{
					m_highlightedObject.Add(m_fullPlaneMesh);
				}
				else
				{
					MeshRenderer[] allRenderers = m_planeRoot.GetComponentsInChildren<MeshRenderer>(true);
					foreach(MeshRenderer renderer in allRenderers)
					{
						if(partNames.Contains(renderer.gameObject.name))
						{
							m_highlightedObject.Add(renderer.gameObject);
						}
					}
				}
			}
			m_orthoHighlight.SetActive(m_highlightedObject.Count > 0);
			if(m_highlightedObject.Count > 0)
			{
				Bounds bounds = ComputeBoundsOfAllObjects(m_highlightedObject);
				m_orthoHighlight.transform.position = bounds.center;
				foreach(GameObject obj in m_highlightedObject)
				{
					if(obj == m_fullPlaneMesh)
					{
						Material[] previousMaterials = obj.GetComponent<Renderer>().materials;
						Material[] materialsPlusOne = new Material[previousMaterials.Length+1];
						for(int i = 0; i < previousMaterials.Length; ++i)
						{
							materialsPlusOne[i] = previousMaterials[i];
						}
						materialsPlusOne[previousMaterials.Length] = m_highlightMaterial;
                        obj.GetComponent<Renderer>().materials = materialsPlusOne;
					}
					obj.SetActive(true);
                }
            }
        }
        
        private Bounds ComputeBoundsOfAllObjects(List<GameObject> objects)
        {
            if(objects.Count == 0 || (objects.Count == 1 && objects[0] == m_fullPlaneMesh))
			{
				return new Bounds();
			}
			else
			{
				Bounds result = objects[0].GetComponent<Renderer>().bounds;
				for(int i = 1; i < objects.Count; ++i)
				{
					result.Encapsulate(objects[i].GetComponent<Renderer>().bounds);
				}
				return result;
			}
		}

		Vector3 GetMainNormalDirection(List<GameObject> objects)
		{
			Vector3 result = Vector3.zero;
			foreach(GameObject go in objects)
			{
				MeshFilter filter = go.GetComponent<MeshFilter>();
				Transform t = go.transform;
				if(filter != null)
				{
					Mesh mesh = filter.sharedMesh;
					Vector3[] vertices = mesh.vertices;
					int[] triangles = mesh.triangles;
					for(int i = 0; i < triangles.Length; i +=3)
					{
						Vector3 v0 = t.TransformPoint(vertices[triangles[i]]);
						Vector3 v1 = t.TransformPoint(vertices[triangles[i+1]]);
						Vector3 v2 = t.TransformPoint(vertices[triangles[i+2]]);
						Vector3 delta1 = v1 - v0;
						Vector3 delta2 = v2 - v0;
						Vector3 cross = Vector3.Cross(delta1, delta2);
						// cross is normal * triangle area, so we have our data !!!
						result += cross;
                    }
                }
            }
			return result;
		}

		public void StartPerspectiveCameraAnimationToViewHighlightedObject()
		{
			Bounds bounds = ComputeBoundsOfAllObjects(m_highlightedObject);
			Vector3 direction = GetMainNormalDirection(m_highlightedObject);
			m_perspectiveCamera.gameObject.AddComponent<CameraAnimation>().Init(m_cameraAnimationStartPosition.transform, bounds, direction, m_perspectiveCamera.nearClipPlane, m_cameraAnimationParameters);
		}

		private List<GameObject> m_highlightedObject;
		[SerializeField] private Camera m_perspectiveCamera;
		[SerializeField] private Camera m_orthoCamera;
		[SerializeField] private GameObject m_cameraAnimationStartPosition;
		[SerializeField] private GameObject m_orthoHighlight;
		[SerializeField] private GameObject m_planeRoot;
		[SerializeField] private CameraAnimationParameters m_cameraAnimationParameters;
		[SerializeField] private Material m_highlightMaterial;
		[SerializeField] private GameObject m_fullPlaneMesh;
	}
}
