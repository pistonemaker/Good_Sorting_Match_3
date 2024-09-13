using System;

public class LockedBox : Box
{
    public int lockedTurn;

    private void OnEnable()
    {
        isSpecialBox = true;
    }

    public override bool CanGetItem
    {
        get
        {
            return lockedTurn == 0;
        }
    }

    public override int SetSpecialData()
    {
        return lockedTurn;
    }
}
