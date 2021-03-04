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
    [SerializeField] private int maxChunks = 4;
    [SerializeField] private float prefabLength = 13;
    [SerializeField] private int platformsGenerated = 0;
    

    private float _speed = 5;

    private float CurrentLevelSpeed
    {
        get { return _speed * (1 * (currentLevel / 2)); }
    }


    //Current Level Index
    private int currentLevel = 0;

    public int[] difficultyDistance = new int[] { 200, 500, 1000 };

    //List of Active Chunks
    private List<GameObject> activeChunks = new List<GameObject>();
    #endregion

    private void Start()
    {
        AddLevel();
        AddLevel();
        AddLevel();
    }

    void FixedUpdate()
    {
        CalculateLevelDifficulty();
        CalculateChunks();
        CheckLevelPosition();
    }

    public void CheckLevelPosition()
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
        if (player.transform.position.x > prefabLength * (platformsGenerated - 2) + 3)
        {
            AddLevel();
            RemoveLevel();
        }
    }

    private void RemoveLevel()
    {
        if (activeChunks.Count > maxChunks)
        {
            ObjectPool.Instance.SetLevelInPool(activeChunks[0]);
            activeChunks.RemoveAt(0);
        }
    }

    private void AddLevel()
    {
        GameObject level = ObjectPool.Instance.GetLevelFromPool(currentLevel);
        level.transform.position = new Vector3(prefabLength * platformsGenerated, 0, 0);
        level.SetActive(true);
        activeChunks.Add(level);
        platformsGenerated++;
    }

    private GameObject GetNewLevel() => ObjectPool.Instance.GetLevelFromPool(currentLevel);
}
