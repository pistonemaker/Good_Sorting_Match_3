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
        if (HomeManager.Instance != null)
        {
            if (buttonBG.sprite == HomeManager.Instance.useSprite)
            {
                NoUse();
                HomeManager.Instance.levelData.isUseClock = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                HomeManager.Instance.levelData.isUseClock = true;
                useImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (buttonBG.sprite == UIManager.Instance.useSprite)
            {
                NoUse();
                GameController.Instance.levelData.isUseClock = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                GameController.Instance.levelData.isUseClock = true;
                useImage.gameObject.SetActive(true);
            }
        }
    }
}