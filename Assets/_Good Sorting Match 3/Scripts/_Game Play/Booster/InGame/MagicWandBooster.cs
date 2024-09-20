using UnityEngine;

public class MagicWandBooster : Booster
{
    protected GameObject boosterPrefab;
    
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Magic_Wand);
        SetUpBooster();
        
        base.OnEnable();
    }

    protected override void Init(string key)
    {
        base.Init(key);
        boosterPrefab = GameManager.Instance.hammerIngamePrefab;
    }

    protected override void UseBooster(string key)
    {
        if (dataKey != key)
        {
            return;
        }

        PoolingManager.Spawn(boosterPrefab, Vector3.zero, Quaternion.identity);
        GameController.Instance.Change9ItemToOne();
    }
}
