using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunkmanager : MonoBehaviour
{
    public int chunksize = 3;


    private void Start()
    {
        for (int x = 0; x < chunksize; x++)
        {
            for (int y = 0; y < chunksize; y++)
            {
                for (int z = 0; z < chunksize; z++)
                {
                    GameObject voxel = new GameObject();
                    voxel.transform.SetParent(gameObject.transform);
                    voxel.AddComponent<NoiseTerrain>();
                    voxel.gameObject.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}
