using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : Singleton<HomeManager>
{
    public Button heartButton;
    public Button coinButton;
    public Button settingButton;
    public Button playButton;

    public TextMeshProUGUI heartText;
    public TextMeshProUGUI heartTimeText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI levelText;

    public Image blockClick;

    public Sprite useSprite;
    public Sprite noUseSprite;

    public PlayPanel playPanel;
    public WinStreakPanel winStreakPanel;
    public SettingPanel settingPanel;
    
    public GameData data;
    public LevelData levelData;

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic("Home");
        Application.targetFrameRate = 60;
        levelData = data.data[PlayerPrefs.GetInt(DataKey.Cur_Level)];
        heartText.text = PlayerPrefs.GetInt(DataKey.Heart).ToString();
        coinText.text = PlayerPrefs.GetInt(DataKey.Coin).ToString();
        starText.text = PlayerPrefs.GetInt(DataKey.Star).ToString();
        levelText.text = "Level " + (PlayerPrefs.GetInt(DataKey.Cur_Level) + 1);

        playButton.onClick.AddListener(() => { playPanel.gameObject.SetActive(true); });
        settingButton.onClick.AddListener(() => { settingPanel.gameObject.SetActive(true); });

        blockClick.gameObject.SetActive(false);
    }

    private void Start()
    {
        PlayerPrefs.SetInt(DataKey.Cur_Level, 1);
        AdmobAds.Instance.rewardedAdController.LoadAd();
        AdmobAds.Instance.ShowBannerAds();
        
        int count = PlayerPrefs.GetInt(DataKey.Open_App_Count);
        if (count == 0)
        {
            PlayerPrefs.GetInt(DataKey.Open_App_Count, 1);
            PlayerPrefs.SetInt(DataKey.Use_Music, 1);
            PlayerPrefs.SetInt(DataKey.Use_SFX, 1);
            PlayerPrefs.SetInt(DataKey.Use_Vibrate, 1);
        }
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(DataKey.Open_App_Ads, 0);
    }
}