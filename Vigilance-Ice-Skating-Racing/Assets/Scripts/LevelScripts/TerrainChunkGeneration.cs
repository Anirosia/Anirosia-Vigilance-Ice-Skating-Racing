using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace LevelScripts
{
    public class TerrainChunkGeneration : MonoBehaviour
    {
        #region Fields
        private List<Vector3[]> _curves = new List<Vector3[]>();
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _topVerticesPoints = new List<Vector2>();

        List<Vector2> _obstacleSpawnPoints = new List<Vector2>();
        List<Vector2> _treeSpawnPoints = new List<Vector2>();
        List<Vector2> _coinSpawnPoints = new List<Vector2>();

        [SerializeField] private GameObject foreground;
        private Mesh _mesh;

        //this also works as how you want the angle of the slope to be
        [Range(.5f, 1f)] public float step = 0.5f;
        private float _stepHelper;

        [SerializeField] int resolution = 20;
        [SerializeField] int mapLength = 10;

        [SerializeField] private GameObject[] obstacles;
        [SerializeField] private GameObject[] trees;
        [SerializeField] private GameObject coinGameObject;

        [SerializeField] private int obstaclePerChunk = 5;
        [SerializeField] private int treesPerChunk = 5;
        [SerializeField] private int coinPerChunk = 5;

        [SerializeField] private LayerMask layerMask;
        RaycastHit2D _hit = new RaycastHit2D();

        [Header("Debug")]
        public bool removeObjects;
  #endregion

        #region Properties
        public Transform StartPoint { get; private set; }
        public Transform EndPoint { get; private set; }
        public Vector3 FlagRemovePoint { get; private set; }
  #endregion

        #region Start Methods
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
            if(!removeObjects){
                SpawnObstacles();
                SpawnTrees();
                SpawnCoins();
            }
        }
  #endregion

        #region Generating Terrain
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
            //creates a corresponding point along the bottom
            _vertices.Add(new Vector3(point.x, -_stepHelper, 0f));

            //then add the top point
            point.y -= _stepHelper - 1.5f;
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
  #endregion

        #region Creating Reference Points
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
  #endregion

        #region Spawing
        private void SpawnObstacles(){

            var baseValue = 0;
            int increaseBy = _topVerticesPoints.Count / obstaclePerChunk;
            var limit = increaseBy;
            for(int i = 0; i < obstaclePerChunk; i++){
                var point = GetRandomPoint(baseValue, limit);
                _obstacleSpawnPoints.Add(point);
                baseValue = limit;
                limit += increaseBy;
            }

            foreach (var spawnPoint in _obstacleSpawnPoints){
                var obstacle = Instantiate(obstacles[Random.Range(0, obstacles.GetLength(0))]);
                _hit = Physics2D.Raycast(obstacle.transform.position, Vector2.down, 2f, layerMask);
                obstacle.transform.SetParent(transform);
                obstacle.transform.localPosition = spawnPoint;
            }
        }
        private void SpawnTrees(){
            var baseValue = 0;
            int increaseBy = _topVerticesPoints.Count / treesPerChunk;
            var limit = increaseBy;
            for(int i = 0; i < treesPerChunk; i++){
                var point = GetRandomPoint(baseValue, limit);
                _treeSpawnPoints.Add(point);
                baseValue = limit;
                limit += increaseBy;
            }

            foreach (var spawnPoint in _treeSpawnPoints){
                var temp = Random.value;
                var tree = Instantiate(trees[Random.Range(0, trees.GetLength(0))]);
                tree.transform.SetParent(transform);
                if(temp > 0.5f) tree.transform.localPosition = new Vector3(spawnPoint.x, spawnPoint.y - .1f, .1f); // back
                else tree.transform.localPosition = new Vector3(spawnPoint.x, spawnPoint.y - 0.75f, -0.1f); // front
            }
        }

        private void SpawnCoins(){
            var baseValue = 0;

            for(int i = 0; i < _topVerticesPoints.Count; i += 20){
                var point = GetRandomPoint(baseValue, i);
                _coinSpawnPoints.Add(point);
                baseValue = i;
            }

            foreach (var spawnPoint in _coinSpawnPoints){
                var coin = Instantiate(coinGameObject);
                coin.transform.SetParent(transform);
                coin.transform.localPosition = new Vector3(spawnPoint.x, spawnPoint.y + .5f); // back

            }
        }
  #endregion

#region Helper Methods
        Vector2 GetRandomPoint(int min, int max)=>_topVerticesPoints[Random.Range(min, max)];
        private Vector3 BézierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
            Vector3 a = Vector3.Lerp(p0, p1, t);
            Vector3 b = Vector3.Lerp(p1, p2, t);
            Vector3 c = Vector3.Lerp(p2, p3, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(d, e, t);
        }
  #endregion


    }
}
