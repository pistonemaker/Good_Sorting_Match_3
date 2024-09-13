using UnityEngine;

public class MovingBox : Box
{
    public bool isMovingLeft;
    public int speed;

    private void OnEnable()
    {
        if (isMovingLeft)
        {
            speed = -speed;
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.right * (speed * Time.deltaTime));
    }

    protected override void CheckForSpecialBox(BoxData boxData)
    {
        base.CheckForSpecialBox(boxData);
        
        speed = boxData.speed;
    }

    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.speed = speed;
    }
}
