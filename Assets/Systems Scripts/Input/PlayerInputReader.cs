using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads player input from an Input Actions asset and exposes it as simple values.
/// Other scripts read from here and never touch the Input System directly.
/// </summary>
public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string actionMapName = "Player";

    private InputActionMap map;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    public Vector2 Move => moveAction.ReadValue<Vector2>();
    public Vector2 Look => lookAction.ReadValue<Vector2>();
    public bool JumpPressed => jumpAction.WasPressedThisFrame();
    public bool DashPressed => dashAction.WasPressedThisFrame();

    private void Awake()
    {
        map = controls.FindActionMap(actionMapName, true);
        moveAction = map.FindAction("Move", true);
        lookAction = map.FindAction("Look", true);
        jumpAction = map.FindAction("Jump", true);
        dashAction = map.FindAction("Dash", true);
    }

    private void OnEnable() => map.Enable();
    private void OnDisable() => map.Disable();
}