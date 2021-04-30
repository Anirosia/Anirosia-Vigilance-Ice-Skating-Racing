using System;
using System;
using System.Collections.Generic;
using System.Numerics;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace LevelScripts
{
	public class SurfaceGen : MonoBehaviour
	{
		public GameObject foreground;

		private Mesh m_mesh;
		private List<Vector3[]> m_curves = new List<Vector3[]>();
		private List<Vector3> m_vertices = new List<Vector3>();
		private List<int> m_triangles = new List<int>();
		public int mapLength = 10;
		private List<Vector2> topVeticePoints = new List<Vector2>();
		[ReadOnlyInspector] [SerializeField] private float f = 0;
		[ReadOnlyInspector] [SerializeField] private float c = 0;
		[Range(.5f, 1f)] public float step;
		[SerializeField] int m_resolution = 20;

		private void Start() {
			var filter = GetComponent<MeshFilter>();
			m_mesh = filter.mesh;
			m_mesh.Clear();

			var xPos = 0f;
			for (int c = 0; c < mapLength; c++) {
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

					xPos += step;
					// xPos += 0.5f;
				}

				m_curves.Add(curve);
			}


			foreach (var curve in m_curves) {
				for (int i = 0; i < m_resolution; i++) {
					float t = (float) i / (float) (m_resolution - 1);
					Vector3 p = BézierPoint(t, curve[0], curve[1], curve[2], curve[3]);
					AddTerrainPoint(p, i);
					Debug.Log(i);
				}
			}

			m_mesh.vertices = m_vertices.ToArray();
			m_mesh.triangles = m_triangles.ToArray();

			foreground.GetComponent<MeshFilter>().mesh = m_mesh;

			m_mesh.RecalculateBounds();
			m_mesh.RecalculateNormals();

			EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
			collider.points = topVeticePoints.ToArray();
			
			
		}


		private void AddTerrainPoint(Vector3 point, int a) {
			m_vertices.Add(new Vector3(point.x, -f, 0f));
			point.y -= f;
			m_vertices.Add(point);
			topVeticePoints.Add(point);

			if (m_vertices.Count >= 4) {
				int start = m_vertices.Count - 4;
				m_triangles.Add(start        + 0);
				m_triangles.Add(start        + 1);
				m_triangles.Add(start        + 2);
				m_triangles.Add(start        + 1);
				m_triangles.Add(start        + 3);
				m_triangles.Add(start        + 2);
			}

			if (a != m_resolution - 1) f += 0.05f;
		}


		private Vector3 BézierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			Vector3 a = Vector3.Lerp(p0, p1, t);
			Vector3 b = Vector3.Lerp(p1, p2, t);
			Vector3 c = Vector3.Lerp(p2, p3, t);
			
			Vector3 d = Vector3.Lerp(a, b, t);
			Vector3 e = Vector3.Lerp(b, c, t);

			return Vector3.Lerp(d, e, t);
		}
	}
}