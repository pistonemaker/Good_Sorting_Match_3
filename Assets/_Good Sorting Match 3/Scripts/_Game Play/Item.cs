using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID;
    public int idInRow;
    public int rowID;
    public bool isDragging;
    public Transform oldTrf;
    public ItemPosition holder;
    public SpriteRenderer sr;
    public BoxCollider2D coll;

    private void OnEnable()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void GetSprite()
    {
        sr.sprite = GameManager.Instance.itemData.itemSprites[itemID];
    }

    public void MoveToBox(ItemPosition target)
    {
        transform.DOMove(target.transform.position, 0.25f).OnComplete(() =>
        {
            holder.itemHolding = null;
            transform.SetParent(target.transform);
        });
    }

    public void BackToOldPosition()
    {
        transform.DOMove(oldTrf.position, 0.25f);
    }

    private void OnMouseDown()
    {
        oldTrf = holder.transform;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
        {
            return;
        }
        
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y - 0.5f, worldPosition.z);
        transform.position = worldPosition;
    }

    private void OnMouseUp()
    {
        var boxTarget = GameController.Instance.GetNearestBox(transform.position);
        if (!boxTarget.CanGetItem)
        {
            BackToOldPosition();
        }
        else
        {
            var target = boxTarget.frontRow.GetNerestEmptyPositions(this);
            MoveToBox(target);
        }
    }
}
