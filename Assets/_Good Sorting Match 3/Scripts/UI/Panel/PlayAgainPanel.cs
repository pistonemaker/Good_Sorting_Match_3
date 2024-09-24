using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainPanel : BasePanel
{
    public TextMeshProUGUI levelText;

    public Button closeButton;
    public Button playButton;
    public Button playWithAllBoosterButton;
    
    public LevelData levelData;

    protected override void OnEnable()
    {
        levelData = GameController.Instance.levelData;
        levelData.isUseHammer = false;
        levelData.isUseClock = false;
        levelData.isUseDoubleStar = false;
        base.OnEnable();
        LoadData();
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
        playWithAllBoosterButton.onClick.RemoveAllListeners();
    }

    protected override void LoadButtonAndImage()
    {
        levelText = transform.Find("Ribbon").GetChild(0).GetComponent<TextMeshProUGUI>();
        closeButton = transform.Find("Ribbon").Find("Close Button").GetComponent<Button>();
        playButton = transform.Find("Play Button").GetComponent<Button>();
        playWithAllBoosterButton = transform.Find("Play With All Booster Button").GetComponent<Button>();
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(() =>
        {
            ClosePanel(0f);
            SceneManager.LoadSceneAsync("Home");
        });
        
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync("Game");
        });
        
        playWithAllBoosterButton.onClick.AddListener(() =>
        {
            levelData.isUseHammer = true;
            levelData.isUseClock = true;
            levelData.isUseDoubleStar = true;
            SceneManager.LoadSceneAsync("Game");
        });
    }

    public void LoadData()
    {
        levelText.text = "Level " + (PlayerPrefs.GetInt(DataKey.Cur_Level) + 1);
    }
}