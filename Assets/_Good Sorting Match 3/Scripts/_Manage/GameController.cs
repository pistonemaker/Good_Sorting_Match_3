using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public LevelData levelData;
    public List<Box> boxes;
    public List<LockedBox> lockedBoxes;

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
}
