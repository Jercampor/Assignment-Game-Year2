using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float groundAcceleration = 60f;
    [SerializeField] private float airAcceleration = 20f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 28f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField, Range(0f, 1f)] private float dashExitMomentum = 0.6f;

    // Read-only access to settings for the states
    public float GroundAcceleration => groundAcceleration;
    public float AirAcceleration => airAcceleration;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public float DashExitMomentum => dashExitMomentum;

    // Shared data the states read and write
    public CharacterController Controller { get; private set; }
    public PlayerInputReader InputReader { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set; }
    public float NextDashTime { get; set; }

    public bool CanDash => Time.time >= NextDashTime;

    // The state machine and the states it can switch between
    public StateMachine<PlayerController> Machine { get; private set; }
    public PlayerGroundedState Grounded { get; private set; }
    public PlayerAirborneState Airborne { get; private set; }
    public PlayerDashingState Dashing { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputReader = GetComponent<PlayerInputReader>();

        Machine = new StateMachine<PlayerController>(this);
        Grounded = new PlayerGroundedState();
        Airborne = new PlayerAirborneState();
        Dashing = new PlayerDashingState();
    }

    private void Start()
    {
        Machine.ChangeState(Grounded);
    }

    private void Update()
    {
        Machine.Tick();
    }

    // ---- Helpers the states share ----

    public Vector3 GetWishDirection()
    {
        Vector2 move = InputReader.Move;
        return transform.right * move.x + transform.forward * move.y;
    }

    public void ApplyHorizontalAcceleration(float acceleration)
    {
        Vector3 target = Vector3.ClampMagnitude(GetWishDirection(), 1f) * moveSpeed;
        HorizontalVelocity = Vector3.MoveTowards(HorizontalVelocity, target, acceleration * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        VerticalVelocity += gravity * Time.deltaTime;
    }

    public void Jump()
    {
        VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void MoveCharacter()
    {
        Controller.Move((HorizontalVelocity + Vector3.up * VerticalVelocity) * Time.deltaTime);
    }

    // Temporary on-screen label so you can watch the states change
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "State: " + Machine?.Current?.GetType().Name);
    }
}