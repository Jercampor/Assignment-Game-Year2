using UnityEngine;

public class PlayerGrappleState : State<PlayerController>
{
    private const float AimAboveAnchor = 1f;   // pull toward a point slightly above the surface so you clear ledges
    private const float ArrivalBoost = 8f;     // upward kick on arrival when the anchor is above you

    public override void Tick(PlayerController p)
    {
        GrappleHook hook = p.Hook;

        // Dash out of a grapple
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

        // Right-click again, or the rope was lost
        if (p.InputReader.GrapplePressed || !hook.IsActive)
        {
            p.Machine.ChangeState(p.Airborne);
            return;
        }

        // Rope still travelling out: no gravity, so you don't drop before the pull starts
        if (hook.IsFiring)
        {
            p.ApplyHorizontalAcceleration(p.AirAcceleration);
            p.MoveCharacter();
            return;
        }

        // Rope attached: pull toward a point just above the anchor
        Vector3 aimPoint = hook.AnchorPoint + Vector3.up * AimAboveAnchor;
        Vector3 toAim = aimPoint - p.transform.position;

        if (toAim.magnitude <= p.GrappleStopDistance)
        {
            // Arrived: if the anchor was above you, kick upward so you can vault over the edge
            if (hook.AnchorPoint.y > p.transform.position.y)
                p.VerticalVelocity = Mathf.Max(p.VerticalVelocity, ArrivalBoost);

            p.Machine.ChangeState(p.Airborne);
            return;
        }

        Vector3 target = toAim.normalized * p.GrapplePullSpeed;
        Vector3 current = p.HorizontalVelocity + Vector3.up * p.VerticalVelocity;
        Vector3 next = Vector3.MoveTowards(current, target, p.GrapplePullAcceleration * Time.deltaTime);

        p.HorizontalVelocity = new Vector3(next.x, 0f, next.z);
        p.VerticalVelocity = next.y;
        p.MoveCharacter();
    }

    public override void Exit(PlayerController p)
    {
        // You keep your velocity, so releasing mid-pull flings you onward
        p.Hook.Release();
    }
}