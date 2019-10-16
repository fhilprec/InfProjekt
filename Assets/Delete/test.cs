using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Material mat;
    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject resultsphere;
    public float v1 = 0.5f;
    public float v2 = 0.7f;



    // Start is called before the first frame update
    void OnValidate()
    {
        Vector3 point1 = new Vector3(0, 0, 0);
        Vector3 point2 = new Vector3(1, 0, 0);

        float fak = (0 - v1) / (v2 - v1); ;
        Vector3 result = (point1+point2)/2 + (point2-point1) * (0.5f-v1) + (point2 - point1) * (0.5f - v2);

        sphere1.transform.position = point1;
        sphere2.transform.position = point2;
        resultsphere.transform.position = result;

        

    
    }

   
}
