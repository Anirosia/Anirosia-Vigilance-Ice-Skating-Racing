using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Image References")]
    public Sprite audioOnSprite;
    public Sprite audioOffSprite;
    public Image musicToggleSprite;
    public Image sfxToggleSprite;

    [Header("UI References")]
    public GameObject raceModePanel;
    public GameObject endlessModePanel;

    //----------------Private----------------
    private int[] _distanceLevels = new int[] { 200, 500, 1000 };

    private bool scenesHaveBeenPreloaded = false;
    private bool loadEndlessScene = false;

    public static event Action OnGameStateChanged;

    private bool musicOn;
    private bool sfxOn;
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
        musicOn = true;
        sfxOn = true;

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
    public void loadRaceMode(int mapNumber)
    {
        // For now this is just to switch UI
        ChangeGameState(GameState.inRaceMode);
        SceneManager.LoadScene("Map-" + mapNumber + "-Level-1");
        raceModePanel.SetActive(true);
    }

    public void loadEndlessMode()
    {
        // For now this is just to switch UI
        ChangeGameState(GameState.inEndlessMode);
        SceneManager.LoadScene("EndlessRunner");
        endlessModePanel.SetActive(true);
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

    public void MusicToggle()
    {
        if (musicOn) // turn music off and switch sprite
        {
            //audioManager.StopAudio()
            musicToggleSprite.sprite = audioOffSprite;
            musicOn = false;
        }
        else // turn music on and switch sprite
        {
            //audioManager.PlayAudio()
            musicToggleSprite.sprite = audioOnSprite;
            musicOn = true;
        }
    }

    public void SFXToggle()
    {
        if (sfxOn)
        {
            //audioManager.StopAudio()
            sfxToggleSprite.sprite = audioOffSprite;
            sfxOn = false;
        }
        else
        {
            //audioManager.PlayAudio()
            sfxToggleSprite.sprite = audioOnSprite;
            sfxOn = true;
        }
    }
    #endregion

    public int GetDifficultyLevel(int level) => _distanceLevels[level];
}
