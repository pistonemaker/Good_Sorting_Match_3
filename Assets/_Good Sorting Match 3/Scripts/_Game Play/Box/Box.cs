using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Box : MonoBehaviour
{
    protected BoxData data;
    public BoxType boxType;
    public int boxID;
    public int curRowID;
    public bool isSpecialBox = false;
    public bool canPutItem = true;
    public BoxRow frontRow;
    public BoxRow backRow;
    public List<BoxRow> rows;
    public int itemPositionInRow = 3;

    public bool IsEmpty
    {
        get
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (!rows[i].IsEmpty)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public virtual bool CanGetItem
    {
        get => frontRow.CanGetItem;
    }

    public bool IsFull
    {
        get => frontRow.IsFull;
    }

    public int Speed
    {
        get => data.speed;
    }

    public void Init(BoxData boxData, int id)
    {
        data = boxData;
        boxType = boxData.boxType;
        boxID = id;
        isSpecialBox = boxData.isSpecialBox;
        name = boxData.name;
        curRowID = 0;


        for (int i = 0; i < boxData.rowData.Count; i++)
        {
            var row = PoolingManager.Spawn(GameManager.Instance.boxRowPrefab, transform.position, Quaternion.identity);
            row.transform.SetParent(transform);
            row.transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            row.transform.localScale = Vector3.one;
            row.box = this;
            row.boxID = boxID;
            row.Init(data.rowData[i], i);
            rows.Add(row);
        }

        AssignItem();
        SetSpecialBoxData(boxData);
    }

    protected virtual void OnEnable()
    {
        this.RegisterListener(EventID.On_Check_Row_Empty, param => CheckIfFrontRowEmpty((int)param));
        this.RegisterListener(EventID.On_Check_Match_3, param => CheckIfMatch3((int)param));
    }

    protected virtual void OnDisable()
    {
        this.RemoveListener(EventID.On_Check_Row_Empty, param => CheckIfFrontRowEmpty((int)param));
        this.RemoveListener(EventID.On_Check_Match_3, param => CheckIfMatch3((int)param));
    }

    protected virtual void SetSpecialBoxData(BoxData boxData)
    {
        if (!isSpecialBox)
        {
            return;
        }
    }

    // Giữ nguyên hàng 1, bôi đen hàng 2, tắt các hàng từ hàng 3 trở đi
    protected virtual void AssignItem()
    {
        int rowNumber = rows.Count;
        if (rowNumber == 0)
        {
            return;
        }

        SetFrontRow(rows[0]);

        for (int i = 1; i < rows.Count; i++)
        {
            rows[i].GrayRow();
        }

        if (rowNumber > 1)
        {
            SetBackRow(rows[1]);

            for (int i = rowNumber - 1; i > 1; i--)
            {
                rows[i].DeactiveRow();
            }
        }
        
        if (frontRow.IsEmpty)
        {
            Debug.LogWarning($"{name}: Front row is empty");
        }
    }

    protected void SetFrontRow(BoxRow boxRow)
    {
        frontRow = boxRow;
        frontRow.ActiveRow();
        frontRow.ShowRow();
    }

    protected virtual void SetBackRow(BoxRow boxRow)
    {
        backRow = boxRow;
        backRow.ActiveRow();
        backRow.GrayRow();
    }

    protected virtual void CheckIfFrontRowEmpty(int id)
    {
        if (id != boxID)
        {
            return;
        }

        if (!frontRow.IsEmpty)
        {
            return;
        }

        if (backRow == null || rows.Count <= 1)
        {
            return;
        }

        Debug.Log(name + " Front Row Empty");
        PoolingManager.Despawn(frontRow.gameObject);
        rows.Remove(frontRow);
        SetFrontRow(backRow);
        curRowID++;
        CheckIfMatch3(boxID);
        
        if (rows.Count > 1)
        {
            SetBackRow(rows[1]);
        }
        else
        {
            backRow = null;
            Debug.Log(name + " not have backrow to set to frontrow");
        }
    }

    protected void CheckIfBackRowEmpty()
    {
        if (backRow == null)
        {
            return;
        }
        
        if (!backRow.IsEmpty)
        {
            return;
        }

        if (rows.Count < 3)
        {
            return;
        }
        
        PoolingManager.Despawn(backRow.gameObject);
        SetBackRow(rows[2]);
    }

    public IEnumerator CheckRows()
    {
        AssignItem();

        for (int i = 0; i < frontRow.itemPositions.Count; i++)
        {
            if (frontRow.itemPositions[i].IsHoldingItem)
            {
                var item = frontRow.itemPositions[i].itemHolding;
                item.ShowItemImmediately("Item Front");
            }
        }
        
        for (int i = 1; i < rows.Count; i++)
        {
            rows[i].GrayRow();
        }

        if (rows.Count > 1)
        {
            SetBackRow(rows[1]);

            for (int i = rows.Count - 1; i > 1; i--)
            {
                rows[i].DeactiveRow();
            }
        }

        yield return new WaitForSeconds(0.75f);
        
        if (rows.Count <= 1)
        {
            yield return null;
        }

        while (frontRow == null || frontRow.IsEmpty)
        {
            CheckIfFrontRowEmpty(boxID);
            if (rows.Count <= 1)
            {
                yield return null;
            }
        }

        for (int i = 2; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.IsEmpty)
            {
                PoolingManager.Despawn(row.gameObject);
                rows.RemoveAt(i);
            }
        }
    }
    
    public void CheckIfMatch3(int id)
    {
        if (id != boxID)    
        {
            return;
        }

        if (frontRow.CanMatch3())
        {
            Match3();
            EventDispatcher.Instance.PostEvent(EventID.On_Check_Player_Win);
        }
    }

    public void Match3()
    {
        StartCoroutine(frontRow.Match3Item());
    }

    public void Validate()
    {
        rows = GetComponentsInChildren<BoxRow>().ToList();
        if (rows.Count > 0)
        {
            frontRow = rows[0];
        }

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].box = this;
            rows[i].rowID = i;
            rows[i].Validate();
        }
    }

    public virtual void SaveBoxData(BoxData boxData)
    {
        if (boxData.rowData == null || boxData.rowData.Count != rows.Count)
        {
            boxData.rowData = new List<RowData>();
            boxData.name = "Box " + (boxID + 1);
            boxData.boxType = boxType;
            boxData.boxPosition = transform.position;
            boxData.isSpecialBox = isSpecialBox;
            for (int i = 0; i < rows.Count; i++)
            {
                boxData.rowData.Add(new RowData());
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].SaveRowData(boxData.rowData[i]);
        }
    }
}