using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // Events
    public event Action OnMouseClick;
    public event Action OnFly;

    // Read-only properties for gameplay
    public bool IsRunning { get; private set; }
    public Vector3 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; } // mouse delta / look
    public bool IsJumping { get; private set; }

    [Header("Assign Input Actions (from your Input Actions asset)")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference runAction;
    public InputActionReference flyAction;
    public InputActionReference clickAction;

    private void OnEnable()
    {
        // Enable actions if assigned
        moveAction?.action?.Enable();
        lookAction?.action?.Enable();
        jumpAction?.action?.Enable();
        runAction?.action?.Enable();
        flyAction?.action?.Enable();
        clickAction?.action?.Enable();

        // Subscribe to performed events for one-shot actions
        if (clickAction?.action != null)
            clickAction.action.performed += ClickPerformed;
        if (flyAction?.action != null)
            flyAction.action.performed += FlyPerformed;
    }

    private void OnDisable()
    {
        if (clickAction?.action != null)
            clickAction.action.performed -= ClickPerformed;
        if (flyAction?.action != null)
            flyAction.action.performed -= FlyPerformed;

        // Disable actions (optional but recommended)
        moveAction?.action?.Disable();
        lookAction?.action?.Disable();
        jumpAction?.action?.Disable();
        runAction?.action?.Disable();
        flyAction?.action?.Disable();
        clickAction?.action?.Disable();
    }

    private void Update()
    {
        ReadMovement();
        ReadLook();
        ReadJump();
        ReadRun();
        // Fire and Fly are handled via performed callbacks (no need to poll each frame)
    }

    private void ReadMovement()
    {
        if (moveAction?.action != null)
        {
            Vector2 v = moveAction.action.ReadValue<Vector2>();
            // Map (x, y) -> (x, 0, y) to keep your previous MovementInput shape
            MovementInput = new Vector3(v.x, 0f, v.y);
        }
        else MovementInput = Vector3.zero;
    }

    private void ReadLook()
    {
        if (lookAction?.action != null)
        {
            // The usual binding is <Mouse>/delta or gamepad/rightStick
            MousePosition = lookAction.action.ReadValue<Vector2>();
        }
        else MousePosition = Vector2.zero;
    }

    private void ReadJump()
    {
        if (jumpAction?.action != null)
        {
            // Button that can be held — check if pressed (1) or not (0)
            // Using ReadValue<float>() because Button actions return 0/1
            IsJumping = jumpAction.action.ReadValue<float>() > 0.5f;
        }
        else IsJumping = false;
    }

    private void ReadRun()
    {
        if (runAction?.action != null)
        {
            // Run is a hold button in your original; same here
            IsRunning = runAction.action.ReadValue<float>() > 0.5f;
        }
        else IsRunning = false;
    }

    // Called when fire action is performed (button press)
    private void ClickPerformed(InputAction.CallbackContext ctx)
    {
        OnMouseClick?.Invoke();
    }

    // Called when fly action is performed (button press)
    private void FlyPerformed(InputAction.CallbackContext ctx)
    {
        OnFly?.Invoke();
    }
}
