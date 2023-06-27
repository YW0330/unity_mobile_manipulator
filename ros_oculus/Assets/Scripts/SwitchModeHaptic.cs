using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[System.Serializable]
class HapticParams
{
    [Range(0, 1)]
    public float intesity;
    public float duration;
}
public class SwitchModeHaptic : MonoBehaviour
{
    HandController handController;
    [SerializeField] XRBaseController leftHandController;

    [SerializeField] HapticParams switchMode;
    [SerializeField] HapticParams awake;

    private int curr = 0;
    private int prev = 0;
    private float waitTime = 0.0f;

    private bool flag = false;

    void Start()
    {
        handController = GetComponent<HandController>();
    }

    void Update()
    {
        curr = handController.GetPrimaryBtnCount();
        if (!flag && curr != prev)
        {
            leftHandController.SendHapticImpulse(switchMode.intesity, switchMode.duration);
            flag = true;
            waitTime = Time.time + 4;
        }
        else if (flag && Time.time > waitTime)
        {
            leftHandController.SendHapticImpulse(awake.intesity, awake.duration);
            flag = false;
        }
        prev = curr;
    }
}
