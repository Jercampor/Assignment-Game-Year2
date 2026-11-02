using UnityEngine;

public class PlayerGrappleState : State<PlayerController>
{
    private const float AimAboveAnchor = 1f;   // aim above the surface so you clear ledges
    private const float AimOffWall = 1f;       // aim away from the wall so you slide up it
    private const float ArrivalBraking = 25f;  // slows you when you reach the anchor so you don't orbit it

    private float startTime;

    public override void Enter(PlayerController p)
    {
        startTime = Time.time;
    }

    public override void Tick(PlayerController p)
{
    GrappleHook hook = p.Hook;
    ChargeMeter charge = p.GrappleCharge;

    // Using the grapple costs charge every second
    charge.Drain(Time.deltaTime);

    if (p.InputReader.DashPressed && p.CanDash)
    {
        p.Machine.ChangeState(p.Dashing);
        return;
    }

    // Jump: release the rope and jump, keeping any upward speed you already have
    if (p.InputReader.JumpPressed)
    {
        float keptVertical = p.VerticalVelocity;
        p.Jump();
        p.VerticalVelocity = Mathf.Max(p.VerticalVelocity, keptVertical);
        p.Machine.ChangeState(p.Airborne);
        return;
    }

    // Player ended it, the rope was lost, or the charge ran out
    if (p.InputReader.GrapplePressed || !hook.IsActive || charge.IsEmpty)
    {
        p.Machine.ChangeState(p.Airborne);
        return;
    }

    float dt = Time.deltaTime;

    // Pull strength and top speed grow the longer you stay on the rope
    float t = Mathf.Clamp01((Time.time - startTime) / p.GrappleRampTime);
    float ramp = Mathf.Lerp(1f, p.GrappleRampMultiplier, t);

    Vector3 velocity = p.HorizontalVelocity + Vector3.up * p.VerticalVelocity;

    // Reduced gravity while on the rope so you swing instead of dropping
    velocity += Vector3.up * p.Gravity * p.GrappleGravityScale * dt;

    if (hook.IsAttached)
    {
        Vector3 aimPoint = hook.AnchorPoint
                         + hook.AnchorNormal * AimOffWall
                         + Vector3.up * AimAboveAnchor;
        Vector3 toAim = aimPoint - p.transform.position;

        // Check if we are physically blocked by a wall/obstacle in our way
        bool isBlockedByWall = (p.Controller.collisionFlags & CollisionFlags.CollidedSides) != 0;

        if (toAim.magnitude > p.GrappleStopDistance)
        {
            // ONLY add pull acceleration if we aren't slamming directly into a wall
            if (!isBlockedByWall)
            {
                velocity += toAim.normalized * p.GrapplePullAcceleration * ramp * dt;
            }
            else
            {
                // If we hit a wall, kill the pull velocity pushing into it so it doesn't build up
                velocity = Vector3.ProjectOnPlane(velocity, p.Controller.velocity.normalized);
            }
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, ArrivalBraking * dt);
        }
    }

    // Also note: clamping with 'ramp' can make max speed super high (e.g. 35 * 2.5 = 87.5 units/sec!). 
    // If it's still too fast, consider clamping to p.GrapplePullSpeed instead of multiplying by ramp.
    velocity = Vector3.ClampMagnitude(velocity, p.GrapplePullSpeed * ramp);

    p.HorizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
    p.VerticalVelocity = velocity.y;
    p.MoveCharacter();

    // Read back what actually happened, so walls and ceilings can't store up fake speed
    Vector3 actual = p.Controller.velocity;
    p.HorizontalVelocity = new Vector3(actual.x, 0f, actual.z);
    p.VerticalVelocity = actual.y;
}

    public override void Exit(PlayerController p)
    {
        // You keep your velocity, so releasing mid-pull flings you onward
        p.Hook.Release();
    }
}