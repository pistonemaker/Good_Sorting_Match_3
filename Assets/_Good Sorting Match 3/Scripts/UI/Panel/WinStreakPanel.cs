using UnityEngine.UI;

public class WinStreakPanel : BasePanel
{
    public Button closeButton;
    public Button continueButton;
    public WinStreak winStreak;
    
    protected override void LoadButtonAndImage()
    {
        closeButton = transform.Find("Close Button").GetComponent<Button>();
        continueButton = transform.Find("Continue Button").GetComponent<Button>();
        winStreak = transform.Find("Win Streak").GetComponent<WinStreak>();
        winStreak.SetWinStreak();
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(ClosePanel);
        continueButton.onClick.AddListener(ClosePanel);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
    }
}
