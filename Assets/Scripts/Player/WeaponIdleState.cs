using UnityEngine;

public class WeaponIdleState : IPlayerState
{
    private PlayerController controller;

    public WeaponIdleState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Update()
    {

        if (controller.IsReloadPressed())
        {
            controller.ReloadWeapon();
            return;
        }
        if (controller.IsFirePressed() && controller.CanFire())
        {
            controller.WeaponStateMachine.ChangeState(controller.WeaponFireState);
            return;
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}