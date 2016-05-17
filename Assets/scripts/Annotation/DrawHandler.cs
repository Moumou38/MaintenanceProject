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
// - Fichier créé le 4/21/2015 10:23:08 AM
// - Dernière modification le 4/21/2015 10:23:08 AM
//@END-HEADER
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace dassault
{
    /// <summary>
    /// description for class DrawHandler
    /// </summary>
    public class DrawHandler : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
			m_mouseDown = false;
			m_currentLine = new List<Vector3>();
        }

		public void Reset()
		{
			while(m_lineRoot.transform.childCount > 0)
			{
				Transform t = m_lineRoot.transform.GetChild(m_lineRoot.transform.childCount - 1);
				t.parent = null;
				GameObject.Destroy(t.gameObject);
			}
			m_currentLine.Clear();
			m_mouseDown = false;
		}

		public byte[] TakeScreenshot()
		{
			Rect cameraRect = m_camera.pixelRect;
			RenderTexture screenshotRenderTexture = RenderTexture.GetTemporary((int)cameraRect.width, (int)cameraRect.height, 32, RenderTextureFormat.ARGB32);
			//screenshotRenderTexture.antiAliasing = 8; // TODO : trouver un moyen pour spécifier le niveau max d'antialiasing
			
			RenderTexture.active = screenshotRenderTexture;
			RenderTexture previousRenderTexture = m_camera.targetTexture;
			m_camera.targetTexture = screenshotRenderTexture;
			m_camera.rect = new Rect(0, 0, 1, 1);
			m_camera.Render();
			m_camera.pixelRect = cameraRect;
			m_camera.targetTexture = previousRenderTexture;
			
			Texture2D resultTexture = new Texture2D((int)cameraRect.width, (int)cameraRect.height, TextureFormat.RGB24, false);
			resultTexture.ReadPixels(new Rect(0, 0, cameraRect.width, cameraRect.height), 0, 0, false);
			resultTexture.Apply();
			RenderTexture.active = null;
			
			RenderTexture.ReleaseTemporary(screenshotRenderTexture);
			
			return resultTexture.EncodeToPNG();
		}
		
		public void SetCurrentSize(float size)
		{
			m_size = size;
		}

		public void SetCurrentColor(Color color)
		{
			m_color = color;
		}
    
        // Update is called once per frame
        void Update ()
        {
			if(m_mouseDown)
			{
				Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit result;
				if(Physics.Raycast(ray, out result))
				{
					Vector3 newPoint = result.point + ComputeOffset();
					Vector3 delta = m_currentLine[m_currentLine.Count - 2] - newPoint;
					float length = delta.magnitude;
					if(length < m_distanceBetweenPoints)
					{
						m_currentLine[m_currentLine.Count - 1] = newPoint;
					}
					else
					{
						m_currentLine.Add(newPoint);
					}
					UpdateMesh();
				}
			}
			if(Input.GetKeyDown(KeyCode.Space))
			{
				TakeScreenshot();
			}
        }

		private void OnMouseDown()
		{
			m_mouseDown = true;
			m_currentLine.Clear();
			Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit result;
			if(Physics.Raycast(ray, out result))
			{
				Vector3 offset = ComputeOffset();
				m_currentLine.Add(result.point + offset);
				m_currentLine.Add(result.point + offset);
				m_currentLineHolder = new GameObject();
				m_currentLineHolder.transform.parent = m_lineRoot.transform;
				MeshFilter meshFilter = m_currentLineHolder.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = new Mesh();
				MeshRenderer renderer = m_currentLineHolder.AddComponent<MeshRenderer>();
				renderer.material = new Material(m_lineMaterial);
				renderer.material.color = m_color;
			}
		}

		private Vector3 ComputeOffset()
		{
			float offset = m_lineRoot.transform.childCount * 0.01f;
			return new Vector3(0, 0, -offset);
		}

		private void OnMouseUp()
		{
			m_mouseDown = false;
		}

		private void UpdateMesh()
		{
			int vertexCount = m_currentLine.Count * 2;
			Vector3[] vertices = new Vector3[vertexCount];
			Vector3[] normales = new Vector3[vertexCount];
			Vector2[] uvs = new Vector2[vertexCount];
			Vector3 direction = m_currentLine[1] - m_currentLine[0];
			float length = direction.magnitude;
			if(length < 0.01f)
				return;
			direction /= length;

			Vector3 side = Vector3.Cross(direction, Vector3.forward).normalized * m_size;
			vertices[0] = m_currentLine[0] + side;
			vertices[1] = m_currentLine[0] - side;

			for(int i = 1; i < m_currentLine.Count - 1; ++i)
			{
				Vector3 previousPoint = m_currentLine[i-1];
				Vector3 nextPoint = m_currentLine[i+1];
				direction = nextPoint - previousPoint;
				direction.Normalize();
				side = Vector3.Cross(direction, Vector3.forward).normalized * m_size;
				vertices[i*2] = m_currentLine[i] + side;
				vertices[i*2+1] = m_currentLine[i] - side;
				normales[i*2] = -Vector3.forward;
				normales[i*2+1] = -Vector3.forward;
				uvs[i*2] = Vector2.zero;
				uvs[i*2+1] = Vector2.zero;
			}

			direction = m_currentLine[m_currentLine.Count - 1] - m_currentLine[m_currentLine.Count - 2];
			length = direction.magnitude;
			if(length < 0.01f)
			{
				vertices[vertices.Length - 2] = vertices[vertices.Length - 4];
				vertices[vertices.Length - 1] = vertices[vertices.Length - 3];
			}
			else
			{
				side = Vector3.Cross(direction, Vector3.forward).normalized * m_size;
				vertices[vertices.Length - 2] = m_currentLine[m_currentLine.Count - 1] + side;
				vertices[vertices.Length - 1] = m_currentLine[m_currentLine.Count - 1] - side;
			}

			int triangleCount = (m_currentLine.Count-1) * 6;
			int[] trianglesIndices = new int[triangleCount];
			for(int i = 0; i < m_currentLine.Count-1; ++i)
			{
				trianglesIndices[i*6  ] = i*2;
				trianglesIndices[i*6+1] = i*2+1;
				trianglesIndices[i*6+2] = (i+1)*2;
				trianglesIndices[i*6+3] = (i+1)*2;
				trianglesIndices[i*6+4] = i*2+1;
				trianglesIndices[i*6+5] = (i+1)*2+1;
			}

			MeshFilter meshFilter = m_currentLineHolder.GetComponent<MeshFilter>();
			Mesh mesh = meshFilter.sharedMesh;
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = trianglesIndices;
		}

		private bool m_mouseDown;
		private List<Vector3> m_currentLine;
		private GameObject m_currentLineHolder;
		[SerializeField] private GameObject m_lineRoot;
		[SerializeField] private Camera m_camera;
		[SerializeField] private float m_size;
		[SerializeField] private float m_distanceBetweenPoints = 0.01f;
		[SerializeField] private Color m_color;
		[SerializeField] private Material m_lineMaterial;
    }
}
