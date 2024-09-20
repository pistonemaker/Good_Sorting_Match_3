using UnityEngine.UI;

public class ExitPanelLostWinStreak : BasePanel
{
    public WinStreak winStreak;
    public Button closeButton;
    public Button quitButton;

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
            ClosePanel(0f);
        });
        
        quitButton.onClick.AddListener(() =>
        {
            ClosePanel(0.75f);
        });
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
