using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : Singleton<GameController>
{
    public GameData data;
    public LevelData levelData;
    public List<Box> boxes;
    public List<LockedBox> lockedBoxes;
    public ItemManager itemManager;
    public List<Item> itemList;
    public List<Item> fitem;

    public int MaxRow
    {
        get
        {
            int maxRow = 0;
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].rows.Count > maxRow)
                {
                    maxRow = boxes[i].rows.Count;
                }
            }

            return maxRow;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        Application.targetFrameRate = 60;
        int level = PlayerPrefs.GetInt(DataKey.Cur_Level);
        levelData = data.data[level];

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

        UIManager.Instance.BlockClick();
        Invoke(nameof(ApplyingOutGameBooster), 0.25f);
    }

    private void ApplyingOutGameBooster()
    {
        if (levelData.isUseHammer)
        {
            PoolingManager.Spawn(GameManager.Instance.hammerOutgamePrefab, Vector3.zero, Quaternion.identity);
            HandleHammerBooster(4);
        }

        if (levelData.isUseClock)
        {
            var clock = PoolingManager.Spawn(GameManager.Instance.clockOutgamePrefab, Vector3.zero, Quaternion.identity);
            TimeManager.Instance.BoostTime(60f, clock);
        }

        if (levelData.isUseDoubleStar)
        {
            var star = PoolingManager.Spawn(GameManager.Instance.doubleStarOutgamePrefab, Vector3.zero, Quaternion.identity);
            UIManager.Instance.DoubleStar(star);
        }

        int winStreak = PlayerPrefs.GetInt(DataKey.Win_Streak);
        if (winStreak == 1)
        {
            var treak = PoolingManager.Spawn(GameManager.Instance.winStreak1Prefab, Vector3.zero, Quaternion.identity);
            TimeManager.Instance.BoostTime(10f, treak);
        }
        else if (winStreak == 2)
        {
            var treak = PoolingManager.Spawn(GameManager.Instance.winStreak2Prefab, Vector3.zero, Quaternion.identity);
            TimeManager.Instance.BoostTime(20f, treak);
        }
        else if (winStreak == 3)
        {
            var treak = PoolingManager.Spawn(GameManager.Instance.winStreak3Prefab, Vector3.zero, Quaternion.identity);
            TimeManager.Instance.BoostTime(30f, treak);
        }

        UIManager.Instance.DeBlockClick(1.25f);
    }

    private Box CreateBox(BoxType boxType)
    {
        Box prefab = boxType switch
        {
            BoxType.Normal => GameManager.Instance.boxPrefab,
            BoxType.Locked => GameManager.Instance.lockedBoxPrefab,
            BoxType.Moving => GameManager.Instance.movingBoxPrefab,
            _ => GameManager.Instance.oneShotBoxPrefab
        };

        return PoolingManager.Spawn(prefab, transform.position, Quaternion.identity);
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

    public bool IsPlayerWin() => boxes.All(box => box.IsEmpty);

    public bool IsPlayerLose() => boxes.All(box => box.IsFull);

    private void OnCheckPlayerWin(object param)
    {
        if (IsPlayerWin())
        {
            UIManager.Instance.winPanel.gameObject.SetActive(true);
            int curLevel = PlayerPrefs.GetInt(DataKey.Cur_Level);
            curLevel++;
            PlayerPrefs.SetInt(DataKey.Cur_Level, curLevel);

            if (!DataKey.IsLostCurLevelBefore())
            {
                int winStreak = PlayerPrefs.GetInt(DataKey.Win_Streak);
                winStreak++;
                if (winStreak > 3)
                {
                    winStreak = 3;
                }

                PlayerPrefs.SetInt(DataKey.Win_Streak, winStreak);
            }
            else
            {
                PlayerPrefs.SetInt(DataKey.Cur_Level_Lost_Time, 0);
            }
        }
    }

    private void OnCheckPlayerLose(object param)
    {
        if (IsPlayerLose())
        {
            Debug.Log("Lose");
            PlayerPrefs.SetInt(DataKey.Cur_Level_Lost_Time, 1);
            PlayerPrefs.SetInt(DataKey.Win_Streak, 0);
        }
    }

    public void HandleMagicWandBooster(int number = 3)
    {
        List<Item> foundItems = FindMatch3(number);
        Debug.Log(foundItems.Count);

        if (foundItems.Count == 0)
        {
            return;
        }

        int idrandom = Random.Range(0, itemManager.itemCollections.Count);
        int idbecome = itemManager.itemCollections[idrandom].itemID;

        for (int i = 0; i < foundItems.Count; i++)
        {
            var item = foundItems[i];
            Debug.Log(item);
            itemManager.RemoveItem(item);
            StartCoroutine(item.ChangeItemID(idbecome));
            itemManager.AddItem(item);
            var spawnPos = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
            var light = PoolingManager.Spawn(GameManager.Instance.changeLightPrefab, spawnPos, Quaternion.identity);
            light.SetTarget(item.transform);
        }
    }

    public void HandleHammerBooster(int number)
    {
        List<Item> foundItems = FindMatch3(number);
        Debug.Log(foundItems.Count);
    
        if (foundItems.Count == 0)
        {
            return;
        }
    
        for (int i = 0; i < foundItems.Count; i++)
        {
            var item = foundItems[i];
            itemList.Remove(item);
            itemManager.RemoveItem(item);
            item.ShowItemImmediately("UI Behind");
            item.MoveToCenter();
        }
    }
    
    private List<Item> FindMatch3(int number)
    {
        ItemManager manager = new ItemManager();
        List<Item> listReturn = new List<Item>();

        for (int row = 0; row < MaxRow; row++)
        {
            AddItemFromRowToManager(row, manager);

            List<Item> listFound = FindMatch3InItemManager(manager, number * 3 - listReturn.Count);
        
            if (listFound != null)
            {
                listReturn.AddRange(listFound);
                if (listReturn.Count >= number * 3)
                {
                    break;
                }
            }
        }

        Invoke(nameof(PostEventCompleteAMatch3), 1f);

        return listReturn;
    }
    
    private void AddItemFromRowToManager(int rowID, ItemManager manager)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            if (boxes[i].boxType != BoxType.Normal)
            {
                continue;
            }
    
            var box = boxes[i];
            if (rowID >= box.rows.Count)
            {
                continue;
            }
    
            for (int j = 0; j < box.rows[rowID].itemPositions.Count; j++)
            {
                var itemPos = box.rows[rowID].itemPositions[j];
                if (!itemPos.IsHoldingItem)
                {
                    continue;
                }
    
                var item = itemPos.itemHolding;
                manager.AddItem(item);
            }
        }
    }
    
    private List<Item> FindMatch3InItemManager(ItemManager manager, int requiredItems)
    {
        List<Item> listReturn = new List<Item>();
        List<Item> itemsToRemove = new List<Item>();

        foreach (var collection in manager.itemCollections)
        {
            int itemsToAdd = Math.Min(3, requiredItems - listReturn.Count);  
            
            if (collection.items.Count >= 3)
            {
                for (int j = 0; j < itemsToAdd; j++)  
                {
                    listReturn.Add(collection.items[j]);
                    itemsToRemove.Add(collection.items[j]);
                }

                if (listReturn.Count >= requiredItems)
                {
                    break;  
                }
            }
        }

        foreach (var item in itemsToRemove)
        {
            foreach (var collection in manager.itemCollections)
            {
                if (collection.items.Contains(item))
                {
                    collection.items.Remove(item);
                    break;
                }
            }
        }

        return listReturn.Count > 0 ? listReturn : null;
    }
    
    private List<Item> GetListFrontBack()
    {
        List<Item> items = new List<Item>();

        for (int i = 0; i < boxes.Count; i++)
        {
            if (boxes[i].boxType != BoxType.Normal)
            {
                continue;
            }

            for (int j = 0; j < boxes[i].frontRow.itemPositions.Count; j++)
            {
                var itemPos = boxes[i].frontRow.itemPositions[j];
                if (itemPos.IsHoldingItem)
                {
                    items.Add(itemPos.itemHolding);
                    itemPos.itemHolding.ShowItemImmediately("Item Front");
                    itemPos.itemHolding.MoveToShuffle();
                }
            }

            if (boxes[i].backRow == null)
            {
                continue;
            }

            for (int j = 0; j < boxes[i].backRow.itemPositions.Count; j++)
            {
                var itemPos = boxes[i].backRow.itemPositions[j];
                if (itemPos.IsHoldingItem)
                {
                    items.Add(itemPos.itemHolding);
                    itemPos.itemHolding.ShowItemImmediately("Item Front");
                    itemPos.itemHolding.MoveToShuffle();
                }
            }
        }

        int n = items.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (items[k], items[n]) = (items[n], items[k]);
        }

        return items;
    }

    public void Shuffle()
    {
        UIManager.Instance.BlockClick();
        var list = GetListFrontBack();
        StartCoroutine(ReAssignItems(list));
    }

    private IEnumerator ReAssignItems(List<Item> items)
    {
        yield return new WaitForSeconds(1f);

        int itemIndex = 0;

        while (itemIndex < items.Count)
        {
            foreach (var box in boxes)
            {
                if (box.boxType != BoxType.Normal) continue;

                itemIndex = PlaceItemsInRow(box.frontRow.itemPositions, items, itemIndex, box);
                if (itemIndex >= items.Count) yield break;

                if (box.backRow != null)
                {
                    itemIndex = PlaceItemsInRow(box.backRow.itemPositions, items, itemIndex, box);
                    if (itemIndex >= items.Count) yield break;
                }
            }
        }

        UIManager.Instance.DeBlockClick(0.5f);
    }

    private int PlaceItemsInRow(List<ItemPosition> itemPositions, List<Item> items, int itemIndex, Box box)
    {
        foreach (var itemPos in itemPositions)
        {
            if (itemPos.IsHoldingItem) continue;

            if (itemIndex >= items.Count) return itemIndex; 

            if (Random.Range(0, 2) == 1)
            {
                itemPos.itemHolding = items[itemIndex];
                items[itemIndex].SetHolder(itemPos);
                items[itemIndex].MoveToItemPos(itemPos);
                items[itemIndex].ChangeColor(box);
                itemIndex++;

                if (itemIndex >= items.Count) return itemIndex; 
            }
        }

        return itemIndex;
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
    public List<ItemCollection> itemCollections = new();

    public bool ContainsItem(int itemID)
    {
        for (int i = 0; i < itemCollections.Count; i++)
        {
            if (itemCollections[i].itemID == itemID)
            {
                return true;
            }
        }

        return false;
    }

    public void AddItem(Item item)
    {
        if (!ContainsItem(item.itemID))
        {
            List<Item> newItemList = new List<Item>();
            newItemList.Add(item);
            itemCollections.Add(new ItemCollection(newItemList, item.itemID));
        }
        else
        {
            for (int i = 0; i < itemCollections.Count; i++)
            {
                if (itemCollections[i].itemID == item.itemID)
                {
                    itemCollections[i].items.Add(item);
                }
            }
        }
    }

    public void RemoveItem(Item item)
    {
        if (!ContainsItem(item.itemID))
        {
            return;
        }

        for (int i = 0; i < itemCollections.Count; i++)
        {
            if (itemCollections[i].itemID == item.itemID)
            {
                if (itemCollections[i].items.Count <= 1)
                {
                    itemCollections.RemoveAt(i);
                }
                else
                {
                    itemCollections[i].items.Remove(item);
                }

                return;
            }
        }
    }
}

[Serializable]
public class ItemCollection
{
    public List<Item> items;
    public int itemID;

    public ItemCollection(List<Item> items, int itemID)
    {
        this.items = items;
        this.itemID = itemID;
    }
}