using System;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Transform brick;
    public Rigidbody brickRB;
    public int width = 200;
    public int height = 100;
    private new BoxCollider collider;

    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var o = Instantiate(brick, transform);
                o.position = new Vector3(j, i + 0.5F, -70);
            }
        }
        brickRB = brick.GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        collider.size = new Vector3(width, height, 1);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        brickRB.useGravity = true;
        
        Debug.Log("Use Gravity: true");
    }
}
