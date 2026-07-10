using UnityEngine;

public class BotIdleState : IBotState
{
    private readonly BotController _bot;
    private readonly BotStateMachine _stateMachine;

    private Vector3 _startPos;
    private float _floatTimer;

    public BotIdleState(BotController bot, BotStateMachine stateMachine)
    {
        _bot = bot;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Idle Enter");
        _startPos = _bot.transform.position;
        _floatTimer = 0f;
    }

    public void Update()
    {
        if (_bot.IsDead())
        {
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
            return;
        }

        //공중부양 연출
        _floatTimer += Time.deltaTime;

        float yOffset = Mathf.Sin(_floatTimer * 2f) * 0.2f;

        Vector3 pos = _startPos;
        pos.y += yOffset;
        _bot.transform.position = pos;

        //죽음테스트
        if (Input.GetKeyDown(KeyCode.K))
        {
            _bot.ForceDead();
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
            return;
        }
    }

    public void Exit()
    {
        Debug.Log("Idle Exit");
    }
}