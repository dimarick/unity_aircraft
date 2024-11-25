using UnityEngine;

public class Wall : MonoBehaviour
{
    public Transform brick;
    public Rigidbody brickRB;
    public int width = 200;
    public int height = 100;
    public int depth = 10;
    private new BoxCollider collider;

    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    var o = Instantiate(brick, transform);
                    o.localPosition = new Vector3(i % 2 == 0 ? j : j + 0.5F, i + 0.5F, (i % 2 == 0 ? k : k + 0.5F));
                }
            }
        }
    }
}
