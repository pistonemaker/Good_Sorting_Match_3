using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Item itemPrefab;
    public ItemPosition itemPositionPrefab;
    public BoxRow boxRowPrefab;
    public Box boxPrefab;
    public LockedBox lockedBoxPrefab;
    public MovingBox movingBoxPrefab;
    public OneShotBox oneShotBoxPrefab;
    public GameObject hpBarPrefab;
    public Star star;

    public GameObject hammerIngamePrefab;
    public GameObject hammerOutgamePrefab;
    public GameObject clockOutgamePrefab;
    public GameObject doubleStarOutgamePrefab;
    public GameObject winStreak1Prefab;
    public GameObject winStreak2Prefab;
    public GameObject winStreak3Prefab;
    public GameObject vfxMatchPrefab;
    
    public ChangingLight changeLightPrefab;

    public ItemData itemData;
    public BoosterData boosterData;

    private void Start()
    {
        AudioManager.Instance.PlayMusic("Game_Play");
        if (AdmobAds.Instance != null)
        {
            AdmobAds.Instance.interstitialAdController.LoadAd();
            AdmobAds.Instance.rewardedAdController.LoadAd();
            AdmobAds.Instance.ShowBannerAds();
        }
    }
    
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(DataKey.Open_App_Ads, 0);
    }
}
