public class HammerOutBooster : OutBooster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Init(DataKey.Outgame_Hammer);
        SetUp();
    }
    
    protected override void Click()
    {
        if (buttonBG.sprite == HomeManager.Instance.useSprite)
        {
            buttonBG.sprite = HomeManager.Instance.noUseSprite;
            HomeManager.Instance.levelData.isUseHammer = false;
            useImage.gameObject.SetActive(false);
        }
        else
        {
            buttonBG.sprite = HomeManager.Instance.useSprite;
            HomeManager.Instance.levelData.isUseHammer = true;
            useImage.gameObject.SetActive(true);
        }
    }
}
