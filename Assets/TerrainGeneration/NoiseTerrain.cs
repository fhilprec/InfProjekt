using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTerrain : MonoBehaviour
{
    [Range(1, 100)]
    public int chunkresolution = 6;
    public int chunksize = 1;
    Noise noise = new Noise();
    [Range(0, 1)]
    public float threshhold = 0.6f;
    public Vector3 offset = new Vector3(0, 0, 5);
    TriAngulationTable tri = new TriAngulationTable();
    [Range(0.01f, 10f)]
    public float frequenzy = 2f;
    public float amplitude = 1f;
    int[,,] noisevalues;
    public Material mat;
    List<Vector3> verticeslist = new List<Vector3>();               //only for debugging make class list after finising
    List<int> triangleslist = new List<int>();              //same here
    public float accuracy;

    int x = 0;
    int y = 0;
    int z = 0;



    private float density(Vector3 point1)
    {
        point1 /= (chunkresolution - 1);
        return (noise.Evaluate(point1 * frequenzy + gameObject.transform.position/(chunksize)) + 1) / 2;
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


        int verticescount = 0;

        for (int i = 0; i < 16; i++)           
        {

            int index = tri.triTable[System.Convert.ToInt32(binary, 2), i];
            if (index != -1)
            {
                Vector3 point1 = (tri.edgetable[index, 0] + new Vector3(x, y, z)); 
                Vector3 point2 = (tri.edgetable[index, 1] + new Vector3(x,y,z));

                float v1 = density(point1);
                float v2 = density(point2);
                float t = (threshhold-v1) / (v2-v1);
                Vector3 newp;         
                newp = point1 + t * (point2-point1);
                //newp = (point1 + point2) / 2;

                verticeslist.Add(newp / (chunkresolution-1));
                verticescount++;


            }

        }
        for (int i = 0; i < verticescount; i++)
        {
            triangleslist.Add(verticeslist.Count - verticescount + i);
        }

    }

    private void Update()
    {
        int[,,] noisevalues = new int[chunkresolution, chunkresolution, chunkresolution];
        triangleslist.Clear();
        verticeslist.Clear();
        if (gameObject.GetComponent<MeshRenderer>() == null) { gameObject.AddComponent<MeshRenderer>(); }
        if (gameObject.GetComponent<MeshFilter>() == null) { gameObject.AddComponent<MeshFilter>(); }
     
        //filling the 3d array with the noisevalues
        for (int x = 0; x < chunkresolution ; x++)             //by adding the minus the borders are rendered
        {
            for (int y = 0; y < chunkresolution; y++)
            {

                for (int z = 0; z < chunkresolution; z++)
                {
                    float noisevalue = density(new Vector3(x, y, z));
                    if ( noisevalue > threshhold)
                    {
                        noisevalues[x, y, z] = 1;

                        
                    }
                    else
                    {
                        noisevalues[x, y, z] = 0;
                    }


                }
            }


        }




        for (z = 0; z < chunkresolution - 1; z++)
        {
            for (y = 0; y < chunkresolution - 1; y++)
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
        gameObject.GetComponent<MeshRenderer>().material = mat;




    }



}


