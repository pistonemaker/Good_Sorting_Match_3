using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExitPanel : BasePanel
{
    public Button closeButton;
    public Button quitButton;
    public Image redLost1;
    public Image redLost2;
    public TextMeshProUGUI heartLostText;
    public TextMeshProUGUI starLostText;

    protected override void OnEnable()
    {
        base.OnEnable();
        Invoke(nameof(PlayAnimRedLost), 0.15f);
    }

    protected override void LoadButtonAndImage()
    {
        closeButton = transform.Find("Ribbon").Find("Close Button").GetComponent<Button>();
        quitButton = transform.Find("Quit Button").GetComponent<Button>();
        redLost1.fillAmount = redLost2.fillAmount = 0;
        starLostText.text = UIManager.Instance.starGain.ToString();
    }

    private void PlayAnimRedLost()
    {
        redLost1.DOFillAmount(1, 0.25f);
        redLost2.DOFillAmount(1, 0.25f);
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(() =>
        {
            ClosePanel(0f);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            ClosePanel(0.75f);
            CheckIfShowLostWinStreakPanel();
        });
    }

    protected void CheckIfShowLostWinStreakPanel()
    {
        int winStreak = PlayerPrefs.GetInt(DataKey.Win_Streak);
        
        if (winStreak > 0)
        {
            UIManager.Instance.exitPanelLostWinStreak.isLost = false;
            UIManager.Instance.exitPanelLostWinStreak.gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.playAgainPanel.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
