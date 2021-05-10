using System;
using System.Collections;
using System.Collections.Generic;
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


    private List<GameObject> _chunks = new List<GameObject>();
    private Vector3 _removePointLocation;
    private bool _newFlagLocation;
    private const int MAX_DISTANCE_THRESHOLD = 500;

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
    private void Start(){
        _playerStartPosX = player.transform.position.x;
        InitializeLevel();
        StartCoroutine(CheckPlayerPosition());
        StartCoroutine(DistanceTraveled());
    }
    private void OnDisable()=>StopAllCoroutines();
    #endregion

    #region Distance Checks
    IEnumerator DistanceTraveled(){
        var gm = GameManager.Instance;
        while (true){
            gm.CurrentDistance =(int)(player.transform.position.x - _playerStartPosX + _offset);
            yield return null;
        }
    }
    IEnumerator CheckPlayerPosition(){
        while (true){
            if(PlayerFromMaxDistance()){
                ResetLevelPosition();
                AddChunk();
            }
            if(CheckLevelForRemoval() && _newFlagLocation) RemoveLastChunk();
            yield return new WaitForSeconds(1.5f);
            CleanUpExtra();
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
        _chunks.Add(fistChunk);
        AddChunk();
    }
    private void AddChunk(){
        Vector3 spawnObject = new Vector3(-1000, -1000, -1000);
        var chunk = Instantiate(chunkPrefab, spawnObject, quaternion.identity);
        _chunks.Add(chunk);
        StartCoroutine(ConnectChunks());
    }
    private void RemoveLastChunk(){
        RemoveChunk();
        _newFlagLocation = false;
        AddChunk();
    }
    private void CleanUpExtra(){
        if(_chunks.Count > 2) RemoveChunk(2);
    }
    private void RemoveChunk(int chunkIndex = default){
        var temp = _chunks[chunkIndex].transform.parent.gameObject;
        _chunks.RemoveAt(chunkIndex);
        Destroy(temp);
    }
    private IEnumerator ConnectChunks(){
        yield return new WaitForSeconds(.1f);
        var currentChunk = _chunks[0].GetComponent<TerrainChunkGeneration>();
        var newChunk = _chunks[1].GetComponent<TerrainChunkGeneration>();
        newChunk.StartPoint.position = currentChunk.EndPoint.position;
        yield return new WaitForSeconds(1f);
        newChunk.CreateRemoveFlagPoint();
        _removePointLocation = newChunk.FlagRemovePoint;
        _newFlagLocation = true;
        yield return null;
    }
    #endregion




}
