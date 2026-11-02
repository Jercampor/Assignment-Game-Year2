/* using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(GrappleHook))]
[RequireComponent(typeof(TelekinesisPower))]
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

    [Header("Grapple")]
    [SerializeField] private float grapplePullSpeed = 35f;          // top speed on the rope
    [SerializeField] private float grapplePullAcceleration = 70f;   // how hard the rope pulls
    [SerializeField] private float grappleStopDistance = 2f;
    [SerializeField, Range(0f, 1f)] private float grappleGravityScale = 0.3f;
    [SerializeField] private ChargeMeter grappleCharge = new ChargeMeter();
    [SerializeField] private float grappleRampMultiplier = 2.5f;   // how much stronger the pull gets over time
    [SerializeField] private float grappleRampTime = 1.5f;         // seconds to reach full strength

    // Read-only access to settings for the states
    public float Gravity => gravity;
    public float GroundAcceleration => groundAcceleration;
    public float AirAcceleration => airAcceleration;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public float DashExitMomentum => dashExitMomentum;
    public float GrapplePullSpeed => grapplePullSpeed;
    public float GrapplePullAcceleration => grapplePullAcceleration;
    public float GrappleStopDistance => grappleStopDistance;
    public float GrappleGravityScale => grappleGravityScale;
    public ChargeMeter GrappleCharge => grappleCharge;
    public float GrappleRampMultiplier => grappleRampMultiplier;
    public float GrappleRampTime => grappleRampTime;
    public TelekinesisPower Telekinesis { get; private set; }
    public PlayerTelekinesisState TelekinesisState { get; private set; }

    // Shared data the states read and write
    public CharacterController Controller { get; private set; }
    public PlayerInputReader InputReader { get; private set; }
    public GrappleHook Hook { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set; }
    public float NextDashTime { get; set; }

    public bool CanDash => Time.time >= NextDashTime;
    private float groundY;
    private float peakY;

    // The state machine and the states it can switch between
    public StateMachine<PlayerController> Machine { get; private set; }
    public PlayerGroundedState Grounded { get; private set; }
    public PlayerAirborneState Airborne { get; private set; }
    public PlayerDashingState Dashing { get; private set; }
    public PlayerGrappleState Grapple { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputReader = GetComponent<PlayerInputReader>();
        Hook = GetComponent<GrappleHook>();
        Telekinesis = GetComponent<TelekinesisPower>(); // <-- ADD THIS

        Machine = new StateMachine<PlayerController>(this);
        Grounded = new PlayerGroundedState();
        Airborne = new PlayerAirborneState();
        Dashing = new PlayerDashingState();
        Grapple = new PlayerGrappleState();
        TelekinesisState = new PlayerTelekinesisState(); // <-- ADD THIS

        grappleCharge.Refill();
    }

    private void Start()
    {
        Machine.ChangeState(Grounded);
    }

    private void Update()
    {
        
        bool canStartTk = Machine.Current == Grounded || Machine.Current == Airborne;
        if (canStartTk && InputReader.TelekinesisPressed && Telekinesis.TryGrab())
        {
            Machine.ChangeState(TelekinesisState);
            return;
        }
        
        
        // The grapple can start from the ground or the air, but not mid-dash, and needs some charge
        bool canStartGrapple = Machine.Current == Grounded || Machine.Current == Airborne;
        
        if (canStartGrapple && InputReader.GrapplePressed && grappleCharge.CanStart && Hook.TryFire())
        {
            Machine.ChangeState(Grapple);
            return;
        }

        Machine.Tick();
        if (Machine.Current == Grounded)
        {
            groundY = transform.position.y;
            peakY = groundY;
        }
        else
        {
            peakY = Mathf.Max(peakY, transform.position.y);
        }

        // Charge refills whenever the grapple isn't in use
        if (Machine.Current != Grapple)
            grappleCharge.Regenerate(Time.deltaTime);
        
        
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

    // Temporary on-screen debug: state label and grapple charge bar
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 30), "State: " + Machine?.Current?.GetType().Name);
        GUI.Label(new Rect(10, 60, 700, 30),
            $"VertVel: {VerticalVelocity:F1}   Actual: {Controller.velocity.y:F1}   Frame: {Time.deltaTime * 1000f:F0} ms   Jump rise: {peakY - groundY:F2}");
        GUI.Box(new Rect(10, 40, 200, 16), GUIContent.none);
        GUI.Box(new Rect(10, 40, 200 * grappleCharge.Normalized, 16), GUIContent.none);
    }
}
*/


using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(GrappleHook))]
[RequireComponent(typeof(TelekinesisPower))]
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

    [Header("Grapple")]
    [SerializeField] private float grapplePullSpeed = 35f;          // top speed on the rope
    [SerializeField] private float grapplePullAcceleration = 70f;   // how hard the rope pulls
    [SerializeField] private float grappleStopDistance = 2f;
    [SerializeField, Range(0f, 1f)] private float grappleGravityScale = 0.3f;
    [SerializeField] private ChargeMeter grappleCharge = new ChargeMeter();
    [SerializeField] private float grappleRampMultiplier = 2.5f;   // how much stronger the pull gets over time
    [SerializeField] private float grappleRampTime = 1.5f;         // seconds to reach full strength

    // Read-only access to settings for the states
    public float Gravity => gravity;
    public float GroundAcceleration => groundAcceleration;
    public float AirAcceleration => airAcceleration;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public float DashExitMomentum => dashExitMomentum;
    public float GrapplePullSpeed => grapplePullSpeed;
    public float GrapplePullAcceleration => grapplePullAcceleration;
    public float GrappleStopDistance => grappleStopDistance;
    public float GrappleGravityScale => grappleGravityScale;
    public ChargeMeter GrappleCharge => grappleCharge;
    public float GrappleRampMultiplier => grappleRampMultiplier;
    public float GrappleRampTime => grappleRampTime;
    
    public TelekinesisPower Telekinesis { get; private set; }
    public PlayerTelekinesisState TelekinesisState { get; private set; }

    // Shared data the states read and write
    public CharacterController Controller { get; private set; }
    public PlayerInputReader InputReader { get; private set; }
    public GrappleHook Hook { get; private set; }
    public Vector3 HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set; }
    public float NextDashTime { get; set; }

    public bool CanDash => Time.time >= NextDashTime;
    private float groundY;
    private float peakY;

    // The state machine and the states it can switch between
    public StateMachine<PlayerController> Machine { get; private set; }
    public PlayerGroundedState Grounded { get; private set; }
    public PlayerAirborneState Airborne { get; private set; }
    public PlayerDashingState Dashing { get; private set; }
    public PlayerGrappleState Grapple { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputReader = GetComponent<PlayerInputReader>();
        Hook = GetComponent<GrappleHook>();
        Telekinesis = GetComponent<TelekinesisPower>();

        Machine = new StateMachine<PlayerController>(this);
        Grounded = new PlayerGroundedState();
        Airborne = new PlayerAirborneState();
        Dashing = new PlayerDashingState();
        Grapple = new PlayerGrappleState();
        TelekinesisState = new PlayerTelekinesisState();

        grappleCharge.Refill();
    }

    private void Start()
    {
        Machine.ChangeState(Grounded);
    }

    private void Update()
    {
        // 1. Handle global inputs and state transitions first
        if (TryHandleInputTransitions()) return;

        // 2. Tick the active state (movement, gravity, etc.)
        Machine.Tick();

        // 3. Handle frame clean-up / non-state logic
        HandlePostTick();
    }

    private bool TryHandleInputTransitions()
    {
        bool canStartStandardMove = Machine.Current == Grounded || Machine.Current == Airborne;

        // --- Telekinesis Check ---
        if (canStartStandardMove && InputReader.TelekinesisPressed && Telekinesis.TryGrab())
        {
            Machine.ChangeState(TelekinesisState);
            return true; // Handled! Stop checking other inputs this frame.
        }
    
        // --- Grapple Check ---
        if (canStartStandardMove && InputReader.GrapplePressed && grappleCharge.CanStart && Hook.TryFire())
        {
            Machine.ChangeState(Grapple);
            return true; // Handled! Stop checking other inputs this frame.
        }

        return false; // No transitions happened, continue normal tick
    }

    private void HandlePostTick()
    {
        if (Machine.Current == Grounded)
        {
            groundY = transform.position.y;
            peakY = groundY;
        }
        else
        {
            peakY = Mathf.Max(peakY, transform.position.y);
        }

        // Charge refills whenever the grapple isn't in use
        if (Machine.Current != Grapple)
            grappleCharge.Regenerate(Time.deltaTime);
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

    // Temporary on-screen debug: state label and grapple charge bar
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 30), "State: " + Machine?.Current?.GetType().Name);
        GUI.Label(new Rect(10, 60, 700, 30),
            $"VertVel: {VerticalVelocity:F1}   Actual: {Controller.velocity.y:F1}   Frame: {Time.deltaTime * 1000f:F0} ms   Jump rise: {peakY - groundY:F2}");
        GUI.Box(new Rect(10, 40, 200, 16), GUIContent.none);
        GUI.Box(new Rect(10, 40, 200 * grappleCharge.Normalized, 16), GUIContent.none);
    }
}
