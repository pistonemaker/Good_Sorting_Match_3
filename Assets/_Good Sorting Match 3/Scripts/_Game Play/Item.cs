using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool canDrag;
    public int itemID;
    public int idInRow;
    public int rowID;
    public int boxID;
    public bool isDragging;
    public Transform oldTrf;
    public ItemPosition holder;
    public SpriteRenderer sr;
    public BoxCollider2D coll;

    public void Init(ItemPosition itemPosition, int id)
    {
        itemID = id;
        coll = GetComponent<BoxCollider2D>();
        GetSprite();
        idInRow = itemPosition.idInRow;
        rowID = itemPosition.rowID;
        holder = itemPosition;
        oldTrf = itemPosition.transform;
        transform.position = oldTrf.position;
        canDrag = true;
    }

    public void GetSprite()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameManager.Instance.itemData.itemSprites[itemID];
    }

    public void MoveToBox(ItemPosition target, System.Action onComplete = null)
    {
        holder.itemHolding = null;
        transform.SetParent(target.transform);
        holder = target;
        target.itemHolding = this;
        this.PostEvent(EventID.On_Check_Row_Empty, boxID);

        rowID = target.rowID;
        idInRow = target.idInRow;
        boxID = target.boxID;

        transform.DOMove(target.transform.position, 0.25f).OnComplete(() => { onComplete?.Invoke(); });
    }

    private void Bounce()
    {
        transform.DOScaleY(0.8f, 0.15f).OnComplete(() =>
        {
            transform.DOScaleY(1f, 0.15f).OnComplete(() =>
            {
                EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Lose);
            });
        });
    }

    public void BounceMatch3()
    {
        holder.itemHolding = null;
        transform.DOScaleY(0.8f, 0.15f).OnComplete(() => 
        { 
            transform.DOScaleY(1f, 0.15f).OnComplete(() =>
            {
                holder = null;
                PoolingManager.Despawn(gameObject); 
            }); 
        });
    }

    public void BackToOldPosition()
    {
        var duration = 0.25f;
        var newPos = new Vector3(holder.transform.position.x + duration / 2f * holder.row.box.Speed, 
            holder.transform.position.y, holder.transform.position.z);
        transform.DOMove(newPos, duration);
    }

    public void ShowItem()
    {
        sr.DOColor(Color.white, 0.5f);
        sr.sortingLayerName = "Item Front";
        coll.enabled = true;
    }

    public void GrayItem()
    {
        sr.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        sr.sortingLayerName = "Item";
        coll.enabled = false;
    }

    private void OnMouseDown()
    {
        if (!canDrag)
        {
            return;
        }
        
        oldTrf = holder.transform;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || !canDrag)
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
        if (!canDrag)
        {
            return;
        }
        
        isDragging = false;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var boxTarget = GameController.Instance.GetNearestBox(worldPosition);

        if (!boxTarget.CanGetItem)
        {
            BackToOldPosition();
        }
        else
        {
            var target = boxTarget.frontRow.GetNerestEmptyPositions(this);
            
            if (target.IsHoldingItem)
            {
                BackToOldPosition();
                return;
            }

            MoveToBox(target, () =>
            {
                bool isMatch3 = boxTarget.frontRow.CanMatch3();

                if (isMatch3)
                {
                    boxTarget.Match3();
                }
                else
                {
                    Bounce();
                }
            });
        }
    }
}