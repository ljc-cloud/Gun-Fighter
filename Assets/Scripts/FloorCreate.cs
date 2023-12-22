using UnityEngine;

public class FloorCreate : MonoBehaviour
{
    public GameObject FloorPre;
    public int len;
    void Start()
    {
        float x = 0;
        float z = 0;
        for (int i = 0; i < len; i++)
        {
            Vector3 xpos = new Vector3(x, 0, z);
            Instantiate(FloorPre, xpos, Quaternion.identity);
            for (int j = 0; j < len; j++)
            {
                z += 3;
                Vector3 zpos = new Vector3(x, 0, z);
                Instantiate(FloorPre, zpos, Quaternion.identity);
            }
            z = 0;
            x += 3;
        }

    }
}
