using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string actionMapName = "Player";

    private InputActionMap map;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction grappleAction;
    private InputAction telekinesisAction;
    private InputAction throwAction;

    public Vector2 Move => moveAction.ReadValue<Vector2>();
    public Vector2 Look => lookAction.ReadValue<Vector2>();
    public bool JumpPressed => jumpAction.WasPressedThisFrame();
    public bool DashPressed => dashAction.WasPressedThisFrame();
    public bool GrapplePressed => grappleAction.WasPressedThisFrame();
    public bool TelekinesisPressed => telekinesisAction.WasPressedThisFrame();
    public bool ThrowPressed => throwAction.WasPressedThisFrame();

    private void Awake()
    {
        map = controls.FindActionMap(actionMapName, true);
        moveAction = map.FindAction("Move", true);
        lookAction = map.FindAction("Look", true);
        jumpAction = map.FindAction("Jump", true);
        dashAction = map.FindAction("Dash", true);
        grappleAction = map.FindAction("Grapple", true);
        telekinesisAction = map.FindAction("Telekinesis", true);
        throwAction = map.FindAction("Throw", true);
    }

    private void OnEnable() => map.Enable();
    private void OnDisable() => map.Disable();
}