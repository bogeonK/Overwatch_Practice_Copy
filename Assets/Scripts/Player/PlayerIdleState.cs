using UnityEngine;


public class PlayerIdleState : IPlayerState
{
    private PlayerController controller;

    public PlayerIdleState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        controller.SetMoveSpeed(0f);
        controller.StopHorizontalMovement();
    }

    public void Update()
    {
        if (controller.IsJumpPressed())
        {
            controller.StateMachine.ChangeState(controller.JumpState);
            return;
        }

        if (controller.HasMoveInput())
        {
            if (controller.IsDashPressed())
            {
                controller.StateMachine.ChangeState(controller.DashState);
            }
            else
            {
                controller.StateMachine.ChangeState(controller.MoveState);
            }
        }
    }

    public void FixedUpdate()
    {
        //controller.HandleRotation();
        controller.StickToGround();
    }

    public void Exit()
    {
    }
}
