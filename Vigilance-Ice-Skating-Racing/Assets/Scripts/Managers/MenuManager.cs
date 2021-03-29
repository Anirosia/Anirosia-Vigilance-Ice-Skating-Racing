using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region  Variables
    public UIPanel[] UIPanels;

    [Header("Image References")]
    public Sprite audioOnSprite;
    public Sprite audioOffSprite;
    public Image musicToggleSprite;
    public Image sfxToggleSprite;

    [Header("UI References")]
    public Text endlessDistanceText;
    public Text endlessCoinsText;
    public Text resultsDistanceText;
    public Text resultsCoinsText;

    //Reference to the Previously active panel
    private GameObject previousPanel;
    private bool _musicOn = true;
    private bool _sfxOn = true;

    #endregion

    #region Unity Messages
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += SwitchPanel;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= SwitchPanel;
    }
    public void Update()
    {
        switch (GameManager.Instance.gameState)
        {
            case GameManager.GameState.inMenu:
                break;
            case GameManager.GameState.inPaused:
                break;
            case GameManager.GameState.inEndlessMode:
                endlessDistanceText.text = "Dist: " + GameManager.Instance.CurrentDistance;
                endlessCoinsText.text = "Coins: " + GameManager.Instance.CurrentCoins;
                break;
            case GameManager.GameState.Results:
                resultsDistanceText.text = "Dist: " + GameManager.Instance.CurrentDistance;
                resultsCoinsText.text = "Coins: " + GameManager.Instance.CurrentCoins;
                break;
            case GameManager.GameState.Dead:
                break;
        }
    }
    #endregion

    #region UI Switching
    public void SwitchPanel()
    {
        if (previousPanel != null) previousPanel.SetActive(false);

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

    #region Audio
    public void MusicToggle()
    {
        if (_musicOn) // turn music off and switch sprite
        {
            _musicOn = false;
            musicToggleSprite.sprite = audioOffSprite;

        }
        else // turn music on and switch sprite
        {
            _musicOn = true;
            musicToggleSprite.sprite = audioOnSprite;
        }
        AudioManager.Instance.MuteTrack(AudioTrackType.Music, _musicOn);
    }

    public void SFXToggle()
    {
        if (_sfxOn)
        {
            _sfxOn = false;
            sfxToggleSprite.sprite = audioOffSprite;
        }
        else
        {
            _sfxOn = true;
            sfxToggleSprite.sprite = audioOnSprite;
        }
        AudioManager.Instance.MuteTrack(AudioTrackType.SFX, _sfxOn);
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