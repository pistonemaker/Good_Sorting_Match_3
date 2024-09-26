using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI starText;
    public Image uiStar;

    public int starGain;
    public int starOnUI;

    public Button pauseButton;

    public PausePanel pausePanel;
    public ExitPanel exitPanel;
    public ExitPanelLostWinStreak exitPanelLostWinStreak;
    public WinStreakPanel winStreakPanel;
    public TimeUpPanel timeUpPanel;
    public GetMoreBoosterPanel getMoreBoosterPanel;
    public WinPanel winPanel;
    public PlayAgainPanel playAgainPanel;
    public OutOfSlotPanel outOfSlotPanel;

    public Image blockClick;

    public int starMultiplier = 1;
    
    public Sprite doubleStar;
    public Sprite useSprite;
    public Sprite noUseSprite;

    private void SetLevelName()
    {
        int level = PlayerPrefs.GetInt(DataKey.Cur_Level);
        levelText.text = "Lv." + (level + 1);
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
        
        blockClick.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.On_Update_Star, param => UpdateStar((int)param));
        this.RemoveListener(EventID.On_Update_Star_Text, param => UpdateStarText((int)param));
        this.RemoveListener(EventID.On_Show_Get_More_Panel, param => ShowGetMoreBoosterPanel((string) param));

        pauseButton.onClick.RemoveAllListeners();
    }

    public void DoubleStar(GameObject booster)
    {
        starMultiplier = 2;

        booster.transform.DOScale(0.1f, 1f);
        booster.transform.DOJump(uiStar.transform.position, 0.5f, 1, 1f).OnComplete(() =>
        {
            uiStar.sprite = doubleStar;
            PoolingManager.Despawn(booster);
        });
    }

    private void UpdateStar(int amount)
    {
        starGain += amount * starMultiplier;
    }

    private void UpdateStarText(int amount)
    {
        starOnUI += amount * starMultiplier;
        starText.text = starOnUI.ToString();
    }

    private void PauseGame()
    {
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

    public void BlockClick()
    {
        blockClick.gameObject.SetActive(true);
    }

    private IEnumerator DeBlock(float time)
    {
        yield return new WaitForSeconds(time);
        blockClick.gameObject.SetActive(false);
    }

    public void DeBlockClick(float time)
    {
        StartCoroutine(DeBlock(time));
    }

    public IEnumerator ShowWinPanel()
    {
        yield return new WaitForSeconds(0.5f);
        winPanel.gameObject.SetActive(true);
    }
}