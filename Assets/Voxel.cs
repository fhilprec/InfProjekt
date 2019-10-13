using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    const int chuncksize = 10;
    Noise noise = new Noise();
    [Range(0,1)]
    public float threshhold;
    TriAngulationTable tri = new TriAngulationTable();
    [Range(0.1f,1)]
    public float frequenzy;
    float[,,] noisevalues = new float[chuncksize, chuncksize, chuncksize];
    Mesh mesh;
    GameObject cube;
    float time = 1;
    public Material mat;
    private Vector3[] vertices;
    private int[] triangles;

    int x = 0;
    int y = 0;
    int z = 0;

    float test = 0;

    public float cubespeed = 1;


    private void triangulate()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        Vector3[] verticesarray = new Vector3[3];
        int[] trianglesarray = new int[3];


        mesh.vertices = verticesarray;
        mesh.triangles = trianglesarray;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;



    }

    private void Start()
    {
        
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
        //gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(1, 0, 0);


        //filling the 3d array with the noisevalues
        for (int x = 0; x  < chuncksize; x++)
        {
            for (int y = 0; y < chuncksize; y++)
            {
                for (int z = 0; z < chuncksize; z++)
                {
                    float noisevalue = (noise.Evaluate(new Vector3(x, y, z) * frequenzy)+1)/2;
                    noisevalues[x, y, z] = noisevalue;
                    Debug.Log(noisevalue);
                    if (noisevalue > threshhold)
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        sphere.transform.position = new Vector3(x, y, z);
                    }

                }

            }
        }


        //move the cube

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);





        //check whether  all the points of the cube are in the according surface
        cube.transform.position = new Vector3(0.5f,0.5f, 0.5f);
        

    }

    private void Update()
    {

        test += 0.01f;
        Mesh mesh = new Mesh();
        time += Time.deltaTime;
        while (time > cubespeed)
        {
                if (x > chuncksize - 2) { x = 0; y++; }
                if (y > chuncksize - 2) { x = 0; y = 0; z++; }
                if (z > chuncksize - 2) { x = 0; y = 0; z = 0; }

            cube.transform.position = new Vector3(x, y, z) + new Vector3(0.5f, 0.5f, 0.5f);
            triangulate();



            x++;
                time = 0;
            }
        }
    }


