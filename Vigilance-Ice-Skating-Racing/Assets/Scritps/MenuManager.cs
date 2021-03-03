using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject[] UIPanels;
    /* 
        0 = Menu
        1 = Settings
        2 = Controls
        3 = InGame
        4 = Pause
        5 = LevelComplete
    */
    

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void switchPanel(GameObject panel) // Sets all panels to false before activating the passed in panel.
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        panel.SetActive(true);
    }

    public void backFromSettings()
    {
        if (gameManager.gameState == GameManager.GameState.inMenu) // From Menu > Settings; to back to menu
        {
            switchPanel(UIPanels[0]); 
        }
        else // From InGame > Settings; to back to inGame
        {
            switchPanel(UIPanels[3]);
        }
    }
}
