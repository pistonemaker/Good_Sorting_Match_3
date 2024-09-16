using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditLevelManager : Singleton<EditLevelManager>
{
    public LevelData levelData;
    public List<Box> boxes;

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
}
