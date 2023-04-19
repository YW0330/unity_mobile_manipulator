using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ChangeVision : MonoBehaviour
{
    [SerializeField] HandController handController;
    [SerializeField] GameObject LocomotionSystem;
    [SerializeField] float step = 0.05f;
    void Start()
    {

    }

    void FixedUpdate()
    {
        Vector2 tmp = handController.GetPrimary2DAxisValue();
        if (handController.GetGripBtnPressed())
        {
            LocomotionSystem.SetActive(false);
        }
        else
        {
            if ((handController.GetThumbBtnCount() % 2) == 0)
                LocomotionSystem.SetActive(true);
            else
            {
                LocomotionSystem.SetActive(false);
                transform.Translate(tmp[1] * Vector3.up * step);
            }
        }

    }
}
