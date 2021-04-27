using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
	private Mesh m_mesh;

	//Bézier curve
	private Vector3[] m_curve = new Vector3[4];

	private List<Vector3> m_vertices = new List<Vector3>();
	private List<int> m_triangles = new List<int>();

	private int m_resolution = 20;

	void Start() {
		var filter = GetComponent<MeshFilter>();
		m_mesh = filter.mesh;
		m_mesh.Clear();


		var xP = 0f;
		for (int i = 0; i < m_curve.Length; i++) {
			var point = new Vector3(xP, Random.Range(1f, 2f), 0);
			m_curve[i] = point;
			// AddTerrainPoint(point);
			xP += .5f;
		}

		for (int i = 0; i < m_resolution; i++) {
			float t = (float) i / (float) (m_resolution - 1);
			Vector3 p = BézierPoint(t, m_curve[0], m_curve[1], m_curve[2], m_curve[3]);
			AddTerrainPoint(p);
		}

		m_mesh.vertices = m_vertices.ToArray();
		m_mesh.triangles = m_triangles.ToArray();
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
		float u = 1 - t;
		float tt = t   * t;
		float uu = u   * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		p += 3   * uu * t * p1;
		p += 3   * u  * t * p2;
		p += ttt * p3;


		return p;
	}
}