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

    protected override void ClosePanel(float time, bool resume)
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
        levelText.text = "Leevel " + (PlayerPrefs.GetInt(DataKey.Cur_Level) + 1);
    }

    protected override void SetListener()
    {
        watchAdsButton.onClick.AddListener(() =>
        {
            AdmobAds.Instance.ShowRewardAds(() =>
            {
                GainCoin(extraReward);
                SceneManager.LoadSceneAsync("Home");
                AdmobAds.Instance.rewardedAdController.LoadAd();
            });
        });

        collectButton.onClick.AddListener(() =>
        {
            int count = PlayerPrefs.GetInt(DataKey.Show_Inter_Count);
            PlayerPrefs.SetInt(DataKey.Show_Inter_Count, count + 1);

            if (count % 2 == 0)
            {
                AdmobAds.Instance.ShowInterAds(() =>
                {
                    GainCoin(reward);
                    LoadToHomeScene();
                });
            }
            else
            {
                GainCoin(reward);
                LoadToHomeScene();
            }

            //LoadToHomeScene();
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

    private void LoadToHomeScene()
    {
        SceneManager.LoadSceneAsync("Home");
    }
}