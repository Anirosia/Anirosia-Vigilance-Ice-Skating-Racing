using System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;

namespace LevelScripts
{
	public class SurfaceGen : MonoBehaviour
	{
		private Mesh m_mesh;
		private List<Vector3[]> m_curves = new List<Vector3[]>();
		private List<Vector3> m_vertices = new List<Vector3>();
		private List<int> m_triangles = new List<int>();

		private void Start() {
			var filter = GetComponent<MeshFilter>();
			m_mesh = filter.mesh;
			m_mesh.Clear();

			var xPos = 0f;
			for (int c = 0; c < 10; c++) {
				var curve = new Vector3[4];
				for (int i = 0; i < curve.Length; i++) {
					Vector3[] prev = null;

					if (m_curves.Count > 0) {
						prev = m_curves[m_curves.Count - 1];
					}

					if (prev != null && i == 0) {
						curve[i] = prev[curve.Length - 1];
					}
					else if (prev != null && i == 1) {
						curve[i] = 2f * prev[curve.Length - 1] - prev[curve.Length - 2];
					}
					else curve[i] = new Vector3(xPos, Random.Range(1f, 2f), 0f);

					xPos += 0.5f;
				}

				m_curves.Add(curve);
			}


			foreach (var curve in m_curves) {
				int m_resolution = 20;
				for (int i = 0; i < m_resolution; i++) {
					float t = (float) i / (float) (m_resolution - 1);
					Vector3 p = BézierPoint(t, curve[0], curve[1], curve[2], curve[3]);
					AddTerrainPoint(p);
				}
			}

			m_mesh.vertices = m_vertices.ToArray();
			m_mesh.triangles = m_triangles.ToArray();

			Destroy(GetComponent<MeshCollider>());
			MeshCollider collider = gameObject.AddComponent<MeshCollider>();
			collider.sharedMesh = null;
			collider.sharedMesh = m_mesh;
		}

		private void AddTerrainPoint(Vector3 point) {
			m_vertices.Add(new Vector3(point.x, 0f, 0f));
			m_vertices.Add(point);
			if (m_vertices.Count >= 4) {
				int start = m_vertices.Count - 4;
				m_triangles.Add(start        + 0);
				m_triangles.Add(start        + 1);
				m_triangles.Add(start        + 2);
				m_triangles.Add(start        + 1);
				m_triangles.Add(start        + 3);
				m_triangles.Add(start        + 2);
			}
		}

		private Vector3 BézierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			//Huh
			// float u = 1 - t;
			// float tt = t   * t;
			// float uu = u   * u;
			// float uuu = uu * u;
			// float ttt = tt * t;
			//
			// Vector3 p = uuu * p0;
			// p += 3   * uu * t * p1;
			// p += 3   * u  * t * p2;
			// p += ttt * p3;

			float u = 1 - t;
			float tt = t   * t;
			float uu = u   * u;
			float uuu = uu * u;
			float ttt = tt * t;

			Vector3 p = uuu * p0;
			p += 3   * uu * t  * p1;
			p += 3   * u  * tt * p2;
			p += ttt * p3;

			return p;
		}


		// public bool generateContinuously = false;
		// public bool generateCollider = false;
		// [Range(0.1f,50.0f)]
		// public float yScaling = 5.0f;
		// [Range(0.1f,20.0f)]
		// public float detailScaling = 1.0f;
		// [HideInInspector]
		// public Vector3[] vertices;
		//
		// private Mesh mesh;
		//
		// void Start()
		// {
		// 	mesh = GetComponent<MeshFilter>().mesh;
		// 	vertices = mesh.vertices;
		// }
		//
		// void Update()
		// {
		// 	GenerateSurface();
		// }
		//
		// void GenerateSurface()
		// {
		// 	vertices = mesh.vertices;
		// 	int counter = 0;
		// 	for (int i = 0; i < 11; i++)
		// 	{
		// 		for (int j = 0; j < 11; j++)
		// 		{
		// 			MeshCalculate(counter, i);
		// 			counter++;
		// 		}
		// 	}
		//
		// 	mesh.vertices = vertices;
		// 	mesh.RecalculateBounds();
		// 	mesh.RecalculateNormals();
		//
		// 	if (generateCollider)
		// 	{
		// 		Destroy(GetComponent<MeshCollider>());
		// 		MeshCollider collider = gameObject.AddComponent<MeshCollider>();
		// 		collider.sharedMesh = null;
		// 		collider.sharedMesh = mesh;
		// 	}
		// }
		//
		// void MeshCalculate(int vertexIndex, int yOffset)
		// {
		// 	if (generateContinuously)
		// 	{            
		// 		vertices[vertexIndex].z = Mathf.PerlinNoise
		// 		(Time.time    + (vertices[vertexIndex].x + transform.position.x) / detailScaling,
		// 			Time.time + (vertices[vertexIndex].y + transform.position.y)) * yScaling;
		// 		vertices[vertexIndex].z -= yOffset;
		// 	}
		// 	else
		// 	{
		// 		vertices[vertexIndex].z = Mathf.PerlinNoise
		// 		((vertices[vertexIndex].x + transform.position.x) / detailScaling,
		// 			(vertices[vertexIndex].y + transform.position.y)) * yScaling;
		// 		vertices[vertexIndex].z -= yOffset;
		// 	}
		// }
	}
}