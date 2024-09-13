using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Box : MonoBehaviour
{
    protected BoxData data;
    public BoxType boxType;
    public int boxID;
    public int curRow;
    public int maxItemPositionInRow = 3;
    public bool isSpecialBox = false;
    public BoxRow frontRow;
    public BoxRow backRow;
    public List<BoxRow> rows;

    public void Init(BoxData boxData)
    {
        data = boxData;
    }

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
    
    public void Validate()
    {
        boxID = transform.GetSiblingIndex();
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

    public virtual int SetSpecialData()
    {
        return 0;
    }
}