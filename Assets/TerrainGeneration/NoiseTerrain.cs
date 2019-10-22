using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTerrain : MonoBehaviour
{
    [Range(5, 100)]
    public int chunkresolution = 30;
    private bool useParent = true;

    public int chunksize = 1;
    public bool haschanged;
    Noise noise = new Noise();
    [Range(0, 1)]
    public float threshhold = 0.6f;
    public Vector3 offset = new Vector3(0, 0, 5);
    TriAngulationTable tri = new TriAngulationTable();
    [Range(0.1f, 10)]
    public float frequenzy = 2f;
    public float terrainfrequenzy = 1f;
    public float amplitude = 1f;
    public int maxheight = 10;

    int[,,] noisevalues;
    public Material mat;
    public int octaves = 8;

    List<Vector3> verticeslist = new List<Vector3>();               //only for debugging make class list after finising
    List<int> triangleslist = new List<int>();              //same here

    int x = 0;
    int y = 0;
    int z = 0;


    private float density(Vector3 point)
    {
        float v = 0;

        v = (noise.Evaluate(point * frequenzy + offset) ) / 2;

        return v;
    }

    private float terrain(Vector3 point)
    {

        float v = 0;
        float tempfrequenzy = terrainfrequenzy;
        float tempamplitude = amplitude;


        for (int i = 0; i < octaves; i++)
        {

            v += (noise.Evaluate(point * tempfrequenzy) + 1) / 2 * tempamplitude;
            tempfrequenzy *= 2.05f;
            tempamplitude *= 0.5f;
        }
       
        return v;


    }




    private void triangulate(int[,,] noisevalues)
    {
        string binary = "";
        binary += noisevalues[x + 1, y, z + 1].ToString();
        binary += noisevalues[x, y, z + 1].ToString(); ;
        binary += noisevalues[x, y, z].ToString();
        binary += noisevalues[x + 1, y, z].ToString();
        binary += noisevalues[x + 1, y + 1, z + 1].ToString();
        binary += noisevalues[x, y + 1, z + 1].ToString();
        binary += noisevalues[x, y + 1, z].ToString();
        binary += noisevalues[x + 1, y + 1, z].ToString();

        // Debug.Log("Binary is equal to " + binary);

        int verticescount = 0;

        for (int i = 0; i < 16; i++)            //Should be working
        {
            int index = tri.triTable[System.Convert.ToInt32(binary, 2), i];
            if (index != -1)
            {
                Vector3 point1 = tri.edgetable[index, 0];
                Vector3 point2 = tri.edgetable[index, 1];
                //creates smooth terrain
                // magic number at the end creates best results -> higher number smoother terrain

                float v1 = density((point1 + new Vector3(x, y, z))/(chunkresolution-1) + gameObject.transform.position) *50  ;
                float v2 = density((point2 + new Vector3(x, y, z))/(chunkresolution-1) + gameObject.transform.position)  *50;
                Vector3 newp = ((point1 + point2) / 2) + (point2 - point1) * v1 + (point1 - point2) * v2;

                //creates blocky terrain
                //Vector3 newp = ((point1 + point2) / 2);



                newp += new Vector3(x, y, z);
                newp /= chunkresolution - 1;      //-3 instead of -2 to avoid most cracks should be fixed later
                 

                verticeslist.Add(newp);
                verticescount++;


            }

        }
        for (int i = 0; i < verticescount; i++)
        {
            triangleslist.Add(verticeslist.Count - verticescount + i);
        }








    }

    private void generate()
    {
        if (useParent)
        {
            terrainfrequenzy = gameObject.transform.parent.GetComponent<ChunkManagement>().frequenzy;
            chunkresolution = gameObject.transform.parent.GetComponent<ChunkManagement>().resolution;
            amplitude = gameObject.transform.parent.GetComponent<ChunkManagement>().amplitude;
            octaves = gameObject.transform.parent.GetComponent<ChunkManagement>().octaves;

        }
        int[,,] noisevalues = new int[chunkresolution, chunkresolution, chunkresolution];



        triangleslist.Clear();
        verticeslist.Clear();

        if (gameObject.GetComponent<MeshRenderer>() == null) { gameObject.AddComponent<MeshRenderer>(); }
        if (gameObject.GetComponent<MeshFilter>() == null) { gameObject.AddComponent<MeshFilter>(); }





        //filling the 3d array with the noisevalues
        for (int x = 0; x < chunkresolution ; x++)             //by adding the minus the borders are rendered
        {
            for (int y = 0; y < chunkresolution-1 * maxheight; y++)
            {


                for (int z = 0; z < chunkresolution ; z++)
                {
                    
                    float height = 0;
                    height += terrain(new Vector3(x,0,z)/(chunkresolution-1) + gameObject.transform.position);
                    //Debug.Log(height);
                    

                    if (y <= (int)height)
                    {
                        noisevalues[x, y, z] = 1;
                    }


                }
            }

            
        }




        for (z = 0; z < chunkresolution - 1; z++)
        {
            for (y = 0; y < chunkresolution - 1 * maxheight; y++)
            {
                for (x = 0; x < chunkresolution - 1; x++)
                {

                    triangulate(noisevalues);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Vector3[] verticesarray = new Vector3[verticeslist.Count];
        int[] trianglesarray = new int[triangleslist.Count];

        verticesarray = verticeslist.ToArray();
        trianglesarray = triangleslist.ToArray();


        mesh.vertices = verticesarray;
        mesh.triangles = trianglesarray;
        mesh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.transform.localScale = new Vector3(chunksize, chunksize, chunksize);




    }


    private void Update()
    {
        if (haschanged)
        {
            generate();
            haschanged = false;
        }
    }



}


