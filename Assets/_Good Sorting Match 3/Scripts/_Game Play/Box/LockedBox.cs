using UnityEngine;

public class LockedBox : Box
{
    public int lockedTurn;
    
    public override bool CanGetItem
    {
        get
        {
            Debug.Log("lockedTurn = " + lockedTurn);
            return lockedTurn == 0;
        }
    }

    private void BlockDrag()
    {
        frontRow.BlockDragItem();
    }

    private void UnBlockDrag()
    {
        frontRow.UnBlockDragItem();
        boxType = BoxType.Normal;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isSpecialBox = true;
    }

    public void DecreaseLockTurn()
    {
        lockedTurn--;
        
        if (lockedTurn <= 0)
        {
            UnBlockDrag();
        }
    }

    protected override void SetSpecialBoxData(BoxData boxData)
    {
        base.SetSpecialBoxData(boxData);
        lockedTurn = boxData.lockedTurn;
        BlockDrag();
    }
    
    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.lockedTurn = lockedTurn;
    }
}
