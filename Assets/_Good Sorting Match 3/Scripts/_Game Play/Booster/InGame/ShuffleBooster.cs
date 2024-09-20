public class ShuffleBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Shuffle);
        SetUpBooster();
        
        base.OnEnable();
    }
    
    protected override void UseBooster(string key)
    {
        if (dataKey != key)
        {
            return;
        }

        GameController.Instance.Shuffle();
    }
}
