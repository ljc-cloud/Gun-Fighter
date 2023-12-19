using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCreate : MonoBehaviour
{
    public GameObject FloorPre;
    void Start()
    {
        float x = 0;
        float z = 0;
        for (int i = 0; i < 50; i++)
        {
            Vector3 xpos = new Vector3(x, 0, z);
            Instantiate(FloorPre, xpos, Quaternion.identity);
            for (int j = 0; j < 50; j++)
            {
                z += 3;
                Vector3 zpos = new Vector3(x, 0, z);
                Instantiate(FloorPre, zpos, Quaternion.identity);
            }
            z = 0;
            x += 3;

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
