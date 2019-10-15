using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    const int chuncksize = 10;
    Noise noise = new Noise();
    [Range(0,1)]
    public float threshhold = 0.808f;
    public Vector3 offset = new Vector3(0,0,5);
    TriAngulationTable tri = new TriAngulationTable();
    [Range(0.1f,1)]
    public float frequenzy = 0.118f;
    int[,,] noisevalues = new int[chuncksize, chuncksize, chuncksize];
    Mesh mesh;
    GameObject cube;
    float time = 1000;
    public Material mat;
    private Vector3[] vertices;
    private int[] triangles;
    private bool done;

    List<Vector3> verticeslist = new List<Vector3>();               //only for debugging make class list after finising
    List<int> triangleslist = new List<int>();              //same here



    int x = 0;
    int y = 0;
    int z = 0;

    float test = 0;

    public float cubespeed = 0;

    
    private float density(Vector3 point)
    {
        float v = 0;

        v = (noise.Evaluate(point * frequenzy + offset + gameObject.transform.position) + 1) / 2;

        return v;
    }
    private void triangulate()
    {
        

        Mesh mesh = new Mesh();

        

        //check all the points within the cube
        string binary = "";
        binary += noisevalues[x+1, y, z+1].ToString();
        binary += noisevalues[x, y, z+1].ToString();;
        binary += noisevalues[x, y, z].ToString();
        binary += noisevalues[x + 1, y, z].ToString();     
        binary += noisevalues[x + 1, y+1, z + 1].ToString();
        binary += noisevalues[x, y+1, z + 1].ToString();
        binary += noisevalues[x, y + 1, z].ToString();
        binary += noisevalues[x + 1, y+1, z].ToString();

       // Debug.Log("Binary is equal to " + binary);

        int verticescount = 0;
        Debug.ClearDeveloperConsole();
        for (int i = 0; i < 16; i++)            //Should be working
        {
            int index = tri.triTable[System.Convert.ToInt32(binary, 2), i];
            if (index != -1){
                Vector3 point1 = tri.edgetable[index, 0];
                Vector3 point2 = tri.edgetable[index, 1];
                //verticeslist.Add((tri.edgetable[index, 0] + new Vector3(x, y, z) + tri.edgetable[index, 1] + new Vector3(x, y, z)) / 2  +gameObject.transform.position); //adding gameobjects position for worldspace





                /*float v1 = density(point1);
                float v2 = density(point2);
                Vector3 newp;
                if (v1 < v2)
                {
                    newp = (point1 + point2) / 2 * v2 / v1;
                }
                else
                {
                    newp = (point1 + point2) / 2 * v1 / v2;
                }
                if(v1 == v2)
                {*/
                    Vector3 newp = (point1 + point2) / 2;
                //}



                newp += new Vector3(x, y, z) + gameObject.transform.position;
                verticeslist.Add(newp);

                Debug.Log(density(point1));


                Debug.Log(density(point2));

                verticescount++;
                //Debug.Log(index);
                
            }

            
        }
        for (int i = 0; i < verticeslist.Count; i++)                //spawn spheres for debugging
        {
            GameObject debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debug.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            debug.transform.position = verticeslist[i];
            debug.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0);
            debug.transform.SetParent(gameObject.transform);

        }





        for (int i = 0; i < verticescount; i++)
        {
            //Debug.Log(verticeslist.Count - verticescount + i);
            triangleslist.Add(verticeslist.Count - verticescount + i);
        }




        Vector3[] verticesarray = new Vector3[verticeslist.Count];
        int[] trianglesarray = new int[triangleslist.Count];

        verticesarray = verticeslist.ToArray();
        trianglesarray = triangleslist.ToArray();


        mesh.vertices = verticesarray;
        mesh.triangles = trianglesarray;
        mesh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;



    }

    private void Start()
    {

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
        //gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 0, 0);


        //filling the 3d array with the noisevalues
        for (int x = 1; x < chuncksize-1; x++)          //trying to fill the gaps thats why the magic numbers
        {
            for (int y = 1; y < chuncksize-1; y++)
            {
                for (int z = 1; z < chuncksize-1; z++)
                {
                    float noisevalue = (density(new Vector3(x,y,z)));             //use worldpspace


                    if (noisevalue > threshhold)
                    {
                        noisevalues[x, y, z] = 1;
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        sphere.transform.position = new Vector3(x, y, z) + gameObject.transform.position;
                        sphere.transform.SetParent(gameObject.transform);
                    }
                    else
                    {
                        noisevalues[x, y, z] = 0;
                    }

                }

            }
        }
        /*int temporary = 0;
        noisevalues[1 + temporary, 1 + temporary, 1 + temporary] = 1;
        noisevalues[2 + temporary, 1 + temporary, 1 + temporary] = 1;
        noisevalues[1 + temporary, 1 + temporary, 2 + temporary] = 1;
        noisevalues[2 + temporary, 1 + temporary, 2 + temporary] = 1;
        noisevalues[1 + temporary, 2 + temporary, 1 + temporary] = 1;
        noisevalues[1 + temporary, 2 + temporary, 2 + temporary] = 1;
        noisevalues[2 + temporary, 2 + temporary, 1 + temporary] = 1;
        noisevalues[2 + temporary, 2 + temporary, 2 + temporary] = 1;       


        //testloop
        for (int i = 0; i < chuncksize; i++)
        {
            for (int j = 0; j < chuncksize; j++)
            {
                for (int k = 0; k < chuncksize; k++)
                {
                    if (noisevalues[i,j,k] == 1)
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        sphere.transform.position = new Vector3(i,j,k);
                    }
                }
            }
        }*/
        

        //move the cube

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);





        //check whether  all the points of the cube are in the according surface
        cube.transform.position = new Vector3(0.5f, 0.5f, 0.5f);


    }

    private void march()
    {
        for (int x = 0; x < chuncksize-1; x++)
        {
            for (int y = 0; y < chuncksize-1; y++)
            {
                for (int z = 0; z < chuncksize-1; z++)
                {

                }
            }
        }
    }

    private void animate()
    {
        if (done == false)
        {
            test += 0.01f;
            Mesh mesh = new Mesh();
            time += Time.deltaTime;
            while (time > cubespeed)
            {
                if (x > chuncksize - 2) { x = 0; y++; }
                if (y > chuncksize - 2) { x = 0; y = 0; z++; }
                if (z > chuncksize - 2) { x = 0; y = 0; done = true; }

                cube.transform.position = new Vector3(x, y, z) + new Vector3(0.5f, 0.5f, 0.5f) + gameObject.transform.position;
                triangulate();



                x++;
                time = 0;
            }
        }
    }

    private void Update()
    {
        animate();
    }

}


