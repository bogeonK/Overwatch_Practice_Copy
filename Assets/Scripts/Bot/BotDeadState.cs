using System.Collections;
using UnityEngine;

public class BotDeadState : IBotState
{
    private readonly BotController _bot;
    private readonly BotStateMachine _stateMachine;

    public BotDeadState(BotController bot, BotStateMachine stateMachine)
    {
        _bot = bot;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Dead Enter");

        var explosion = _bot.GetComponent<EnemyExplosion>();
        if (explosion != null)
        {
            explosion.Die();
        }

        _bot.StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(_bot.RespawnTime);

        _bot.Respawn();
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}