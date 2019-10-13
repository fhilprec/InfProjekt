using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scratch : MonoBehaviour
{
    public Material test;
    private void Start()
    {
        int[] triangles = new int[3];
        Vector3[] vertices = new Vector3[3];

        vertices[0] = new Vector3(1, 0, 0);
        vertices[1] = new Vector3(1, 1, 0);
        vertices[2] = new Vector3(0, 1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        Mesh mesh = new Mesh();

        gameObject.AddComponent<MeshFilter>();


        mesh.vertices = vertices;
        mesh.triangles = triangles;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = test;

    }
}
