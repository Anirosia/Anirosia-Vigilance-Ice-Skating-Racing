using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { inMenu,inPaused, inRaceMode, inEndlessMode };
    public GameState gameState;


    #region Variables
    //----------------Public----------------
    [Header("Script References")]
    public MenuManager menuManager;
    
    [Header("Level References")]
    public string[] levelNames;

    //----------------Private----------------
    private int[] _distanceLevels = new int[] { 200, 500, 1000 };

    private bool scenesHaveBeenPreloaded = false;
    private bool loadEndlessScene = false;

    public static event Action OnGameStateChanged;
    #endregion

    #region Mutators
    public string GetLevelName
    {
        get { return "LevelOne"; }
    }
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

    #region Start
    void Start()
    {
        //Application Settings
        Application.targetFrameRate = 60;
        // QualitySettings.vSyncCount = 1;
        // When Game starts load the menu:
        ChangeGameState(GameState.inMenu);

        ObjectPool.Instance.InitializePool();
    }
    #endregion

    #region Update
    void Update()
    {
        GameStateManager();
    }

    void GameStateManager() // Use these states to determine the actions of other scripts based on where the player/user is in the game.
    {
        switch (gameState)
        {
            case GameState.inMenu:

                break;
            case GameState.inPaused:

                break;
            case GameState.inRaceMode:
                
                break;
            case GameState.inEndlessMode:
                
                break;
        }
    }
    #endregion

    #region Switching Game States
    public void loadRaceMode()
    {
        // For now this is just to switch UI
        ChangeGameState(GameState.inRaceMode);
    }

    public void loadEndlessMode()
    {
        // For now this is just to switch UI
        ChangeGameState(GameState.inEndlessMode);
        SceneManager.LoadScene("EndlessRunner");
    }

    private void ChangeGameState(GameState state)
    {
        gameState = state;
        OnGameStateChanged?.Invoke();
    }

    #endregion
    
    #region Menu Methods
    //private void PreloadScences()
    //{
    //    if (scenesHaveBeenPreloaded) return;

    //    StartCoroutine(PreloadScenes());


    //    scenesHaveBeenPreloaded = true;
    //}

    //IEnumerator PreloadScenes()
    //{
    //    AsyncOperation operation = SceneManager.LoadSceneAsync("EndlessRunner");
    //    operation.allowSceneActivation = false;

    //    while (!operation.isDone)
    //    {
    //        float progress = Mathf.Clamp01(operation.progress / 0.9f) * 100;
    //        print(progress);
    //        //slider.value = progress;
    //        //text.text = progress + "%";
    //        operation.allowSceneActivation = loadEndlessScene;
    //        yield return null;
    //    }

    //    yield break;
    //}
    #endregion

    public int GetDifficultyLevel(int level) => _distanceLevels[level];
}
