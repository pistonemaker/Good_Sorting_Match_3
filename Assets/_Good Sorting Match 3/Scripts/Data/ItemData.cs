using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    public List<Sprite> itemSprites;
}
