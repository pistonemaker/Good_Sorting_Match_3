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
        if (HomeManager.Instance != null)
        {
            if (buttonBG.sprite == HomeManager.Instance.useSprite)
            {
                NoUse();
                HomeManager.Instance.levelData.isUseHammer = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                HomeManager.Instance.levelData.isUseHammer = true;
                useImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (buttonBG.sprite == UIManager.Instance.useSprite)
            {
                NoUse();
                GameController.Instance.levelData.isUseHammer = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                GameController.Instance.levelData.isUseHammer = true;
                useImage.gameObject.SetActive(true);
            }
        }
    }
}
