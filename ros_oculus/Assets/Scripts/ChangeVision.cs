using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ChangeVision : MonoBehaviour
{
    [SerializeField] HandController handController;
    [SerializeField] GameObject plane;
    [SerializeField] float step = 0.05f;
    void Start()
    {

    }

    void FixedUpdate()
    {
        Vector2 tmp = handController.GetPrimary2DAxisValue();
        if ((handController.GetThumbBtnCount() % 2) == 0)
            // transform.Translate((tmp[0] * Vector3.right + tmp[1] * Vector3.forward) * step);
            plane.GetComponent<TeleportationArea>().enabled = true;
        else
        {
            plane.GetComponent<TeleportationArea>().enabled = false;
            transform.Translate(tmp[1] * Vector3.up * step);
        }
    }
}
