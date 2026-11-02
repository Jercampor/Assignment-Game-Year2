using System.Collections;
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

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 28f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField, Range(0f, 1f)] private float dashExitMomentum = 0.6f;

    private CharacterController controller;
    private PlayerInputReader input;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private bool isDashing;
    private Vector3 dashVelocity;
    private float nextDashTime;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        // While dashing, ignore normal movement and just travel in a straight line
        if (isDashing)
        {
            controller.Move(dashVelocity * Time.deltaTime);
            return;
        }

        bool grounded = controller.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        Vector2 moveInput = input.Move;
        Vector3 wishDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 targetVelocity = Vector3.ClampMagnitude(wishDirection, 1f) * moveSpeed;

        float acceleration = grounded ? groundAcceleration : airAcceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);

        if (grounded && input.JumpPressed)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (input.DashPressed && Time.time >= nextDashTime)
        {
            // Dash where you're steering, or straight ahead if no keys are held
            Vector3 dashDirection = wishDirection.sqrMagnitude > 0.01f ? wishDirection.normalized : transform.forward;
            StartCoroutine(DashRoutine(dashDirection));
            return;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator DashRoutine(Vector3 direction)
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;
        dashVelocity = direction * dashSpeed;
        verticalVelocity = 0f;

        yield return new WaitForSeconds(dashDuration);

        // Keep part of the dash speed so it flows into your movement instead of stopping dead
        horizontalVelocity = direction * dashSpeed * dashExitMomentum;
        isDashing = false;
    }
}