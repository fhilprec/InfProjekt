using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    public GameObject debugsphere;
    private int[,,] setsphere;
    public int chunkresolution;
    public float threshhold;
    public float frequenzy = 0.1f;
    Noise noise = new Noise();
    public GameObject cube;
    int j = 0;
    int i = 0;
    int k = 0;
    float time = 0;
    public float cubespeed;
    TriAngulationTable tri = new TriAngulationTable();

    private float density(Vector3 point)        //return noisevalues between 0 and 1
    {
        float v;

        v = (noise.Evaluate(point * frequenzy) + 1) / 2;

        return v;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        int[,,] setsphere = new int[chunkresolution+1, chunkresolution+1, chunkresolution+1];     //initialize the 3d array for determining where spheres are set

        //threefold for loop for looping through each dimension

        for (int x = 0; x < chunkresolution+1; x++)
        {
            for (int y = 0; y < chunkresolution+1; y++)
            {
                for (int z  = 0; z  < chunkresolution+1; z ++)
                {
                    if(density(new Vector3(x,y,z) + gameObject.transform.position) > threshhold){   //if bigger than threshhold place a sphere
                        setsphere[x, y, z] = 1;

                        //spawning a sphere on those positions for debug purposes
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        sphere.transform.position = new Vector3((float)x/chunkresolution, (float)y/chunkresolution, (float)z/chunkresolution )+ gameObject.transform.position;

                    }
                    else
                    {
                        setsphere[x, y, z] = 0;
                    }
                    
                }
            }
        }
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3((float)1 / chunkresolution, (float)1 / chunkresolution, (float)1 / chunkresolution);
    }

    private void animate(Vector3 cubeposition)
    {
        int x = (int)cubeposition.x;
        int y = (int)cubeposition.y;
        int z = (int)cubeposition.z;


        string binary = "";
        binary += setsphere[x + 1, y, z + 1].ToString();
        binary += setsphere[x, y, z + 1].ToString(); ;
        binary += setsphere[x, y, z].ToString();
        binary += setsphere[x + 1, y, z].ToString();
        binary += setsphere[x + 1, y + 1, z + 1].ToString();
        binary += setsphere[x, y + 1, z + 1].ToString();
        binary += setsphere[x, y + 1, z].ToString();
        binary += setsphere[x + 1, y + 1, z].ToString();

        Debug.Log(System.Convert.ToInt32(binary, 2));


    }

    private void Update()
    {
        time += Time.deltaTime;
        Debug.Log(time);

        if (time > cubespeed)
        {
            
            if (i > chunkresolution - 1) { j++; i = 0; };
            if (j > chunkresolution - 1) { j = 0; k++; }
            if (k > chunkresolution - 1) {i = 0; k = 0; j = 0 ; Debug.Break(); }

            cube.transform.position = new Vector3((float)i / chunkresolution, (float)j / chunkresolution, (float)k / chunkresolution) + gameObject.transform.position;
            cube.transform.position += new Vector3(0.5f / chunkresolution, 0.5f / chunkresolution, 0.5f / chunkresolution);

            animate(cube.transform.position - new Vector3(0.5f,0.5f,0.5f));

            i++;

            
            time = 0;
        }
    }

}
