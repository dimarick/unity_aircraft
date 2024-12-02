using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform brick;
    public int radius = 10;
    public int height = 50;

    void Start()
    {
        var sectors = Mathf.FloorToInt(radius * Mathf.PI * 2);
        var epsilon = 1.001F;
        var radiusFixed = 1 / (2 * Mathf.Tan(Mathf.PI / sectors)) * epsilon;
        var sectorSize = 360F / sectors;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < sectors; j++)
            {
                var o = Instantiate(brick, transform);
                o.localPosition = new Vector3(0, i + 0.5f, 0);
                o.Rotate(0, sectorSize * j + (i % 2 == 0 ? 0 : sectorSize / 2), 0);
                o.Translate(new Vector3(radiusFixed + 0.5F, 0, 0));
            }
        }
    }
}
