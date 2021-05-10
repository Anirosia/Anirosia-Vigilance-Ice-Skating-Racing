using System;
using System.Collections;
using System.Collections.Generic;
using LevelScripts;
using Unity.Mathematics;
using UnityEngine;

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
    private const int MAX_DISTANCE = 50000;

    //Platforms Generated
    private int platformsGenerated = 0;
    //Current Level Index
    private int currentLevel = 0;

    //Distance At Which platforms become harder and more variance is added
    public int[] difficultyDistance = new int[] {200, 500, 1000};
    //List of Active Chunks
    private List<GameObject> activeChunks = new List<GameObject>();
    //Start Position of the Player
    private float playerStartPosX = 0;
    //If the level position is reset to Zero, add the previous position to account for that in the total distance
    private float offset = 0;
    [SerializeField] private Vector3 _removePointLocation;
    
    private bool _newFlagLocation;

    //When A Level is Added Callback
    public static event Action<GameObject> OnLevelAdded;
    #endregion

    #region Unity Messages
    private void Start(){
        InitializeLevel();
        StartCoroutine(CheckPlayerPosition());
    }
    private void Update(){
        // GameManager.Instance.CurrentDistance = (int)(player.transform.position.x - playerStartPosX + offset);
    }

    private void OnDisable()=>StopAllCoroutines();
    #endregion

    #region Distance Checks
    IEnumerator CheckPlayerPosition(){
        while (true){
            if(PlayerFromMaxDistance()){
                //reset level and player position
            }
            if(CheckLevelForRemoval() && _newFlagLocation) RemoveChunk();
            yield return new WaitForSeconds(1.5f);
        }
    }

    bool CheckLevelForRemoval()=>player.transform.position.x > _removePointLocation.x;

    bool PlayerFromMaxDistance(){
        if(player.transform.position.x >= MAX_DISTANCE) return true;
        if(player.transform.position.y >= MAX_DISTANCE) return true;
        if(player.transform.position.z >= MAX_DISTANCE) return true;

        return false;
    }
    private void ResetLevelPosition(){
        //Reset All Platforms Positions to 0
        platformsGenerated = maxChunks;
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
    private void RemoveChunk(){
        var temp = _chunks[0].transform.parent.gameObject;
        Destroy(temp);
        _chunks.RemoveAt(0);
        _newFlagLocation = false;
        AddChunk();
    }
    private IEnumerator ConnectChunks(){
        yield return new WaitForSeconds(.1f);
        var currentChunk = _chunks[0].GetComponent<TerrainGeneration>();
        var newChunk = _chunks[1].GetComponent<TerrainGeneration>();
        newChunk.StartPoint.position = currentChunk.EndPoint.position;
        yield return new WaitForSeconds(1f);
        _removePointLocation = newChunk.FlagRemovePoint;
        _newFlagLocation = true;
        yield return null;
    }
    #endregion




}
