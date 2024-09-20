using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    public bool isUseHammer;
    public bool isUseClock;
    public bool isUseDoubleStar;
    public List<BoxData> boxData;
}

[Serializable]
public class BoxData
{
    public string name;
    public BoxType boxType;
    public Vector3 boxPosition;
    public bool isSpecialBox;
    public int lockedTurn;
    public int speed;
    public int hp;
    public List<RowData> rowData;
}

[Serializable]
public class RowData
{
    public string name;
    public int posNumber;
    public List<ItemPosData> itemPosData;
}

[Serializable]
public class ItemPosData
{
    public string name;
    public int itemID;
}
