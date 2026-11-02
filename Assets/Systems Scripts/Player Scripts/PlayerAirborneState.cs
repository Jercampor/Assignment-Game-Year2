public class PlayerAirborneState : State<PlayerController>
{
    public override void Tick(PlayerController p)
    {
        if (p.InputReader.DashPressed && p.CanDash)
        {
            p.Machine.ChangeState(p.Dashing);
            return;
        }

        p.ApplyHorizontalAcceleration(p.AirAcceleration);
        p.ApplyGravity();
        p.MoveCharacter();

        if (p.Controller.isGrounded && p.VerticalVelocity <= 0f)
            p.Machine.ChangeState(p.Grounded);
    }
}