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

    public override int SetSpecialData()
    {
        return speed;
    }
}
