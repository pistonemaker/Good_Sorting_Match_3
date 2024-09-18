using UnityEngine;

public class HammerBooster : Booster
{
    protected override void OnEnable()
    {
        Init(DataKey.Ingame_Hammer);
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
        Debug.Log(key + "   " + gameObject.name);
        if (dataKey != key)
        {
            return;
        }

        var prefab = PoolingManager.Spawn(boosterPrefab, Vector3.zero, Quaternion.identity);
        GameController.Instance.FindMatch3(4);
    }
}