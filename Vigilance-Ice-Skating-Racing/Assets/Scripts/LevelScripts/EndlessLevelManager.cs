using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using LevelScripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class EndlessLevelManager : MonoBehaviour
{
    #region Variables
    [Header("Debug")]
    public bool debug = false;

    [Header("Assignables")]
    public GameObject player;
    public GameObject chunkPrefab;

    [Header("Variables")]
    private int maxChunks = 3;

    private GameObject[] _chunks = new GameObject[2];
    private TerrainChunkGeneration[] _terrainChunkGenerations = new TerrainChunkGeneration[2];
    private Vector3 _removePointLocation;
    private bool _newFlagLocation;
    private const int MAX_DISTANCE_THRESHOLD = 150;


    private GameManager _gameManager;
    //Current Level Index
    private int currentLevel = 0;

    //Distance At Which platforms become harder and more variance is added
    public int[] difficultyDistance = new int[] {200, 500, 1000};
    //Start Position of the Player
    private float _playerStartPosX = 0;
    //If the level position is reset to Zero, add the previous position to account for that in the total distance
    private float _offset = 0;

    //When A Level is Added Callback
    public static event Action<GameObject> OnLevelAdded;
    #endregion

    #region Unity Messages
    private void Awake()=>_gameManager = GameManager.Instance;
    private void Start(){
        _playerStartPosX = player.transform.position.x;
        InitializeLevel();
        StartCoroutine(CheckPlayerPosition());
    }
    private void OnDisable()=>StopAllCoroutines();

    private void Update(){
        //Distance Traveled
        // _gameManager.CurrentDistance = (int)(player.transform.position.x - _playerStartPosX + _offset);
    }
    #endregion

    #region Distance Checks
    IEnumerator CheckPlayerPosition(){
        while (true){
            if(PlayerFromMaxDistance()){
                ResetLevelPosition();
                yield return new WaitForSeconds(.5f);
                _terrainChunkGenerations[1].CreateRemoveFlagPoint();
                _removePointLocation = _terrainChunkGenerations[1].FlagRemovePoint;
                // RemoveExtraChunk();
                // AddChunk();
            }
            if(CheckLevelForRemoval() && _newFlagLocation) RemoveLastChunk();
            yield return new WaitForSeconds(1.5f);
        }
    }

    bool CheckLevelForRemoval()=>player.transform.position.x > _removePointLocation.x;
    bool PlayerFromMaxDistance()=>player.transform.position.magnitude > MAX_DISTANCE_THRESHOLD;
    private void ResetLevelPosition(){
        UnityEngine.Object[] objects = FindObjectsOfType(typeof(Transform));
        _offset += player.transform.position.x;
        foreach (object o in objects){
            Transform t = (Transform)o;
            if(t.parent==null) t.position -= player.transform.position;
        }
    }
    #endregion

    #region Chunk Management
    private void InitializeLevel(){
        _newFlagLocation = false;
        var fistChunk = Instantiate(chunkPrefab);
        fistChunk.SetActive(true);
        fistChunk.transform.position = new Vector3(-10, 0, 0);
        _chunks[0] = fistChunk;
        _terrainChunkGenerations[0] = fistChunk.GetComponent<TerrainChunkGeneration>();
        AddChunk();
    }
    private void AddChunk(){
        Vector3 spawnObject = new Vector3(-1000, -1000, -1000);
        var chunk = Instantiate(chunkPrefab, spawnObject, quaternion.identity);
        _chunks[1] = chunk;
        _terrainChunkGenerations[1] = chunk.GetComponent<TerrainChunkGeneration>();
        StartCoroutine(ConnectChunks());
    }
    private void RemoveLastChunk(){
        var temp = _chunks[0].transform.parent.gameObject;
        _chunks[0] = _chunks[1].gameObject;
        _terrainChunkGenerations[0] = _terrainChunkGenerations[1];
        _chunks[1] = null;
        _terrainChunkGenerations[1] = null;
        Destroy(temp);
        _newFlagLocation = false;
        AddChunk();
    }
    private void RemoveExtraChunk(){
        var temp = _chunks[1].transform.parent.gameObject;
        _chunks[1] = null;
        Destroy(temp);
    }
    private IEnumerator ConnectChunks(){
        yield return new WaitForSeconds(.1f);
        var currentChunk = _terrainChunkGenerations[0];
        var newChunk = _terrainChunkGenerations[1];
        newChunk.StartPoint.position = currentChunk.EndPoint.position;
        yield return new WaitForSeconds(1f);
        newChunk.CreateRemoveFlagPoint();
        _removePointLocation = newChunk.FlagRemovePoint;
        _newFlagLocation = true;
        yield return null;
    }
    #endregion




}
