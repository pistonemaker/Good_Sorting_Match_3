using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    public List<BoxData> boxData;
}

[Serializable]
public class BoxData
{
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
    public int posNumber;
    public List<ItemPosData> itemPosData;
}

[Serializable]
public class ItemPosData
{
    public int itemID;
}
