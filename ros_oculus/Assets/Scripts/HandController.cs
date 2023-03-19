
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference primaryButtonAction;
    private int pressedCount = 0;
    private void OnEnable()
    {
        primaryButtonAction.action.performed += PrimaryPressed;
    }

    private void OnDisable()
    {
        primaryButtonAction.action.performed -= PrimaryPressed;
    }

    private void PrimaryPressed(InputAction.CallbackContext context)
    {
        pressedCount++;
    }

    public int GetPressedCount()
    {
        return pressedCount;
    }
}
