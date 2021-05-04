using System.Collections.Generic;
using UnityEngine;
namespace LevelScripts
{
    public class ChunkGeneration : MonoBehaviour
    {
        private Mesh _mesh;

        //Bézier curve
        [Range(0, 50)]
        public int value;
        private Vector3[] _curve;

        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();

        private int _resolution = 20;

        void Start(){
            var filter = GetComponent<MeshFilter>();
            _mesh = filter.mesh;
            _mesh.Clear();
            _curve = new Vector3[value];

            var xP = 0f;
            for(int i = 0; i < _curve.Length; i++){
                var point = new Vector3(xP, Random.Range(1f, 2f), 0);
                _curve[i] = point;
                // AddTerrainPoint(point, i);
                // xP += .5f;
                xP++;
            }

            for(int i = 0; i < _resolution; i++){
                float t = (float)i / (float)(_resolution - 1);
                Vector3 p = BézierPoint(t, _curve[0], _curve[1], _curve[2], _curve[3]);
                AddTerrainPoint(p, i);
            }

            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();
        }

        private void AddTerrainPoint(Vector3 point, int a){
            _vertices.Add(new Vector3(point.x, Random.Range(-.5f, .5f), 0f));
            _vertices.Add(point);
            if(_vertices.Count >= 8){
                int start = _vertices.Count - 8;
                _triangles.Add(start + 0);
                _triangles.Add(start + 1);
                _triangles.Add(start + 2);
                _triangles.Add(start + 1);
                _triangles.Add(start + 3);
                _triangles.Add(start + 2);
            }
        }

        private Vector3 BézierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * t * p2;
            p += ttt * p3;


            return p;
        }
    }
}
