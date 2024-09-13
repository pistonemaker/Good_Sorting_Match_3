using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class BoxRow : MonoBehaviour
{
    public Box box;
    public int rowID;
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

    private void OnEnable()
    {
        SetUpRow();
    }

    private void SetUpRow()
    {
        itemPositions.Clear();
        for (int i = 0; i < itemPositions.Count; i++)
        {
            var itemPosition = transform.GetChild(i).GetComponent<ItemPosition>();
            itemPositions.Add(itemPosition);
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

        Debug.Log(minDistanceIndex);
        return itemPositions[minDistanceIndex];
    }

    public void CanMatch3()
    {
        // Chưa đủ 3 item thì không match được 
        if (!CanGetItem)
        {
            return;
        }

        if (itemPositions[0].itemHolding.itemID == itemPositions[1].itemHolding.itemID
            && itemPositions[1].itemHolding.itemID == itemPositions[2].itemHolding.itemID)
        {
            Match3Item();
        }
    }

    private void Match3Item()
    {
        Debug.Log("Match");
    }

    public void Validate()
    {
        if (transform.childCount == 0)
        {
            for (int i = 0; i < box.maxItemPositionInRow; i++)
            {
                var itemPosition = PoolingManager.Spawn(GameManager.Instance.itemPositionPrefab, transform.position, Quaternion.identity);
                itemPositions.Add(itemPosition);
                itemPosition.transform.SetParent(transform);
            }
        }
        
        itemPositions = GetComponentsInChildren<ItemPosition>().ToList();

        foreach (var itemPosition in itemPositions)
        {
            itemPosition.Validate();
        }
    }
}