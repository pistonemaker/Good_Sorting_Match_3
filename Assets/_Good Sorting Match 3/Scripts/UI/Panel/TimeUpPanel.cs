using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TimeUpPanel : BasePanel
{
    public Button closeButton;
    public Button buyButton;
    public Button watchAdsButton;
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
        buyButton = transform.Find("Buy Button").GetComponent<Button>();
        watchAdsButton = transform.Find("Watch Ads Button").GetComponent<Button>();
        redLost1.fillAmount = redLost2.fillAmount = 0;
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
        
        buyButton.onClick.AddListener(() =>
        {
            ClosePanel(0.75f);
        });
        
        watchAdsButton.onClick.AddListener(() =>
        {
            ClosePanel(0.75f);
        });
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        watchAdsButton.onClick.RemoveAllListeners();
    }
}
