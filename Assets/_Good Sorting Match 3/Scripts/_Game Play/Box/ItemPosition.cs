using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public BoxRow row;
    public ItemPosData data;
    public Item itemHolding;
    public int rowID;
    public int idInRow;
    public int boxID;

    public bool IsHoldingItem
    {
        get => itemHolding != null;
    }

    public void Init(ItemPosData itemPosData, int id)
    {
        data = itemPosData;
        idInRow = id;
        rowID = row.rowID;
        
        if (itemPosData.itemID != -1)
        {
            var item = PoolingManager.Spawn(GameManager.Instance.itemPrefab, transform.position, Quaternion.identity);
            item.transform.SetParent(transform);
            item.transform.localScale = Vector3.one;
            item.Init(this, itemPosData.itemID);
            itemHolding = item;
            EditLevelManager.Instance.itemManager.AddItem(itemHolding);
        }
    }

    public void GrayHoldingItem()
    {
        if (!itemHolding)
        {
            return;
        }
        
        itemHolding.GrayItem();
    }

    public void ShowHoldingItem()
    {
        if (!itemHolding)
        {
            return;
        }
        
        itemHolding.ShowItem();
    }

    public void Validate()
    {
        if (transform.childCount == 0)
        {
            return;
        }

        itemHolding = GetComponentInChildren<Item>();
        itemHolding.holder = this;
        itemHolding.idInRow = idInRow;
        itemHolding.GetSprite();
    }

    public void SaveItemPositionData(ItemPosData itemPosData)
    {
        itemPosData.name = "Item Position " + (idInRow + 1);
        if (itemHolding != null)
        {
            itemPosData.itemID = itemHolding.itemID;
        }
        else
        {
            itemPosData.itemID = -1; // Không có item
        }
    }
}