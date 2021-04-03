using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool debug = false;

    public enum GameState { inMenu,inPaused, inEndlessMode, Results, Dead };
    [ReadOnlyInspector] public GameState gameState;

    #region Variables
    [Header("Script References")]
    public MenuManager menuManager;    
    
    [Header("Level References")]
    public string[] levelNames;
    public int[] costOfLevels;

    private bool scenesHaveBeenPreloaded = false;
    private bool loadEndlessScene = false;

    private int _currentDistance = 0;

    private int _currentLevel = 0;

    public static event Action OnGameStateChanged;

    #endregion

    #region Mutators
    public int CurrentDiffucultyIndex { get { return _currentLevel; } }
    public string CurrentLevelFolderName { get { return levelNames[_currentLevel]; } }
    public int CurrentDistance { get { return _currentDistance; } set { _currentDistance = value; } }
    #endregion

    #region Singleton
    //Singleton Instantiation
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(this);
    }
    #endregion

    #region Unity Messages
    void Start()
    {
        //Application Settings
        Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 1;
        // When Game starts load the menu:
        ChangeGameState(GameState.inMenu);
        
        ObjectPool.Instance.InitializePool();
        
        LoadSave();
    }

    private void OnEnable()
    {
        OnGameStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        OnGameStateChanged -= OnStateChanged;
    }
    void Update()
    {
        GameStateManager();
    }
    #endregion

    #region Update
    void GameStateManager() // Use these states to determine the actions of other scripts based on where the player/user is in the game.
    {
        switch (gameState)
        {
            case GameState.inMenu:

                break;
            case GameState.inPaused:

                break;
            case GameState.inEndlessMode:
                
                break;
            case GameState.Dead:
                OnDeath();
                break;
            case GameState.Results:
                
                break;
        }
    }
    #endregion

    #region Game States
    public void OnDeath()
    {
        //Save Score
        ChangeGameState(GameState.Results);
    }
    public void OnStateChanged()
    {
        switch (gameState)
        {
            case GameState.inMenu:
                AudioManager.Instance.PlayAudio(AudioTypes.TRACK_MENU);
                break;
            case GameState.inPaused:
                break;
            case GameState.inEndlessMode:
                AudioManager.Instance.PlayAudio(AudioTypes.TRACK_GAME);
                break;
            case GameState.Results:
                break;
            case GameState.Dead:
                AudioManager.Instance.PlayAudio(AudioTypes.SFX_GAMEOVER);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Switching Game States

    public void loadEndlessMode(int i)
    {
        _currentLevel = i;
        ChangeGameState(GameState.inEndlessMode);
        SceneManager.LoadScene("EndlessRunner");
    }

    private void ChangeGameState(GameState state)
    {
        gameState = state;
        OnGameStateChanged?.Invoke();
    }

    #endregion

    #region Game Events
    public void OnCoinCollected()
    {
        StatsAndAchievements.Coins++;
    }
    #endregion

    #region Saving and Loading
    public void Save()
    {
        DataManager.SaveData(StatsAndAchievements.GetSaveData());
    }

    private void LoadSave()
    {
        if (!DataManager.LoadData()) { LogError("Load File was unable to be read"); return; }
        
        Log("Save has been loaded");
    }

    /// <summary>
    /// Permanently Deletes Save Files
    /// </summary>
    public void ResetSave()
    {
        DataManager.ResetData();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
    #endregion

    #region Logging Functions
    private void Log(string msg)
    {
        if (!debug) return;

        Debug.Log("[GAMEMANAGER]: " + msg);
    }
    private void LogWarning(string msg)
    {
        Debug.LogWarning("[GAMEMANAGER]: " + msg);
    }
    private void LogError(string msg)
    {
        Debug.LogError("[GAMEMANAGER]: " + msg);
    }
    #endregion
}
