using TMPro;
using UnityEngine.UI;

public class GetMoreBoosterPanel : BasePanel
{
    public IngameBoosterData data;
    public string dataKey;
    public Button closeButton;
    public Button buyButton;
    public Button watchAdsButton;

    public Image boosterIcon;
    public TextMeshProUGUI description;
    public TextMeshProUGUI costText;

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadPanelData();
    }

    protected override void LoadButtonAndImage()
    {
        closeButton = transform.Find("Close Button").GetComponent<Button>();
        buyButton = transform.Find("Buy Button").GetComponent<Button>();
        watchAdsButton = transform.Find("Watch Ads Button").GetComponent<Button>();
        boosterIcon = transform.Find("Booster Icon").GetComponent<Image>();
        description = transform.Find("BG Description").GetChild(0).GetComponent<TextMeshProUGUI>();
        costText = buyButton.transform.Find("Cost Text").GetComponent<TextMeshProUGUI>();
    }

    protected override void SetListener()
    {
        closeButton.onClick.AddListener(ClosePanel);
        buyButton.onClick.AddListener(() =>
        {
            this.PostEvent(EventID.On_Buy_Ingame_Booster, dataKey);
            ClosePanel();
        });
    }

    public void LoadPanelData()
    {
        boosterIcon.sprite = data.sprite;
        description.text = data.description;
        costText.text = data.cost.ToString();
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        watchAdsButton.onClick.RemoveAllListeners();
    }
}
