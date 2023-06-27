
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference primaryBtnPressedAction;
    [SerializeField] InputActionReference primary2DAxisMovedAction;
    [SerializeField] InputActionReference primary2DAxisPressedAction;
    [SerializeField] InputActionReference triggerPressedValueAction;
    [SerializeField] InputActionReference leftHandSecondaryBtnPressedAction;
    [SerializeField] InputActionReference rightHandSecondaryBtnPressedAction;
    [SerializeField] InputActionReference gripPressedAction;
    private int primaryBtnCount = 0;
    private int thumbBtnCount = 0;
    private bool isSecondaryBtnPressed = false;
    private bool isGripBtnPressed = false;

    private void OnEnable()
    {
        primaryBtnPressedAction.action.performed += PrimaryBtnPressed;
        primary2DAxisPressedAction.action.performed += ThumbBtnCount;
        primary2DAxisMovedAction.action.Enable();
        triggerPressedValueAction.action.Enable();
        leftHandSecondaryBtnPressedAction.action.performed += SecondaryBtnPressed;
        rightHandSecondaryBtnPressedAction.action.performed += SecondaryBtnPressed;
        gripPressedAction.action.started += GripBtnPressed;
        gripPressedAction.action.canceled += GripBtnRealeased;
    }

    private void OnDisable()
    {
        primaryBtnPressedAction.action.performed -= PrimaryBtnPressed;
        primary2DAxisPressedAction.action.performed -= ThumbBtnCount;
        primary2DAxisMovedAction.action.Disable();
        triggerPressedValueAction.action.Disable();
        leftHandSecondaryBtnPressedAction.action.performed -= SecondaryBtnPressed;
        rightHandSecondaryBtnPressedAction.action.performed -= SecondaryBtnPressed;
        gripPressedAction.action.started -= GripBtnPressed;
        gripPressedAction.action.canceled -= GripBtnRealeased;
    }

    private void GripBtnPressed(InputAction.CallbackContext context)
    {
        isGripBtnPressed = true;
    }
    private void GripBtnRealeased(InputAction.CallbackContext context)
    {
        isGripBtnPressed = false;
    }
    private void PrimaryBtnPressed(InputAction.CallbackContext context)
    {
        primaryBtnCount++;
    }
    private void SecondaryBtnPressed(InputAction.CallbackContext context)
    {
        isSecondaryBtnPressed = true;
    }
    private void ThumbBtnCount(InputAction.CallbackContext context)
    {
        thumbBtnCount++;
    }

    public int GetPrimaryBtnCount()
    {
        return primaryBtnCount;
    }
    public int GetThumbBtnCount()
    {
        return thumbBtnCount;
    }
    public Vector2 GetPrimary2DAxisValue()
    {
        return primary2DAxisMovedAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public float GetTriggerValue()
    {
        return triggerPressedValueAction.action?.ReadValue<float>() ?? 0;
    }
    public bool GetSecondaryBtnIsPressed()
    {
        return isSecondaryBtnPressed;
    }

    public bool GetGripBtnPressed()
    {
        return isGripBtnPressed;
    }
}
