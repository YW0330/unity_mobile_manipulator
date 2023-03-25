
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference primaryButtonAction;
    [SerializeField] InputActionReference primary2DAxisMovedAction;
    [SerializeField] InputActionReference primary2DAxisPressedAction;

    private int primaryBtnCount = 0;
    private int thumbBtnCount = 0;

    private void OnEnable()
    {
        primaryButtonAction.action.performed += PrimaryBtnPressed;
        primary2DAxisPressedAction.action.performed += ThumbBtnPressed;
        primary2DAxisMovedAction.action.Enable();
    }

    private void OnDisable()
    {
        primaryButtonAction.action.performed -= PrimaryBtnPressed;
        primary2DAxisPressedAction.action.performed -= ThumbBtnPressed;
        primary2DAxisMovedAction.action.Disable();
    }

    private void PrimaryBtnPressed(InputAction.CallbackContext context)
    {
        primaryBtnCount++;
    }
    private void ThumbBtnPressed(InputAction.CallbackContext context)
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
    public Vector2 GetPrimary2DAxis()
    {
        return primary2DAxisMovedAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
    }
}
