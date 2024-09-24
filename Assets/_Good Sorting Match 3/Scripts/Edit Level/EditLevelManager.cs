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
        // Kiểm tra tính hợp lệ của số lượng item và số loại item
        if (totalItemCount <= 0 || itemTypeCount <= 0 || totalItemCount % 3 != 0)
        {
            Debug.LogError("Invalid total item count or item type count.");
            return;
        }

        int itemsPerType = totalItemCount / itemTypeCount;

        // Tạo một danh sách để theo dõi số lượng đã sinh ra cho từng loại item
        Dictionary<int, int> itemsGenerated = new Dictionary<int, int>();
        for (int i = 0; i < itemTypeCount; i++)
        {
            itemsGenerated[i] = 0; // Khởi tạo số lượng sinh ra cho từng loại item
        }

        foreach (var box in boxes)
        {
            for (int rowIndex = 0; rowIndex < box.rows.Count; rowIndex++)
            {
                var row = box.rows[rowIndex];

                // Chọn số lượng item ngẫu nhiên để sinh trong hàng này (tối đa là 3)
                int maxItemsToPlace = Random.Range(1, 4); // Số item từ 1 đến 3
                int itemsPlaced = 0;

                // Sinh item cho từng vị trí trong hàng
                for (int itemPositionIndex = 0; itemPositionIndex < row.itemPositions.Count; itemPositionIndex++)
                {
                    if (itemsPlaced >= maxItemsToPlace || itemsGenerated.Values.Sum() >= totalItemCount)
                        break; // Dừng nếu đã đạt số lượng tối đa hoặc số lượng tổng đã đạt

                    // Kiểm tra xem có thể sinh item cho vị trí này không
                    var itemPosition = row.itemPositions[itemPositionIndex];
                    if (Random.Range(0, 2) == 0) // 50% cơ hội để không sinh item cho vị trí này
                        continue;

                    // Kiểm tra xem có thể sinh thêm item của loại này không
                    int itemID = Random.Range(0, itemTypeCount);
                    if (itemsGenerated[itemID] < itemsPerType)
                    {
                        itemPosition.Init(new ItemPosData { itemID = itemID }, itemPositionIndex);
                        itemsGenerated[itemID]++;
                        itemsPlaced++;
                    }
                }
            }
        }

        // Bước bổ sung để kiểm tra các hàng còn trống và điền item vào
        foreach (var box in boxes)
        {
            foreach (var row in box.rows)
            {
                int emptyPositions = row.itemPositions.Count(itemPosition => !itemPosition.IsHoldingItem);
                int remainingItems = totalItemCount - itemsGenerated.Values.Sum();

                // Nếu còn item cần sinh và hàng có ô trống
                if (remainingItems > 0 && emptyPositions > 0)
                {
                    int itemsToPlace = Random.Range(0, emptyPositions + 1); // Từ 0 đến số ô trống

                    for (int i = 0; i < itemsToPlace && remainingItems > 0; i++)
                    {
                        // Tìm một ô trống để sinh item
                        for (int itemPositionIndex = 0; itemPositionIndex < row.itemPositions.Count; itemPositionIndex++)
                        {
                            var itemPosition = row.itemPositions[itemPositionIndex];
                            if (!itemPosition.IsHoldingItem)
                            {
                                int itemID = Random.Range(0, itemTypeCount);
                                if (itemsGenerated[itemID] < itemsPerType)
                                {
                                    itemPosition.Init(new ItemPosData { itemID = itemID }, itemPositionIndex);
                                    itemsGenerated[itemID]++;
                                    remainingItems--;
                                    break; // Điền vào ô này và ra khỏi vòng lặp
                                }
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"{totalItemCount} items generated successfully.");
    }
}