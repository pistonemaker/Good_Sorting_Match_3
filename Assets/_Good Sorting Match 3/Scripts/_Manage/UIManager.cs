using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI starText;
    public Transform uiStar;

    public int starGain;
    public int starOnUI;
    
    public Button pauseButton;
    
    public PausePanel pausePanel;
    public ExitPanel exitPanel;
    public ExitPanelLostWinStreak exitPanelLostWinStreak;
    public WinStreakPanel winStreakPanel;

    private void SetLevelName()
    {
        var levelData = GameController.Instance.levelData;
        string levelName = levelData.name;
        string[] splitName = levelName.Split('_');
        string levelNumber = splitName[1]; 
        levelText.text = "Lv." + levelNumber;
    }

    private void OnEnable()
    {
        starGain = 0;
        starOnUI = 0;
        SetLevelName();
        
        this.RegisterListener(EventID.On_Update_Star, param => UpdateStar((int) param));
        this.RegisterListener(EventID.On_Update_Star_Text, param => UpdateStarText((int) param));
        
        pauseButton.onClick.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.On_Update_Star, param => UpdateStar((int) param));
        this.RemoveListener(EventID.On_Update_Star_Text, param => UpdateStarText((int) param));
        
        pauseButton.onClick.RemoveAllListeners();
    }

    private void UpdateStar(int amount)
    {
        starGain += amount;
    }

    private void UpdateStarText(int amount)
    {
        starOnUI += amount;
        starText.text = starOnUI.ToString();
    }

    private void PauseGame()
    {
        EventDispatcher.Instance.PostEvent(EventID.On_Pause_Game);
        pausePanel.gameObject.SetActive(true);
    }
}
