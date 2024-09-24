using TMPro;
using UnityEngine;

public class LockedBox : Box
{
    public int lockedTurn;
    public TextMeshPro lockText;
    public GameObject mask;
    public GameObject locked;
    public GameObject lockedNumber;
    
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
        mask.SetActive(false);
        locked.SetActive(false);
        lockedNumber.SetActive(false);
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isSpecialBox = true;
    }

    public void DecreaseLockTurn()
    {
        lockedTurn--;
        lockText.text = lockedTurn.ToString();
        
        if (lockedTurn <= 0)
        {
            UnBlockDrag();
        }
    }

    protected override void SetSpecialBoxData(BoxData boxData)
    {
        base.SetSpecialBoxData(boxData);
        lockedTurn = boxData.lockedTurn;
        lockText.text = lockedTurn.ToString();
        BlockDrag();
    }
    
    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.lockedTurn = lockedTurn;
    }
}
