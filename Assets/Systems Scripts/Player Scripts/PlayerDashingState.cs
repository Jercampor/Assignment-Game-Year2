using System.Collections;
using UnityEngine;

public class PlayerDashingState : State<PlayerController>
{
    private Vector3 direction;
    private Coroutine routine;

    public override void Enter(PlayerController p)
    {
        // Dash where you're steering, or straight ahead if no keys are held
        Vector3 wish = p.GetWishDirection();
        direction = wish.sqrMagnitude > 0.01f ? wish.normalized : p.transform.forward;

        p.NextDashTime = Time.time + p.DashCooldown;
        p.VerticalVelocity = 0f;

        routine = p.StartCoroutine(DashRoutine(p));
    }

    public override void Tick(PlayerController p)
    {
        p.Controller.Move(direction * p.DashSpeed * Time.deltaTime);
    }

    public override void Exit(PlayerController p)
    {
        // Keep part of the dash speed so it flows into normal movement
        p.HorizontalVelocity = direction * p.DashSpeed * p.DashExitMomentum;

        if (routine != null)
            p.StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator DashRoutine(PlayerController p)
    {
        yield return new WaitForSeconds(p.DashDuration);
        p.Machine.ChangeState(p.Airborne);
    }
}