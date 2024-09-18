using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
    public LevelData levelData;
    public List<Box> boxes;
    public List<LockedBox> lockedBoxes;
    public ItemManager itemManager;
    public List<Item> itemList;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        var boxData = levelData.boxData;
        for (int i = 0; i < boxData.Count; i++)
        {
            if (boxData[i] == null)
            {
                Debug.LogError("Level Data is not valid because of null BoxData at index " + i);
                return;
            }

            var box = CreateBox(boxData[i].boxType);
            box.transform.SetParent(transform);
            box.transform.position = boxData[i].boxPosition;
            box.transform.localScale = Vector3.one;
            box.Init(boxData[i], i);
            boxes.Add(box);
        }

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].transform.localScale = Vector3.one * 0.5f;
            if (boxes[i].boxType == BoxType.Locked)
            {
                lockedBoxes.Add((LockedBox)boxes[i]);
            }
        }
    }

    private Box CreateBox(BoxType boxType)
    {
        Box box;

        if (boxType == BoxType.Normal)
        {
            box = PoolingManager.Spawn(GameManager.Instance.boxPrefab, transform.position, Quaternion.identity);
        }
        else if (boxType == BoxType.Locked)
        {
            box = PoolingManager.Spawn(GameManager.Instance.lockedBoxPrefab, transform.position, Quaternion.identity);
        }
        else if (boxType == BoxType.Moving)
        {
            box = PoolingManager.Spawn(GameManager.Instance.movingBoxPrefab, transform.position, Quaternion.identity);
        }
        else // if (boxType == BoxType.OneShot)
        {
            box = PoolingManager.Spawn(GameManager.Instance.oneShotBoxPrefab, transform.position, Quaternion.identity);
        }

        return box;
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener(EventID.On_Complete_A_Match_3, DecreaseLockTurn);
        EventDispatcher.Instance.RegisterListener(EventID.On_Check_Player_Win, OnCheckPlayerWin);
        EventDispatcher.Instance.RegisterListener(EventID.On_Check_Player_Lose, OnCheckPlayerLose);
    }

    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener(EventID.On_Complete_A_Match_3, DecreaseLockTurn);
        EventDispatcher.Instance.RemoveListener(EventID.On_Check_Player_Win, OnCheckPlayerWin);
        EventDispatcher.Instance.RemoveListener(EventID.On_Check_Player_Lose, OnCheckPlayerLose);
    }

    public Box GetNearestBox(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int minDistanceIndex = 0;

        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i].canPutItem)
            {
                continue;
            }

            float distance = Vector3.Distance(boxes[i].transform.position, position);

            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceIndex = i;
            }
        }

        return boxes[minDistanceIndex];
    }

    public void DecreaseLockTurn(object param)
    {
        for (int i = 0; i < lockedBoxes.Count; i++)
        {
            if (lockedBoxes[i].lockedTurn > 0)
            {
                lockedBoxes[i].DecreaseLockTurn();
                break;
            }
        }
    }

    public bool IsPlayerWin()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i].IsEmpty)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsPlayerLose()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i].IsFull)
            {
                return false;
            }
        }

        return true;
    }

    private void OnCheckPlayerWin(object param)
    {
        if (IsPlayerWin())
        {
            Debug.Log("Win");
        }
    }

    private void OnCheckPlayerLose(object param)
    {
        if (IsPlayerLose())
        {
            Debug.Log("Lose");
        }
    }

    public void FindMatch3(int match3Number)
    {
        for (int a = 0; a < match3Number; a++)
        {
            if (itemManager.items.Count == 0)
            {
                EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
                break;
            }
            
            int idran = Random.Range(0, itemManager.items.Count);
            int id = itemManager.items[idran].id;
            int numberToFind = 3;

            for (int i = 0; i < itemList.Count; i++)
            {
                if (numberToFind <= 0)
                {
                    break;
                }

                if (itemList[i].itemID == id)
                {
                    var item = itemList[i];
                    itemList.RemoveAt(i);
                    i--;
                    numberToFind--;
                    itemManager.RemoveItem(id);
                    item.ShowItemImmediately();
                    item.MoveToCenter();
                }
            }

            if (numberToFind > 0)
            {
                Debug.Log("Fail " + numberToFind + "   " + id);
            }
            else
            {
                Invoke(nameof(PostEventCompleteAMatch3), 1f);
            }
        }
    }

    public void Change9ItemToOne()
    {
        for (int a = 0; a < 3; a++)
        {
            if (itemManager.items.Count == 0)
            {
                EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
                break;
            }
            
            int idran = Random.Range(0, itemManager.items.Count);
            int idran2 = Random.Range(0, itemManager.items.Count);
            int id = itemManager.items[idran].id;
            int idbecome = itemManager.items[idran2].id;
            int numberToFind = 3;

            for (int i = 0; i < itemList.Count; i++)
            {
                if (numberToFind <= 0)
                {
                    break;
                }

                if (itemList[i].itemID == id)
                {
                    var item = itemList[i];
                    numberToFind--;
                    itemManager.RemoveItem(id);
                    itemManager.AddItem(idbecome);
                    item.ShowItemImmediately();
                    Debug.Log(item.boxID);
                    item.MoveToCenter();
                }
            }

            if (numberToFind > 0)
            {
                Debug.Log("Fail " + numberToFind + "   " + id);
            }
            else
            {
                Invoke(nameof(PostEventCompleteAMatch3), 1f);
            }
        }
    }

    private void PostEventCompleteAMatch3()
    {
        this.PostEvent(EventID.On_Complete_A_Match_3, -1);
        EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
    }
}

[Serializable]
public class ItemManager
{
    public List<ItemCollection> items;

    public bool ContainsItem(int itemID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == itemID)
            {
                return true;
            }
        }

        return false;
    }

    public void AddItem(int itemID)
    {
        if (!ContainsItem(itemID))
        {
            items.Add(new ItemCollection(itemID, 1));
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].id == itemID)
                {
                    items[i].amount++;
                }
            }
        }
    }

    public void RemoveItem(int itemID)
    {
        if (!ContainsItem(itemID))
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == itemID)
            {
                if (items[i].amount <= 1)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    items[i].amount--;
                }

                return;
            }
        }
    }
}

[Serializable]
public class ItemCollection
{
    public int id;
    public int amount;

    public ItemCollection(int id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}