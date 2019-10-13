using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public Vector3 size = new Vector3(10,10,10);
    Noise noise = new Noise();
    private void Start()
    {
        //create the voxels
        for(int x = 0; x  < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    float v = noise.Evaluate(new Vector3(x, y, z));

                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    sphere.transform.position = new Vector3(x, y, z);
                    Color color = new Color(0, 0, v*100, v);
                    sphere.GetComponent<MeshRenderer>().material.color = color;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
