using UnityEngine;

public class PlayerTelekinesisState : State<PlayerController>
{
    public override void Tick(PlayerController p)
    {
        TelekinesisPower tk = p.Telekinesis;

        // Left click throws the held object
        if (p.InputReader.ThrowPressed && tk.IsHolding)
        {
            tk.Throw();
            Leave(p);
            return;
        }

        // Press the button again, or the object is lost: just drop it
        if (p.InputReader.TelekinesisPressed || !tk.IsHolding)
        {
            tk.Release();
            Leave(p);
            return;
        }

        // Normal movement so you can walk and jump while holding it
        bool grounded = p.Controller.isGrounded;

        if (grounded && p.VerticalVelocity < 0f)
            p.VerticalVelocity = -2f;

        if (grounded && p.InputReader.JumpPressed)
            p.Jump();

        p.ApplyHorizontalAcceleration(grounded ? p.GroundAcceleration : p.AirAcceleration);
        p.ApplyGravity();
        p.MoveCharacter();
    }

    private void Leave(PlayerController p)
    {
        p.Machine.ChangeState(p.Controller.isGrounded ? p.Grounded : p.Airborne);
    }

    public override void Exit(PlayerController p)
    {
        // Safety cleanup if we leave the state unexpectedly
        if (p.Telekinesis.IsHolding)
            p.Telekinesis.Release();
    }
}