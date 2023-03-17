
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] InputActionReference primaryButtonAction;
    private bool pressedCount = false;
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
        if (!pressedCount)
            Debug.Log("Platform");
        else
            Debug.Log("Manipulator");

        pressedCount = !pressedCount;
    }
}
