public class FreezeBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Freeze);
        SetUpBooster();
        
        base.OnEnable();
    }
}
