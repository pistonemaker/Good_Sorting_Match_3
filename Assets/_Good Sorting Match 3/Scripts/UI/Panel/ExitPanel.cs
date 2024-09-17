using DG.Tweening;
using TMPro;
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
        closeButton = transform.Find("Close Button").GetComponent<Button>();
        quitButton = transform.Find("Quit Button").GetComponent<Button>();
        redLost1.fillAmount = redLost2.fillAmount = 0;
    }

    private void PlayAnimRedLost()
    {
        redLost1.DOFillAmount(1, 0.25f);
        redLost2.DOFillAmount(1, 0.25f);
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(ClosePanel);
        quitButton.onClick.AddListener(ClosePanel);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
