using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("cube" + gameObject.transform.position);
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
