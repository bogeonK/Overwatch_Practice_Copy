using UnityEngine;

public class PlayerJumpState : IPlayerState
{
    private PlayerController controller;
    private float timer;
    private Vector3 airVelocity;

    public PlayerJumpState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        timer = 0f;
        Vector3 currentVelocity = controller.GetVelocity();
        airVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        controller.PlayJumpAnimation();
        controller.Jump();
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0.15f)
            return;

        if (controller.IsGrounded())
        {
            if (controller.HasMoveInput())
            {
                if (controller.IsDashPressed())
                    controller.StateMachine.ChangeState(controller.DashState);
                else
                    controller.StateMachine.ChangeState(controller.MoveState);
            }
            else
            {
                controller.StateMachine.ChangeState(controller.IdleState);
            }
        }
    }

    public void FixedUpdate()
    {
        controller.HandleAirMovement(airVelocity);
        controller.ApplyBetterGravity();
    }

    public void Exit()
    {
    }
}