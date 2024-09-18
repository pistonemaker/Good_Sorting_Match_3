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
    public TimeUpPanel timeUpPanel;
    public GetMoreBoosterPanel getMoreBoosterPanel;

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

        this.RegisterListener(EventID.On_Update_Star, param => UpdateStar((int)param));
        this.RegisterListener(EventID.On_Update_Star_Text, param => UpdateStarText((int)param));
        this.RegisterListener(EventID.On_Show_Get_More_Panel, param => ShowGetMoreBoosterPanel((string) param));

        pauseButton.onClick.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.On_Update_Star, param => UpdateStar((int)param));
        this.RemoveListener(EventID.On_Update_Star_Text, param => UpdateStarText((int)param));
        this.RemoveListener(EventID.On_Show_Get_More_Panel, param => ShowGetMoreBoosterPanel((string) param));

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

    public void ShowGetMoreBoosterPanel(string dataKey)
    {
        if (dataKey == DataKey.Ingame_Hammer)
        {
            getMoreBoosterPanel.data = GameManager.Instance.boosterData.ingameBoosterData[0];
        }
        else if (dataKey == DataKey.Ingame_Magic_Wand)
        {
            getMoreBoosterPanel.data = GameManager.Instance.boosterData.ingameBoosterData[1];
        }
        else if (dataKey == DataKey.Ingame_Freeze)
        {
            getMoreBoosterPanel.data = GameManager.Instance.boosterData.ingameBoosterData[2];
        }
        else if (dataKey == DataKey.Ingame_Shuffle)
        {
            getMoreBoosterPanel.data = GameManager.Instance.boosterData.ingameBoosterData[3];
            getMoreBoosterPanel.data = GameManager.Instance.boosterData.ingameBoosterData[3];
        }

        getMoreBoosterPanel.gameObject.SetActive(true);
        getMoreBoosterPanel.dataKey = dataKey;
    }
}