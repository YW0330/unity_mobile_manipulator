using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWall : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            // Destroy(gameObject);
            var point1 = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
            Debug.Log(point1);
            GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere1.transform.position = point1;
            sphere1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


            var point2 = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
            Debug.Log(point2);
            GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere2.transform.position = point2;
            sphere2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.AddComponent<DestroyWall>();
            cube1.transform.position = new Vector3((point1.x + point2.x) / 2, 1f, (point1.z + point2.z) / 2);
            cube1.transform.rotation = Quaternion.Euler(0, -Mathf.Atan2((point2.z - point1.z), (point2.x - point1.x)) * 57.3f, 0);
            cube1.transform.localScale = new Vector3(Mathf.Sqrt((point2.x - point1.x) * (point2.x - point1.x) + (point2.z - point1.z) * (point2.z - point1.z)), 0.5f, 0.05f);
        }
    }
    void Update()
    {


    }
}
