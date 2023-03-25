using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVision : MonoBehaviour
{
    [SerializeField] HandController handController;
    [SerializeField] float step = 0.05f;
    void Start()
    {

    }


    void FixedUpdate()
    {
        Vector2 tmp = handController.GetPrimary2DAxis();
        if (handController.GetThumbBtnCount() % 2 == 0)
            transform.Translate((tmp[0] * Vector3.right + tmp[1] * Vector3.forward) * step);
        else
            transform.Translate(tmp[1] * Vector3.up * step);
    }
}
