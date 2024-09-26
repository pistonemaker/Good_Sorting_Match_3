using UnityEngine.UI;

public class ExitPanelLostWinStreak : BasePanel
{
    public WinStreak winStreak;
    public Button closeButton;
    public Button quitButton;
    public bool isLost = false;

    protected override void LoadButtonAndImage()
    {
        closeButton = transform.Find("Ribbon").Find("Close Button").GetComponent<Button>();
        quitButton = transform.Find("Quit Button").GetComponent<Button>();
        winStreak = transform.Find("Win Streak").GetComponent<WinStreak>();
        winStreak.ResetWinStreak();
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(() =>
        {
            if (isLost)
            {
                UIManager.Instance.playAgainPanel.gameObject.SetActive(true);
                ClosePanel(0f);
            }
            else
            {
                ClosePanel(0f);
            }
        });

        quitButton.onClick.AddListener(() =>
        {
            ClosePanel(0.75f);
            UIManager.Instance.playAgainPanel.gameObject.SetActive(true);
        });
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}