public class DoubleStarOutBooster : OutBooster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Init(DataKey.Outgame_Double_Star);
        SetUp();
    }
    
    protected override void Click()
    {
        if (buttonBG.sprite == HomeManager.Instance.useSprite)
        {
            buttonBG.sprite = HomeManager.Instance.noUseSprite;
            HomeManager.Instance.levelData.isUseDoubleStar = false;
            useImage.gameObject.SetActive(false);
        }
        else
        {
            buttonBG.sprite = HomeManager.Instance.useSprite;
            HomeManager.Instance.levelData.isUseDoubleStar = true;
            useImage.gameObject.SetActive(true);
        }
    }
}
