using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayPanel : BasePanel
{
    public TextMeshProUGUI levelText;
    public WinStreak winStreak;

    public Button closeButton;
    public Button playButton;
    public Button playWithAllBoosterButton;
    
    public LevelData levelData;

    protected override void OnEnable()
    {
        levelData = HomeManager.Instance.levelData;
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
        winStreak = transform.Find("Win Streak").GetComponent<WinStreak>();
        playButton = transform.Find("Play Button").GetComponent<Button>();
        playWithAllBoosterButton = transform.Find("Play With All Booster Button").GetComponent<Button>();
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(() =>
        {
            ClosePanel(0f);
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
        levelText.text = HomeManager.Instance.levelText.text;
        winStreak.SetWinStreak();
    }
}
