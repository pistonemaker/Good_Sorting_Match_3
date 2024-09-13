using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public LevelData levelData;
    public List<Box> boxes;

    public Box GetNearestBox(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int minDistanceIndex = 0;

        for (int i = 0; i < boxes.Count; i++)
        {
            float distance = Vector3.Distance(boxes[i].transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                minDistanceIndex = i;
            }
        }

        return boxes[minDistanceIndex];
    }
}
