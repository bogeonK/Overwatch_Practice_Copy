using UnityEngine;

public class WeaponFireState : IPlayerState
{
    private PlayerController controller;
    private float timer;

    public WeaponFireState(PlayerController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        timer = 0f;

        controller.FireRifle();
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= controller.GetFireStateDuration())
        {
            controller.WeaponStateMachine.ChangeState(controller.WeaponIdleState);
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