using UnityEngine;

public class PlayerDashState : IPlayerState
{
    private PlayerController controller;

    public PlayerDashState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.SetMoveSpeed(2f);
    }

    public void Update()
    {
        if (controller.IsJumpPressed())
        {
            controller.StateMachine.ChangeState(controller.JumpState);
            return;
        }

        if (!controller.HasMoveInput())
        {
            controller.StateMachine.ChangeState(controller.IdleState);
            return;
        }

        if (!controller.IsDashPressed())
        {
            controller.StateMachine.ChangeState(controller.MoveState);
        }
    }

    public void FixedUpdate()
    {
        controller.HandleDashMovement();
        //controller.HandleRotation();
        controller.StickToGround();
    }

    public void Exit()
    {
    }
}