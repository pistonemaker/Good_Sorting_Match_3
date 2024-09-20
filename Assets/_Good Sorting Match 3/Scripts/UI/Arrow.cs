using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public WinPanel winPanel;
    private float bound;
    private float speed;

    private void OnEnable()
    {
        bound = 1.7f; 
        speed = 2f;   
    }

    private void Update()
    {
        transform.Translate(Vector2.right * (speed * Time.deltaTime));

        if (transform.position.x > bound)
        {
            transform.position = new Vector2(bound, transform.position.y); 
            speed = -speed; 
        }

        if (transform.position.x < -bound)
        {
            transform.position = new Vector2(-bound, transform.position.y); 
            speed = -speed; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var name = other.gameObject.name;
        int length = name.Length;
        int multiplier = int.Parse(name[length - 1].ToString());
        winPanel.ExtraReward(multiplier); 
    }
}