using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { inMenu,inPaused, inRaceMode, inEndlessMode };
    public GameState gameState;

    public MenuManager menuManager;


    void Start()
    {
        gameState = GameState.inMenu;
        menuManager.switchPanel(menuManager.UIPanels[0]);
    }

    
    void Update()
    {
        GameStateManager();
    }

    void GameStateManager()
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

    public void loadRaceMode()
    {
        // For now this is just to switch UI
        gameState = GameState.inRaceMode;
    }

    public void loadEndlessMode()
    {
        // For now this is just to switch UI
        gameState = GameState.inEndlessMode;
    }
}
