public class PlayerGroundedState : State<PlayerController>
{
    public override void Tick(PlayerController p)
    {
        if (p.InputReader.DashPressed && p.CanDash)
        {
            p.Machine.ChangeState(p.Dashing);
            return;
        }

        // Small downward push keeps the controller stuck to the ground
        if (p.VerticalVelocity < 0f)
            p.VerticalVelocity = -2f;

        p.ApplyHorizontalAcceleration(p.GroundAcceleration);

        bool jumped = p.InputReader.JumpPressed;
        if (jumped)
            p.Jump();

        p.ApplyGravity();
        p.MoveCharacter();

        if (jumped || !p.Controller.isGrounded)
            p.Machine.ChangeState(p.Airborne);
    }
}