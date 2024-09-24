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
        if (HomeManager.Instance != null)
        {
            if (buttonBG.sprite == HomeManager.Instance.useSprite)
            {
                NoUse();
                HomeManager.Instance.levelData.isUseDoubleStar = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                HomeManager.Instance.levelData.isUseDoubleStar = true;
                useImage.gameObject.SetActive(true);
            }
        }
        else
        {
            if (buttonBG.sprite == UIManager.Instance.useSprite)
            {
                NoUse();
                GameController.Instance.levelData.isUseDoubleStar = false;
                useImage.gameObject.SetActive(false);
            }
            else
            {
                Use();
                GameController.Instance.levelData.isUseDoubleStar = true;
                useImage.gameObject.SetActive(true);
            }
        }
    }
}
