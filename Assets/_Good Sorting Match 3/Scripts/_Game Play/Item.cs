using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

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
        SetHolder(itemPosition);
        transform.position = oldTrf.position;
        canDrag = true;
        GameController.Instance.itemManager.AddItem(this);
        GameController.Instance.itemList.Add(this);
    }

    public void SetHolder(ItemPosition itemPosition)
    {
        idInRow = itemPosition.idInRow;
        rowID = itemPosition.rowID;
        boxID = itemPosition.boxID;
        holder = itemPosition;
        oldTrf = itemPosition.transform;
        name = "Item " + itemID + " Pos " + holder.idInRow + " Row " + rowID + " Box " + boxID;
    }

    public void GetSprite()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameManager.Instance.itemData.itemSprites[itemID];
    }

    public void MoveToBox(ItemPosition target, System.Action onComplete = null)
    {
        coll.enabled = false;
        holder.itemHolding = null;
        transform.SetParent(target.transform);
        holder = target;
        target.itemHolding = this;
        this.PostEvent(EventID.On_Check_Row_Empty, boxID);

        rowID = target.rowID;
        idInRow = target.idInRow;
        boxID = target.boxID;

        transform.DOMove(target.transform.position, 0.25f).OnComplete(() =>
        {
            coll.enabled = true;
            sr.sortingLayerName = "Item Front";
            onComplete?.Invoke();
        });
    }

    public void MoveToCenter(System.Action onComplete = null)
    {
        Debug.Log(name);
        coll.enabled = false;
        holder.itemHolding = null;
        transform.SetParent(null);
        holder = null;
        this.PostEvent(EventID.On_Check_Row_Empty, boxID);

        transform.DOMove(Vector3.zero, 1f).OnComplete(() =>
        {
            coll.enabled = true;
            PoolingManager.Despawn(gameObject);
            onComplete?.Invoke();
        });
    }

    public void MoveToShuffle()
    {
        coll.enabled = false;
        holder.itemHolding = null;
        transform.SetParent(null);
        holder = null;
        transform.DOMove(Vector3.zero, 1f).OnComplete(() =>
        {
            coll.enabled = true;
        });
    }

    public void MoveToItemPos(ItemPosition itemPosition)
    {
        transform.localScale = 0.5f * Vector3.one;
        coll.enabled = false;
        transform.SetParent(itemPosition.transform);
        transform.DOMove(itemPosition.transform.position, 1f).OnComplete(() =>
        {
            coll.enabled = true;
            this.PostEvent(EventID.On_Check_Match_3, boxID);
        });
    }

    public void ChangeColor(Box box)
    {
        if (rowID == box.curRowID)
        {
            ShowItemImmediately("Item Front");
            return;
        }

        if (rowID == box.curRowID + 1)
        {
            GrayItem();
        }
        else
        {
            GrayItem();
            gameObject.SetActive(false);
        }
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
                GameController.Instance.itemManager.RemoveItem(this);
                GameController.Instance.itemList.Remove(this);
                PoolingManager.Despawn(gameObject);
            });
        });
    }

    public void BackToOldPosition()
    {
        coll.enabled = false;
        var duration = 0.25f;
        var newPos = new Vector3(holder.transform.position.x + duration / 2f * holder.row.box.Speed,
            holder.transform.position.y, holder.transform.position.z);
        transform.DOMove(newPos, duration).OnComplete(() => {coll.enabled = true; });
    }

    public void ShowItem()
    {
        sr.DOColor(Color.white, 0.5f);
        sr.sortingLayerName = "Item Front";
        coll.enabled = true;
    }

    public void ShowItemImmediately(string sortingLayerName)
    {
        sr.color = Color.white;
        sr.sortingLayerName = sortingLayerName;
        coll.enabled = true;
        //canDrag = true;
    }

    public IEnumerator ChangeItemID(int idbecome, float time = 1f)
    {
        itemID = idbecome;
        gameObject.SetActive(true);
        ShowItemImmediately("UI Behind");
        yield return new WaitForSeconds(time);
        RestoreItems(idbecome);
    }

    public void GrayItem()
    {
        sr.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        sr.sortingLayerName = "Item";
        coll.enabled = false;
        //canDrag = false;
    }

    private void RestoreItems(int idbecome)
    {
        sr.sprite = GameManager.Instance.itemData.itemSprites[idbecome];
        this.PostEvent(EventID.On_Check_Match_3, boxID);

        if (rowID == holder.row.box.curRowID)
        {
            return;
        }

        if (rowID == holder.row.box.curRowID + 1)
        {
            GrayItem();
        }
        else
        {
            GrayItem();
            gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (!canDrag || IsMouseOverUIElement())
        {
            return;
        }

        EventDispatcher.Instance.PostEvent(EventID.On_Start_Countdown_Time);
        oldTrf = holder.transform;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || !canDrag)
        {
            return;
        }

        sr.sortingLayerName = "Item Drag";
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
        
        sr.sortingLayerName = "Item Front";
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

    private bool IsMouseOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}