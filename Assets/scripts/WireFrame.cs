// Copyright 2015 dassault
//
// - Description
//   -- Description synthétique du contenu du fichier
//
// - Namespace(s)
//   -- Préciser le ou les namespaces utilisés dans ce fichier
//
// - Auteurs
//   -- de Verdelhan(Theoris)
//
// - Fichier créé le XXX
// - Dernière modification le XXX
//
using UnityEngine;
using System.Collections.Generic;

namespace dassault
{
	public class WireFrame : MonoBehaviour
	{
		Material LastMaterial;
		[SerializeField] private Material m_lineMaterial;
		[SerializeField] private Material m_meshMaterial;
		string lineshader = "Shader \"Unlit/Color\" { Properties { _Color(\"Color\", Color) = (1, 1, 1, 1) } SubShader { Lighting Off Color[_Color] Pass {} } }";

		class Edge
		{
			public Edge(Vector3 p0, Vector3 p1, Vector3 n0, Vector3 n1)
			{
				this.p0 = p0;
				this.p1 = p1;
				this.n0 = n0;
				this.n1 = n1;
				triangleNormals = new List<Vector3>();
			}
			public bool Equal(Edge other)
			{
				return 	(p0 == other.p0 && p1 == other.p1) ||
						(p0 == other.p1 && p1 == other.p0);
			}
			public Vector3 p0;
			public Vector3 p1;
			public Vector3 n0;
			public Vector3 n1;
			public List<Vector3> triangleNormals;
		}

		private void AddToList(Edge e, List<Edge> edges, Vector3 triangleNormal)
		{
			for(int i = 0; i < edges.Count; ++i)
			{
				if(edges[i].Equal(e))
				{
					edges[i].triangleNormals.Add(triangleNormal);
					return;
				}
			}
			edges.Add(e);
			e.triangleNormals.Add(triangleNormal);
		}

		private List<Edge> filterEdges(List<Edge> edges)
		{
			List<Edge> result = new List<Edge>(edges.Count);
			foreach(Edge edge in edges)
			{
				if(edge.triangleNormals.Count != 2)
				{
					result.Add(edge);
				}
				else
				{

					float angle = Vector3.Angle(edge.triangleNormals[0], edge.triangleNormals[1]);
					if(angle > 15)
					{
						result.Add(edge);
					}
				}
			}
			return result;
		}

		public void OnEnable()
		{
			var mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			var renderer = gameObject.GetComponent<MeshRenderer>();
			LastMaterial = renderer.material;
			renderer.material = m_meshMaterial;
			Vector3[] vertices = mesh.vertices;
			Vector3[] originalNormales = mesh.normals;
			int[] triangles = mesh.triangles;
			List<Edge> allEdges = new List<Edge>();
			for(int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 p0 = vertices[triangles[i]];
				Vector3 p1 = vertices[triangles[i+1]];
				Vector3 p2 = vertices[triangles[i+2]];
				Vector3 n0 = originalNormales[triangles[i]];
				Vector3 n1 = originalNormales[triangles[i+1]];
				Vector3 n2 = originalNormales[triangles[i+2]];
				Vector3 side0 = p1 - p0;
				Vector3 side1 = p2 - p0;
				Vector3 triangleNormal = Vector3.Cross(side0, side1).normalized;
				AddToList(new Edge(p0, p1, n0, n1), allEdges, triangleNormal);
				AddToList(new Edge(p0, p2, n0, n2), allEdges, triangleNormal);
				AddToList(new Edge(p2, p1, n2, n1), allEdges, triangleNormal);
			}
			List<Edge> edges = filterEdges(allEdges);
			Vector3[] lineVertices = new Vector3[edges.Count * 2];
			Vector2[] lineUvs = new Vector2[edges.Count * 2];
			Vector3[] lineNormals = new Vector3[edges.Count * 2];
			int[] linesIndices = new int[edges.Count * 2];
			for(int i = 0; i < edges.Count; ++i)
			{
				lineVertices[i*2] = edges[i].p0;
				lineVertices[i*2+1] = edges[i].p1;
				lineNormals[i*2] = edges[i].n0;
				lineNormals[i*2+1] = edges[i].n1;
				lineUvs[i*2] = Vector2.zero;
				lineUvs[i*2+1] = Vector2.zero;
				linesIndices[i*2] = i*2;
				linesIndices[i*2+1] = i*2+1;
			}
			Mesh GeneratedMesh = new Mesh();
			GeneratedMesh.name = "Generated Wireframe";
			GeneratedMesh.vertices = lineVertices;
			GeneratedMesh.normals = lineNormals;
			GeneratedMesh.uv = lineUvs;
			GeneratedMesh.SetIndices(linesIndices, MeshTopology.Lines, 0);

			GameObject son = new GameObject();
			son.transform.parent = transform;
			son.transform.localPosition = Vector3.zero;
			son.transform.localRotation = Quaternion.identity;
			MeshRenderer newMeshRenderer = son.AddComponent<MeshRenderer>();
			newMeshRenderer.material = m_lineMaterial;
			MeshFilter newMeshFilter = son.AddComponent<MeshFilter>();
			newMeshFilter.mesh = GeneratedMesh;
			//Material tempmaterial = new Material(lineshader);
			//renderer.material = m_lineMaterial;
		}
		public void OnDisable()
		{
			gameObject.GetComponent<MeshRenderer>().material = LastMaterial;
		}
	}
}
