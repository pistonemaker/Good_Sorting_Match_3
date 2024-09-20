public class ClockOutBooster : OutBooster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Init(DataKey.Outgame_Clock);
        SetUp();
    }

    protected override void Click()
    {
        if (buttonBG.sprite == HomeManager.Instance.useSprite)
        {
            buttonBG.sprite = HomeManager.Instance.noUseSprite;
            HomeManager.Instance.levelData.isUseClock = false;
            useImage.gameObject.SetActive(false);
        }
        else
        {
            buttonBG.sprite = HomeManager.Instance.useSprite;
            HomeManager.Instance.levelData.isUseClock = true;
            useImage.gameObject.SetActive(true);
        }
    }
}