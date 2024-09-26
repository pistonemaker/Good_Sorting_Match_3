using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class EditLevelManager : Singleton<EditLevelManager>
{
    public LevelData levelData;
    public List<Box> boxes;
    public ItemManager itemManager;

    private void Start()
    {
        boxes = GetComponentsInChildren<Box>().ToList();

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].boxID = i;
            boxes[i].Validate();
        }
    }

    public void SaveData()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not attached to EditLevelManager!");
            return;
        }

        if (levelData.boxData == null || levelData.boxData.Count != boxes.Count)
        {
            levelData.boxData = new List<BoxData>();
            for (int i = 0; i < boxes.Count; i++)
            {
                levelData.boxData.Add(new BoxData());
            }
        }

        for (int i = 0; i < boxes.Count; i++)
        {
            boxes[i].SaveBoxData(levelData.boxData[i]);
        }

        Debug.Log("Data saved successfully.");
    }

    public void AutoGenerateItems(int totalItemCount, int itemTypeCount)
    {
        if (totalItemCount <= 0 || itemTypeCount <= 0 || totalItemCount % 3 != 0)
        {
            Debug.LogError("Invalid total item count. It must be divisible by 3.");
            return;
        }

        int remainingItems = totalItemCount;
        Dictionary<int, int> itemsPerType = new Dictionary<int, int>();

        for (int i = 0; i < itemTypeCount; i++)
        {
            if (i == itemTypeCount - 1)
            {
                // Loại item cuối cùng sẽ nhận số lượng item còn lại
                itemsPerType[i] = remainingItems;
            }
            else
            {
                // Chia số lượng item ngẫu nhiên cho từng loại, phải chia hết cho 3
                int itemCount = Random.Range(3, remainingItems - (itemTypeCount - i - 1) * 3 + 1); 
                itemCount -= itemCount % 3; // Đảm bảo chia hết cho 3
                itemsPerType[i] = itemCount;
                remainingItems -= itemCount;
            }
        }

        // Danh sách để kiểm tra các vị trí còn trống trong boxes
        List<ItemPosition> allItemPositions = new List<ItemPosition>();

        foreach (var box in boxes)
        {
            foreach (var row in box.rows)
            {
                foreach (var itemPosition in row.itemPositions)
                {
                    if (!itemPosition.IsHoldingItem)
                    {
                        allItemPositions.Add(itemPosition);
                    }
                }
            }
        }

        // Đảm bảo rằng có đủ vị trí trống cho số lượng item
        if (allItemPositions.Count < totalItemCount)
        {
            Debug.LogError("Not enough available positions for all items.");
            return;
        }

        // Shuffle các vị trí ngẫu nhiên
        allItemPositions = allItemPositions.OrderBy(x => Random.value).ToList();

        // Phân phối item vào các vị trí
        foreach (var itemType in itemsPerType)
        {
            int itemID = itemType.Key;
            int itemCount = itemType.Value;

            for (int i = 0; i < itemCount; i++)
            {
                ItemPosition position = allItemPositions[i];
                position.Init(new ItemPosData { itemID = itemID }, position.idInRow);
            }

            // Xóa những vị trí đã được gán item
            allItemPositions.RemoveRange(0, itemCount);
        }

        Debug.Log($"{totalItemCount} items of {itemTypeCount} types have been generated");
    }
}