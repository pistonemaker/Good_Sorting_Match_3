using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ItemPosition : MonoBehaviour
{
    public Item itemHolding;
    public int idInRow;

    public bool IsHoldingItem
    {
        get => itemHolding != null;
    }
    
    public void Validate()
    {
        idInRow = transform.GetSiblingIndex();
        if (transform.childCount <= 0)
        {
            return;
        }
        itemHolding = transform.GetChild(0).GetComponent<Item>();
        itemHolding.holder = this;
        itemHolding.idInRow = idInRow;
        itemHolding.GetSprite();
    }
}
