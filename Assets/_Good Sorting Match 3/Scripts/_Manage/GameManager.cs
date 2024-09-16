using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Item itemPrefab;
    public ItemPosition itemPositionPrefab;
    public BoxRow boxRowPrefab;
    public Box boxPrefab;
    public LockedBox lockedBoxPrefab;
    public MovingBox movingBoxPrefab;
    public OneShotBox oneShotBoxPrefab;
    public GameObject hpBarPrefab;

    public ItemData itemData;
}
