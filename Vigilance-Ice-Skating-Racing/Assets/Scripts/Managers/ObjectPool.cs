using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Variables
    public bool debug = false;
    
    [Header("GameObjects to be Pooled")] public GameObjectToBePooled[] gameObjectsToBePooled;
    [Header("Levels to be Pooled")] public LevelInformation[] levelsToBePooled;

    private List<GameObject> _pooledGameObjects = new List<GameObject>();
    private List<GameObject> _pooledLevels = new List<GameObject>();
    private List<GameObject> _levelPrefabs = new List<GameObject>();

    private float _debugTimer;

    private bool _isInitialized = false;

    #endregion

    #region Singleton
    //Singleton Instantiation
    public static ObjectPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(this);
    }
    #endregion

    #region Initialize Pool
    public void InitializePool()
    {
       Log("Pooling has started"); 
        _debugTimer = Time.realtimeSinceStartup;

        if (_levelPrefabs.Count == 0)
        {
            LoadAssets();
        }

        StartCoroutine(LoadGameObjects());
        StartCoroutine(LoadLevels());

        _isInitialized = true;
    }

    IEnumerator LoadGameObjects()
    {
        for (int i = 0; i < gameObjectsToBePooled.Length; i++)
        {
            for (int j = 0; j < gameObjectsToBePooled[i].amountToBePooled; j += 10)
            {
                for (int k = 0; k < 10; k++)
                {
                    GameObject go = Instantiate(gameObjectsToBePooled[i].gameObjectToBePooled);
                    go.SetActive(false);
                    go.transform.SetParent(transform);
                    _pooledGameObjects.Add(go);
                }
                yield return 0;
            }
        }
        
        Log("Pooling has ended, " + _pooledGameObjects.Count + " Pooled Objects");
        Log("Generating Pool Took " + (Time.realtimeSinceStartup - _debugTimer));
        yield return null;
    }
    IEnumerator LoadLevels()
    {
        for (int i = 0; i < _levelPrefabs.Count; i++)
        {
            GameObject go = Instantiate(_levelPrefabs[i]);
            go.SetActive(false);
            go.transform.SetParent(transform);
            _pooledLevels.Add(go);
            yield return 0;
        }

        Log("Level Pooling has ended, " + _pooledLevels.Count + " Pooled Levels");
        Log("Generating Pool Took " + (Time.realtimeSinceStartup - _debugTimer));
        yield return null;
    }
    public void LoadAssets()
    {
        int currentLevelIndex = 0;
        for (int i = 0; i < levelsToBePooled.Length; i++)
        {
            var loadedLevelPrefabs = Resources.LoadAll<GameObject>(levelsToBePooled[i].folderName);

            int count = 0;
            foreach (var item in loadedLevelPrefabs)
            {
                _levelPrefabs.Add(item);
                count++;
            }

            levelsToBePooled[i].levelIndex = currentLevelIndex;
            levelsToBePooled[i].amountOfLevels = count;

            currentLevelIndex++;
        }
    }
    #endregion

    #region Public Functions
    #region Set in Pool
    public void SetObjectInPool(GameObject go)
    {
        go.SetActive(false);
        go.transform.position = Vector3.zero;
    }
    #endregion

    #region Get from Pool
    public GameObject GetObjectFromPool(string name)
    {
        if (!_isInitialized) InitializePool();

        int startPosInList = 0;
        int index = 0;
        for (int i = 0; i < gameObjectsToBePooled.Length; i++)
        {
            if (gameObjectsToBePooled[i].name != name)
            {
                startPosInList += gameObjectsToBePooled[i].amountToBePooled;
            }
            else
            {
                index = i;
                break;
            }
        }

        for (int i = startPosInList; i < startPosInList + gameObjectsToBePooled[index].amountToBePooled; i++)
        {
            if (!_pooledGameObjects[i].activeSelf) return _pooledGameObjects[i];
        }

        Log("No Objects Ready in Pool " + gameObjectsToBePooled[index].name);

        if (gameObjectsToBePooled[index].loadMoreIfNoneLeft)
        {
            Log("Added Object in Pool " + gameObjectsToBePooled[index].name);
            _pooledGameObjects.Insert(startPosInList + gameObjectsToBePooled[index].amountToBePooled, gameObjectsToBePooled[index].gameObjectToBePooled);
            gameObjectsToBePooled[index].amountToBePooled++;
            return _pooledGameObjects[startPosInList + gameObjectsToBePooled[index].amountToBePooled - 1];
        }

        LogError("No Objects Left in Pool");
        return null;
    }
    #endregion
    #endregion
    #region Logging Functions
    private void Log(string msg)
    {
        if (!debug) return;

        Debug.Log("[OBJECTPOOL]: " + msg);
    }
    private void LogWarning(string msg)
    {
        Debug.LogWarning("[OBJECTPOOL]: " + msg);
    }
    private void LogError(string msg)
    {
        Debug.LogError("[OBJECTPOOL]: " + msg);
    }
    #endregion
}

#region Structs 
[System.Serializable]
public struct GameObjectToBePooled
{
    [Header("Object Info")]
    public string name;
    public int amountToBePooled;
    public GameObject gameObjectToBePooled;

    [Header("Settings")]
    public bool loadMoreIfNoneLeft;
}

[System.Serializable]
public struct LevelInformation
{
    [Header("Level Info")]
    /// <summary>
    /// Folder Name located under the Assets/Resources/folderName
    /// </summary>
    public string folderName;
    [HideInInspector] public int levelIndex;
    [HideInInspector] public int listIndex;
    [HideInInspector] public int amountOfLevels;
    [HideInInspector] public int amountOfLoadedLevels;

    //[Header("Settings")]
    //public bool loadMoreIfNoneLeft;
}
#endregion