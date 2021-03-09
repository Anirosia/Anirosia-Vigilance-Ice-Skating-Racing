using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelManager : MonoBehaviour
{
    #region Variables
    [Header("Debug")]
    public bool debug = false;

    [Header("Assignables")]
    public GameObject player;

    [Header("Variables")]
    [SerializeField] private int maxChunks = 3;

    //Platforms Generated
    private int platformsGenerated = 0;
    //Current Level Index
    private int currentLevel = 0;
    //Distance At Which platforms become harder and more variance is added
    public int[] difficultyDistance = new int[] { 200, 500, 1000 };
    //List of Active Chunks
    private List<GameObject> activeChunks = new List<GameObject>();
    //When A Level is Added Callback
    public static Action<GameObject> OnLevelAdded;
    #endregion

    private void Start()
    {
        InitializeLevels();
    }

    private void FixedUpdate()
    {
        CalculateLevelDifficulty();
        CalculateChunks();
        CheckLevelPosition();
    }

    private void CheckLevelPosition()
    {
        if (activeChunks.Count > 0 && activeChunks[0].transform.position.x > 1000) ResetLevelPosition();
    }

    private void ResetLevelPosition()
    {
        //Reset All Platforms Positions to 0
        platformsGenerated = maxChunks;
    }

    private void CalculateLevelDifficulty()
    {
        if (player.transform.position.x > GameManager.Instance.GetDifficultyLevel(currentLevel)) currentLevel++;
    }

    private void CalculateChunks()
    {
        if (player.transform.position.x > activeChunks[activeChunks.Count - 1].transform.position.x - 20)
        {
            AddLevel();
        }

        if (player.transform.position.x > activeChunks[1].transform.position.x + 10)
        {
            RemoveLevel();
        }
    }

    private void RemoveLevel()
    {
        ObjectPool.Instance.SetLevelInPool(activeChunks[0]);
        activeChunks.RemoveAt(0);
    }
    
    private void AddLevel()
    {
        GameObject level = GetNewLevel();
        level.transform.position = activeChunks[activeChunks.Count - 1].GetComponent<EndlessLevel>().nextLevelTransform.position;
        level.SetActive(true);
        activeChunks.Add(level);
        platformsGenerated++;
        OnLevelAdded?.Invoke(level);
    }

    private void InitializeLevels()
    {
        GameObject level = GetNewLevel();
        level.transform.position = Vector3.zero;
        level.SetActive(true);
        activeChunks.Add(level);
        platformsGenerated++;
        OnLevelAdded?.Invoke(level);
        AddLevel();
        AddLevel();
    }

    private GameObject GetNewLevel() => ObjectPool.Instance.GetLevelFromPool(currentLevel);
}