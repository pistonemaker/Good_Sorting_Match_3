using UnityEngine;

public class LockedBox : Box
{
    public int lockedTurn = 3;

    private void OnEnable()
    {
        isSpecialBox = true;
    }

    public override bool CanGetItem
    {
        get
        {
            Debug.Log("lockedTurn = " + lockedTurn);
            return lockedTurn == 0;
        }
    }

    protected override void CheckForSpecialBox(BoxData boxData)
    {
        base.CheckForSpecialBox(boxData);
        
        lockedTurn = boxData.lockedTurn;
    }
    
    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.lockedTurn = lockedTurn;
    }
}
