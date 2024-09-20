using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class BoxRow : MonoBehaviour
{
    public RowData data;
    public Box box;
    public int rowID;
    public int boxID;
    public List<ItemPosition> itemPositions;

    public bool IsEmpty
    {
        get
        {
            for (int i = 0; i < itemPositions.Count; i++)
            {
                if (itemPositions[i].IsHoldingItem)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool CanGetItem
    {
        get
        {
            for (int i = 0; i < itemPositions.Count; i++)
            {
                if (!itemPositions[i].IsHoldingItem)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool IsFull
    {
        get
        {
            for (int i = 0; i < itemPositions.Count; i++)
            {
                if (!itemPositions[i].IsHoldingItem)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public int ItemPositionCount
    {
        get => transform.childCount;
    }

    public void Init(RowData rowData, int id)
    {
        data = rowData;
        rowID = id;
        name = rowData.name;

        for (int i = 0; i < rowData.itemPosData.Count; i++)
        {
            var itemPosition = PoolingManager.Spawn(GameManager.Instance.itemPositionPrefab, 
                transform.position, Quaternion.identity);
            itemPosition.transform.SetParent(transform);
            itemPosition.transform.localScale = Vector3.one;
            itemPosition.row = this;
            itemPosition.boxID = boxID;
            itemPosition.Init(data.itemPosData[i], i);
            itemPositions.Add(itemPosition);
        }
        
        SetPosition();
    }

    private void SetPosition()
    {
        switch (itemPositions.Count)
        {
            case 0:
                return;
            case 1:
                itemPositions[0].transform.position = new Vector3(transform.position.x, 
                    transform.position.y + 0.5f, transform.position.z);
                return;
            case 3:
                itemPositions[0].transform.position = new Vector3(transform.position.x - 1, 
                    transform.position.y, transform.position.z);
                itemPositions[1].transform.position = new Vector3(transform.position.x, 
                    transform.position.y, transform.position.z);
                itemPositions[2].transform.position = new Vector3(transform.position.x + 1, 
                    transform.position.y, transform.position.z);
                break;
        }
    }

    public ItemPosition GetNerestEmptyPositions(Item item)
    {
        float minDistance = float.MaxValue;
        int minDistanceIndex = 0;

        for (int i = 0; i < itemPositions.Count; i++)
        {
            if (itemPositions[i].IsHoldingItem)
            {
                continue;
            }

            float distance = Vector3.Distance(itemPositions[i].transform.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceIndex = i;
            }
        }

        return itemPositions[minDistanceIndex];
    }

    public bool CanMatch3()
    {
        // Chưa đủ 3 item thì không match được 
        if (ItemPositionCount != 3 || CanGetItem)
        {
            return false;
        }

        if (itemPositions[0].itemHolding.itemID == itemPositions[1].itemHolding.itemID
            && itemPositions[1].itemHolding.itemID == itemPositions[2].itemHolding.itemID)
        {
            return true;
        }
                                         
        return false;
    }

    public IEnumerator Match3Item()
    {
        for (int i = 0; i < itemPositions.Count; i++)
        {
            itemPositions[i].itemHolding.BounceMatch3();
        }
        
        yield return new WaitForSeconds(0.3f);
        
        this.PostEvent(EventID.On_Complete_A_Match_3, boxID);
        this.PostEvent(EventID.On_Check_Row_Empty, boxID);
        EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
    }

    public void ShowRow()
    {
        for (int i = 0; i < itemPositions.Count; i++)
        {
            itemPositions[i].ShowHoldingItem();
        }
    }

    public void GrayRow()
    {
        for (int i = 0; i < itemPositions.Count; i++)
        {
            itemPositions[i].GrayHoldingItem();
        }
    }

    public void DeactiveRow()
    {
        gameObject.SetActive(false);
    }

    public void ActiveRow()
    {
        gameObject.SetActive(true);
    }

    public void BlockDragItem()
    {
        for (int i = 0; i < itemPositions.Count; i++)
        {
            if (itemPositions[i].IsHoldingItem)
            {
                itemPositions[i].itemHolding.canDrag = false;
            }
        }
    }

    public void UnBlockDragItem()
    {
        for (int i = 0; i < itemPositions.Count; i++)
        {
            if (itemPositions[i].IsHoldingItem)
            {
                itemPositions[i].itemHolding.canDrag = true;
            }
        }
    }

    public void Validate()
    {
        itemPositions = GetComponentsInChildren<ItemPosition>().ToList();

        for (int i = 0; i < itemPositions.Count; i++)
        {
            itemPositions[i].idInRow = i;
            itemPositions[i].row = this;
            itemPositions[i].Validate();
        }
    }

    public void SaveRowData(RowData rowData)
    {
        if (rowData.itemPosData == null || rowData.itemPosData.Count != itemPositions.Count)
        {
            rowData.itemPosData = new List<ItemPosData>();
            rowData.name = "Row " + (rowID + 1);
            rowData.posNumber = transform.childCount;
            for (int i = 0; i < itemPositions.Count; i++)
            {
                rowData.itemPosData.Add(new ItemPosData());
            }
        }

        for (int i = 0; i < itemPositions.Count; i++)
        {
            itemPositions[i].SaveItemPositionData(rowData.itemPosData[i]);
        }
    }
}