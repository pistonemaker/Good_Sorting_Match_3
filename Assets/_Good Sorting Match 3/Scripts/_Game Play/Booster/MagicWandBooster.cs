public class MagicWandBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Magic_Wand);
        SetUpBooster();
        
        base.OnEnable();
    }
}
