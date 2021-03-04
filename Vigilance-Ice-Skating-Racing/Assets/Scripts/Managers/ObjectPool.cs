using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Variables
    public bool debug = false;
    
    [Header("GameObjects to be Pooled")] public GameObjectToBePooled[] gameObjectsToBePooled;
    [Header("Levels to be Pooled")] public LevelInformation[] levelsToBePooled;

    private List<GameObject> pooledGameObjects = new List<GameObject>();
    private List<GameObject> pooledLevels = new List<GameObject>();
    private List<GameObject> levelPrefabs = new List<GameObject>();

    private float debugTimer;

    private int previousLevel;

    private bool isInitialized = false;

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
        if (debug) { print("Pooling has started"); debugTimer = Time.realtimeSinceStartup; }

        if (levelPrefabs.Count == 0)
        {
            LoadAssets();
        }

        StartCoroutine(LoadGameObjects());
        StartCoroutine(LoadLevels());

        isInitialized = true;
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
                    pooledGameObjects.Add(go);
                }
                yield return 0;
            }
        }
        if (debug) { print("Pooling has ended, " + pooledGameObjects.Count + " Pooled Objects"); print("Generating Pool Took " + (Time.realtimeSinceStartup - debugTimer)); }
        yield return null;
    }
    IEnumerator LoadLevels()
    {
        for (int i = 0; i < levelPrefabs.Count; i++)
        {
            GameObject go = Instantiate(levelPrefabs[i]);
            go.SetActive(false);
            go.transform.SetParent(transform);
            pooledLevels.Add(go);
            yield return 0;
        }
        if (debug) { print("Level Pooling has ended, " + pooledLevels.Count + " Pooled Levels"); print("Generating Pool Took " + (Time.realtimeSinceStartup - debugTimer)); }
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
                levelPrefabs.Add(item);
                count++;
            }

            levelsToBePooled[i].levelIndex = currentLevelIndex;
            levelsToBePooled[i].amountOfLevels = count;

            currentLevelIndex++;
        }
    }
    #endregion

    #region Get from Pool
    public GameObject GetObjectFromPool(string name)
    {
        if (!isInitialized) InitializePool();

        int startPosInList = -1;
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

        if (debug) print("Start Position in List is " + startPosInList);

        for (int i = startPosInList; i < startPosInList + gameObjectsToBePooled[index].amountToBePooled; i++)
        {
            if (!pooledGameObjects[i].activeSelf) return pooledGameObjects[i];
        }

        if (debug) print("No Objects Ready in Pool " + gameObjectsToBePooled[index].name);

        if (gameObjectsToBePooled[index].loadMoreIfNoneLeft)
        {
            if (debug) print("Added Object in Pool " + gameObjectsToBePooled[index].name);
            pooledGameObjects.Insert(startPosInList + gameObjectsToBePooled[index].amountToBePooled, gameObjectsToBePooled[index].gameObjectToBePooled);
            gameObjectsToBePooled[index].amountToBePooled++;
            return pooledGameObjects[startPosInList + gameObjectsToBePooled[index].amountToBePooled - 1];
        }
        return null;
    }

    public GameObject GetLevelFromPool(int currentLevel)
    {
        if (!isInitialized) InitializePool();
        string name = GameManager.Instance.GetLevelName;
        int startPosInList = -1;
        int index = 0;

        for (int i = 0; i < pooledLevels.Count; i++)
        {
            if (!levelsToBePooled[i].folderName.StartsWith(name))
            {
                startPosInList += levelsToBePooled[i].amountOfLevels;
            }
            else
            {
                startPosInList = 0;
                index = i;
                break;
            }
        }

        if (debug) print("Start Position in List is " + startPosInList);

        if (startPosInList == -1)
        {
            print("Error in the pool, probably not Initialized");
            return null;
        }

        for (int i = 0; i < 10; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pooledLevels.Count);

            if (!pooledLevels[randomIndex].activeSelf)
            {
                //Apply Variance
                return pooledLevels[randomIndex];
            }
        }

        for (int i = 0; i < pooledLevels.Count; i++)
        {
            if (!pooledLevels[i].activeSelf)
            {
                //Apply Variance
                return pooledLevels[i];
            }
        }
        

        return null;
            
    }
    #endregion

    #region Set in Pool
    public void SetObjectInPool(GameObject go)
    {
        go.SetActive(false);
    }
    public void SetLevelInPool(GameObject level)
    {
        //Transform variance = level.transform.GetChild(1);
        //Make Variance in level false
        //for (int i = 0; i < level.transform.GetChild(1).childCount; i++)
        //{
            //level.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        //}
        level.SetActive(false);
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
    public int levelIndex;
    [HideInInspector] public int listIndex;
    [HideInInspector] public int amountOfLevels;
    [HideInInspector] public int amountOfLoadedLevels;

    //[Header("Settings")]
    //public bool loadMoreIfNoneLeft;
}
#endregion