using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Booster Data", menuName = "Data/Booster Data")]
public class BoosterData : ScriptableObject
{
    public List<IngameBoosterData> ingameBoosterData;
}

[Serializable]
public class IngameBoosterData
{
    public Sprite sprite;
    public int cost;
    public string description;
}

