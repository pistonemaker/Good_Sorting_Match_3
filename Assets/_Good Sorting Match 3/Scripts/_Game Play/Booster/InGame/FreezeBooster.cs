public class FreezeBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Freeze);
        SetUpBooster();
        
        base.OnEnable();
    }
    
    protected override void UseBooster(string key)
    {
        if (dataKey != key)
        {
            return;
        }

        TimeManager.Instance.Freeze();
    }
}
