using UnityEngine;


public class PlayerMoveState : IPlayerState
{
    private PlayerController controller;

    public PlayerMoveState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.SetMoveSpeed(1f);
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
        }

        if (controller.IsDashPressed())
        {
            controller.StateMachine.ChangeState(controller.DashState);
            return;
        }
    }

    public void FixedUpdate()
    {
        controller.HandleMovement();
        //controller.HandleRotation();
        controller.StickToGround();
    }

    public void Exit()
    {
        controller.SetMoveSpeed(0f);
    }
}