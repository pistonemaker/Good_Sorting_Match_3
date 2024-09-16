using UnityEngine;

public class MovingBox : Box
{
    public int speed;
    public int bound;

    protected override void OnEnable()
    {
        base.OnEnable();
        bound = 6;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * (0.5f * (speed * Time.deltaTime)));

        if (transform.position.x < -bound)
        {
            transform.position = new Vector2(bound, transform.position.y);
        }

        else if (transform.position.x > bound)
        {
            transform.position = new Vector2(-bound, transform.position.y);
        }
    }

    protected override void SetSpecialBoxData(BoxData boxData)
    {
        base.SetSpecialBoxData(boxData);
        speed = boxData.speed;
    }

    public override void SaveBoxData(BoxData boxData)
    {
        base.SaveBoxData(boxData);
        boxData.speed = speed;
    }
}
