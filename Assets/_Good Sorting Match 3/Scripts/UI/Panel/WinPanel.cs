using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : BasePanel
{
    private const int reward = 10;
    private int extraReward = 10;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI collectText;
    
    public Button watchAdsButton;
    public Button collectButton;
    
    public Arrow arrow;

    protected override void ClosePanel(float time)
    {
        SceneManager.LoadSceneAsync("Home");
    }

    protected override void LoadButtonAndImage()
    {
        levelText = transform.Find("Level Text").GetComponent<TextMeshProUGUI>();
        watchAdsButton = transform.Find("Watch Ads Button").GetComponent<Button>();
        collectButton = transform.Find("Collect Button").GetComponent<Button>();
        coinText = watchAdsButton.transform.Find("Coin Text").GetComponent<TextMeshProUGUI>();
        collectText = watchAdsButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        arrow = transform.Find("Extra Coin Bar").transform.Find("Arrow").GetComponent<Arrow>();
        arrow.winPanel = this;
    }

    protected override void SetListener()
    {
        watchAdsButton.onClick.AddListener(() =>
        {
            GainCoin(extraReward);
            SceneManager.LoadSceneAsync("Home");
        });
        
        collectButton.onClick.AddListener(() => 
        {
            GainCoin(reward);
            SceneManager.LoadSceneAsync("Home");
        });
    }

    public void ExtraReward(int multiplier)
    {
        extraReward = reward * multiplier;
        coinText.text = extraReward.ToString();
        collectText.text = "Collect x" + multiplier;
    }

    private void GainCoin(int coin)
    {
        int currentCoin = PlayerPrefs.GetInt(DataKey.Coin);
        currentCoin += coin;
        PlayerPrefs.SetInt(DataKey.Coin, currentCoin);
    }
    
    private void OnDisable()
    {
        watchAdsButton.onClick.RemoveAllListeners();
        collectButton.onClick.RemoveAllListeners();
    }
}
