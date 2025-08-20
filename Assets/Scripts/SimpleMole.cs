// 2025-08-20 AI-Tag
// Refactored to use PlayerInputActions and events, with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMole : MonoBehaviour
{
    public float popUpHeight = 1f;
    public float animationSpeed = 2f;

    private Testinputactions playerInputActions;
    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;
    private bool isVisible = false;

    public bool IsVisible => isVisible;

    private void Awake()
    {
        playerInputActions = new Testinputactions();
        playerInputActions.Enable();
        playerInputActions.Mole.togglemole.performed += ToggleMole_Performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Mole.togglemole.performed -= ToggleMole_Performed;
        playerInputActions.Dispose();
    }

    private void ToggleMole_Performed(InputAction.CallbackContext context)
    {
        isVisible = !isVisible;
    }

    private void Start()
    {
        hiddenPosition = transform.position;
        visiblePosition = hiddenPosition + Vector3.up * popUpHeight;
    }

    private void Update()
    {
        Vector3 targetPos = isVisible ? visiblePosition : hiddenPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, animationSpeed * Time.deltaTime);
    }

    public void ShowMole() { isVisible = true; }
    public void HideMole() { isVisible = false; }

}
