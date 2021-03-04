using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public UIPanel[] UIPanels;

    //Reference to the Previously active panel
    private GameObject previousPanel;

    #region OnEnable/OnDisable
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += SwitchPanel;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= SwitchPanel;
    }
    #endregion

    #region UI Switching
    public void SwitchPanel()
    {
        if(previousPanel != null) previousPanel.SetActive(false);

        for (int i = 0; i < UIPanels.Length; i++)
        {
            if (UIPanels[i].stateForUI == GameManager.Instance.gameState)
            {
                previousPanel = UIPanels[i].uiPanel;
                previousPanel.SetActive(true);
                break;
            }
        }
    }
    #endregion
}

#region Structs
[System.Serializable]
public struct UIPanel
{
    //GameObejct Reference to the UIPanel
    public GameObject uiPanel;
    //The State that the UIPanel will be active for
    public GameManager.GameState stateForUI;
}
#endregion