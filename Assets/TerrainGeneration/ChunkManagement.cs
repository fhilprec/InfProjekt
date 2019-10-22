using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManagement : MonoBehaviour
{
    public int gridsize = 4;
    public int octaves = 8;
    public float frequenzy;
    public int resolution;
    public float amplitude;
    public bool haschanged = true;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < gridsize; x++)
        {
            for (int y = 0; y < gridsize; y++)
            {
                
                    GameObject chunk = new GameObject();
                    chunk.transform.position = new Vector3(x, 0, y);    //funny mistake
                    chunk.AddComponent<NoiseTerrain>();
                    
                    chunk.transform.SetParent(gameObject.transform);
                    chunk.AddComponent<MeshRenderer>();
                    chunk.GetComponent<MeshRenderer>().material = mat;
                   
                
            }
        }
        
    }

    private void Update()
    {
        if (haschanged)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<NoiseTerrain>().haschanged = true;

            }
            haschanged = false;
        }
    }



}
