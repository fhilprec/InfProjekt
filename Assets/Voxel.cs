using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    const int chunkresolution = 40;
    public int chunksize = 10;
    Noise noise = new Noise();
    [Range(0,1)]
    public float threshhold = 0.7f;
    public Vector3 offset = new Vector3(0,0,5);
    TriAngulationTable tri = new TriAngulationTable();
    [Range(0.1f,1)]
    const float frequenzy = 0.08f;
    int[,,] noisevalues = new int[chunkresolution, chunkresolution, chunkresolution];
    public Material mat;

    List<Vector3> verticeslist = new List<Vector3>();               //only for debugging make class list after finising
    List<int> triangleslist = new List<int>();              //same here

    int x = 0;
    int y = 0;
    int z = 0;

    
    private float density(Vector3 point)
    {
        float v = 0;

        v = (noise.Evaluate(point * frequenzy + offset + gameObject.transform.position) + 1) / 2;

        return v;
    }

    private void triangulate()
    {
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
        
        for (int i = 0; i < 16; i++)            //Should be working
        {
            int index = tri.triTable[System.Convert.ToInt32(binary, 2), i];
            if (index != -1){
                Vector3 point1 = tri.edgetable[index, 0];
                Vector3 point2 = tri.edgetable[index, 1];

                float v1 = density(point1 + new Vector3(x,y,z) + gameObject.transform.position);
                float v2 = density(point2 + new Vector3(x,y,z) + gameObject.transform.position);
                Vector3 newp = (point1 + point2) / 2 + (point2 - point1) * v1 + (point1 - point2) * v2; 



                newp += new Vector3(x, y, z) + gameObject.transform.position;
                //newp /= chunkresolution;
               // newp *= chunksize;

                verticeslist.Add(newp);
                verticescount++;
                
                
            }
            
        }
        for (int i = 0; i < verticescount; i++)
        {
            triangleslist.Add(verticeslist.Count - verticescount + i);
        }




        



    }

    private void Start()
    {

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;


        //filling the 3d array with the noisevalues
        for (int x = 1; x < chunkresolution-1; x++)          
        {
            for (int y = 1; y < chunkresolution-1; y++)
            {
                for (int z = 1; z < chunkresolution-1; z++)
                {
                    float noisevalue = (density(new Vector3(x,y,z)));             //use worldpspace


                    if (noisevalue > threshhold)
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


        

        for (z = 0; z < chunkresolution-1; z++)
        {
            for (y = 0; y < chunkresolution-1; y++)
            {
                for (x = 0; x < chunkresolution-1; x++)
                {
                    
                    triangulate();
                }
            }
        }
        Mesh mesh = new Mesh();
        
        Vector3[] verticesarray = new Vector3[verticeslist.Count];
        int[] trianglesarray = new int[triangleslist.Count];

        verticesarray = verticeslist.ToArray();
        trianglesarray = triangleslist.ToArray();


        mesh.vertices = verticesarray;
        mesh.triangles = trianglesarray;
        mesh.RecalculateNormals();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        
    }

    

    


}


