using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float groundAcceleration = 60f;
    [SerializeField] private float airAcceleration = 20f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -25f;

    private CharacterController controller;
    private PlayerInputReader input;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        bool grounded = controller.isGrounded;

        // Small downward push keeps the controller stuck to the ground
        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        
        Vector2 moveInput = input.Move;
        Vector3 wishDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 targetVelocity = Vector3.ClampMagnitude(wishDirection, 1f) * moveSpeed;

        // Move toward the target gradually instead of instantly (this is the start of momentum)
        float acceleration = grounded ? groundAcceleration : airAcceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);

        if (grounded && input.JumpPressed)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}