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
            FindMatch3(4);
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

    public bool HaveMatch3(ItemManager itemManager)
    {
        for (int i = 0; i < itemManager.items.Count; i++)
        {
            if (itemManager.items[i].amount > 0 && itemManager.items[i].amount % 3 == 0)
            {
                return true;
            }
        }

        return false;
    }

    // public List<Item> Match3Front()
    // {
    //     ItemManager manageFront = new ItemManager();
    //     List<Item> listreturn = new List<Item>();
    //
    //     for (int i = 0; i < boxes.Count; i++)
    //     {
    //         if (boxes[i].boxType != BoxType.Normal)
    //         {
    //             continue;
    //         }
    //         
    //         var box = boxes[i];
    //         for (int j = 0; j < box.frontRow.itemPositions.Count; j++)
    //         {
    //             var itemPos = box.frontRow.itemPositions[j];
    //             if (!itemPos.IsHoldingItem)
    //             {
    //                 continue;
    //             }
    //
    //             var item = itemPos.itemHolding;
    //             manageFront.AddItem(item.itemID);
    //         }
    //     }
    // }

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

    public void FindMatch33(int match3Number)
    {
        
    }

    public void FindMatch3(int match3Number)
    {
        for (int a = 0; a < match3Number; a++)
        {
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
                    item.ShowItemImmediately("UI Behind");
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
        int idran2 = Random.Range(0, itemManager.items.Count);
        int idbecome = itemManager.items[idran2].id;
        
        for (int a = 0; a < 3; a++)
        {
            if (itemManager.items.Count == 0)
            {
                EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
                break;
            }
            
            int idran = Random.Range(0, itemManager.items.Count);
            int id = itemManager.items[idran].id;
            int numberToFind = 3;

            for (int i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                if (numberToFind <= 0)
                {
                    break;
                }
                
                if (box.boxType == BoxType.Locked)
                {
                    continue;
                }
                
                if (box.IsEmpty)
                {
                    continue;
                }

                for (int j = 0; j < box.frontRow.itemPositions.Count; j++)
                {
                    if (box.frontRow.IsEmpty)
                    {
                        break;
                    }
                    
                    if (!box.frontRow.itemPositions[j].IsHoldingItem)
                    {
                        continue;
                    }

                    if (box.frontRow.itemPositions[j].itemHolding.itemID == id)
                    {
                        var item = box.frontRow.itemPositions[j].itemHolding;
                        numberToFind--;
                        itemManager.RemoveItem(id);
                        itemManager.AddItem(idbecome);
                        var spawnPos = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
                        var light = PoolingManager.Spawn(GameManager.Instance.changeLightPrefab, spawnPos, Quaternion.identity);
                        light.SetTarget(item.transform);
                        StartCoroutine(item.ChangeItemID(idbecome));
                    }
                }
            }
            
            for (int i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                if (numberToFind <= 0)
                {
                    break;
                }
                
                if (box.boxType == BoxType.Locked)
                {
                    continue;
                }
                
                if (box.IsEmpty)
                {
                    continue;
                }

                if (boxes[i].backRow == null)
                {
                    continue;
                }
                
                for (int j = 0; j < box.backRow.itemPositions.Count; j++)
                {
                    if (box.backRow.IsEmpty)
                    {
                        break;
                    }
                    
                    if (!box.backRow.itemPositions[j].IsHoldingItem)
                    {
                        continue;
                    }

                    if (box.backRow.itemPositions[j].itemHolding.itemID == id)
                    {
                        var item = box.backRow.itemPositions[j].itemHolding;
                        numberToFind--;
                        itemManager.RemoveItem(id);
                        itemManager.AddItem(idbecome);
                        var spawnPos = new Vector3(item.transform.position.x, item.transform.position.y + 0.5f, item.transform.position.z);
                        var light = PoolingManager.Spawn(GameManager.Instance.changeLightPrefab, spawnPos, Quaternion.identity);
                        light.SetTarget(item.transform);
                        StartCoroutine(item.ChangeItemID(idbecome));
                    }
                }
            }
        }
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

    private IEnumerator AssignItem(List<Item> items)
    {
        yield return new WaitForSeconds(1f);
        int itemIndex = 0; 

        for (int i = 0; i < boxes.Count; i++)
        {
            if (boxes[i].boxType != BoxType.Normal)
            {
                continue;
            }

            for (int j = 0; j < boxes[i].frontRow.itemPositions.Count && itemIndex < items.Count; j++)
            {
                var itemPos = boxes[i].frontRow.itemPositions[j];
                if (!itemPos.IsHoldingItem) 
                {
                    itemPos.itemHolding = items[itemIndex];
                    items[itemIndex].SetHolder(itemPos);
                    items[itemIndex].MoveToItemPos(itemPos);
                    items[itemIndex].ChangeColor(boxes[i]);
                    itemIndex++; 
                }
            }

            if (boxes[i].backRow == null)
            {
                continue;
            }

            for (int j = 0; j < boxes[i].backRow.itemPositions.Count && itemIndex < items.Count; j++)
            {
                var itemPos = boxes[i].backRow.itemPositions[j];
                if (!itemPos.IsHoldingItem)
                {
                    itemPos.itemHolding = items[itemIndex];
                    items[itemIndex].SetHolder(itemPos);
                    items[itemIndex].MoveToItemPos(itemPos);
                    items[itemIndex].ChangeColor(boxes[i]);
                    itemIndex++; 
                }
            }
        }
    }

    public void Shuffle()
    {
        var list = GetListFrontBack();
        StartCoroutine(AssignItem(list));
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