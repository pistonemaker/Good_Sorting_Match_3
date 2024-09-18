public class ShuffleBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Shuffle);
        SetUpBooster();
        
        base.OnEnable();
    }
}
