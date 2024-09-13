using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class EditLevelManager : Singleton<EditLevelManager>
{
    public LevelData levelData;
    
    public List<Box> boxes;

    private void Start()
    {
        boxes = GetComponentsInChildren<Box>().ToList();
        
        foreach (var box in boxes)
        {
            box.Validate();
        }
    }

    private void SaveBoxData()
    {
        for (int i = 0; i < levelData.boxData.Count; i++)
        {
            levelData.boxData[i].boxType = boxes[i].boxType;
            levelData.boxData[i].boxPosition = boxes[i].transform.position;
            levelData.boxData[i].isSpecialBox = boxes[i].isSpecialBox;
            levelData.boxData[i].lockedTurn = boxes[i].SetSpecialData();
            levelData.boxData[i].speed = boxes[i].SetSpecialData();
            levelData.boxData[i].hp = boxes[i].SetSpecialData();
        }
    }
}
