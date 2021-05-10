using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace LevelScripts
{
    public class TerrainGeneration : MonoBehaviour
    {
        private List<Vector3[]> _curves = new List<Vector3[]>();
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _topVerticesPoints = new List<Vector2>();

        public GameObject foreground;
        private Mesh _mesh;

        [Range(.5f, 1f)] public float step = 0.5f;
        [SerializeField] int resolution = 20;
        [SerializeField] int mapLength = 10;

        private float _stepHelper;

        public Transform StartPoint { get; private set; }
        public Transform EndPoint { get; private set; }
        public Vector3 FlagRemovePoint { get; private set; }
        private void Start()=>CreateMesh();
        private void CreateMesh(){
            var filter = GetComponent<MeshFilter>();
            _mesh = filter.mesh;
            _mesh.Clear();

            GenerateCurveConnections();
            DrawConnections();

            _mesh.vertices = _vertices.ToArray();
            _mesh.triangles = _triangles.ToArray();

            foreground.GetComponent<MeshFilter>().mesh = _mesh;
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
            EdgeCollider2D edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider2D.points = _topVerticesPoints.ToArray();

            CreateStartPoint();
            CreateEndPoint();
            // Invoke("CreateRemoveFlagPoint", 0.5f);
            SpawnObstacles();
        }

        private void CreateStartPoint(){
            var firstVertexPosition = _topVerticesPoints[0];
            var startPoint = new GameObject("StartPoint");
            startPoint.transform.SetParent(transform);
            startPoint.transform.localPosition = firstVertexPosition;
            StartPoint = startPoint.transform;

            //this hack corrects the position of the start pivot. 
            startPoint.transform.SetParent(null);
            transform.SetParent(startPoint.transform);

        }
        private void CreateEndPoint(){
            var lastVertexPosition = _topVerticesPoints[_topVerticesPoints.Count - 1];
            var endPoint = new GameObject("EndPoint");
            endPoint.transform.SetParent(transform);
            endPoint.transform.localPosition = lastVertexPosition;
            EndPoint = endPoint.transform;
        }

        public void CreateRemoveFlagPoint(){
            var flag = new GameObject("Flag");
            flag.transform.SetParent(transform);
            flag.transform.localPosition = _topVerticesPoints[50];
            flag.transform.SetParent(null);
            FlagRemovePoint = flag.transform.position;
            Destroy(flag, .1f);
        }


        public GameObject[] obstacles;
        RaycastHit2D _hit = new RaycastHit2D();
        public LayerMask _layerMask;
        private void SpawnObstacles(){
            var obstaclePerChunk = 5;
            List<Vector2> spawnPoints = new List<Vector2>();
            for(int i = 0; i < obstaclePerChunk; i++){
                var point = GetRandomPoint();
                spawnPoints.Add(point);
            }

            foreach (var spawnPoint in spawnPoints){
                var obstacle = Instantiate(obstacles[Random.Range(0, obstacles.GetLength(0))]);


                _hit = Physics2D.Raycast(obstacle.transform.position, Vector2.down, 2f, _layerMask);
                obstacle.transform.SetParent(transform);
                obstacle.transform.localPosition = spawnPoint;
                // obstacle.transform.rotation = ;
            }
        }

        Vector2 GetRandomPoint()=>_topVerticesPoints[Random.Range(5, _topVerticesPoints.Count - 5)];

        private void DrawConnections(){
            _stepHelper = 0f;
            foreach (var curve in _curves){
                for(int i = 0; i < resolution; i++){
                    float t = (float)i / (float)(resolution - 1);
                    Vector3 point = BézierPoint(t, curve[0], curve[1], curve[2], curve[3]);
                    AddTerrainPoint(point, i);
                }
            }
        }

        private void GenerateCurveConnections(){
            var xPos = 0f;
            for(int c = 0; c < mapLength; c++){
                var curve = new Vector3[4];
                for(int i = 0; i < curve.Length; i++){
                    Vector3[] prev = null;
                    if(_curves.Count > 0) prev = _curves[_curves.Count - 1];

                    if(prev!=null && i==0) curve[i] = prev[curve.Length - 1];
                    else if(prev!=null && i==1) curve[i] = 2f * prev[curve.Length - 1] - prev[curve.Length - 2];
                    else curve[i] = new Vector3(xPos, Random.Range(1f, 2f), 0f);
                    xPos += step;
                }
                _curves.Add(curve);
            }
        }

        private void AddTerrainPoint(Vector3 point, int resCount){
            _vertices.Add(new Vector3(point.x, -_stepHelper, 0f));
            point.y -= _stepHelper;
            _vertices.Add(point);
            _topVerticesPoints.Add(point);

            if(_vertices.Count >= 4){
                int start = _vertices.Count - 4;
                _triangles.Add(start + 0);
                _triangles.Add(start + 1);
                _triangles.Add(start + 2);
                _triangles.Add(start + 1);
                _triangles.Add(start + 3);
                _triangles.Add(start + 2);
            }
            if(resCount!=resolution - 1) _stepHelper += 0.05f;
        }

        private Vector3 BézierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(d, e, t);
        }
    }
}
