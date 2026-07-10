using UnityEngine;

public class BotAttackState : IBotState
{
    private readonly BotController _bot;
    private readonly BotStateMachine _stateMachine;

    private Vector3 _basePosition;
    private float _floatTimer;

    private float _shotTimer;
    private float _cooldownTimer;
    private int _shotsFiredInBurst;

    private AttackPhase _phase;

    private enum AttackPhase
    {
        Burst,
        Cooldown
    }

    public BotAttackState(BotController bot, BotStateMachine stateMachine)
    {
        _bot = bot;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Attack Enter");

        _basePosition = _bot.transform.position;
        _floatTimer = 0f;

        _shotTimer = 0f;
        _cooldownTimer = 0f;
        _shotsFiredInBurst = 0;

        _phase = AttackPhase.Burst;
    }

    public void Update()
    {
        if (_bot.IsDead())
        {
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
            return;
        }

        ApplyFloating();

        switch (_phase)
        {
            case AttackPhase.Burst:
                UpdateBurst();
                break;

            case AttackPhase.Cooldown:
                UpdateCooldown();
                break;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            _bot.ForceDead();
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
        }
    }

    private void UpdateBurst()
    {
        _shotTimer += Time.deltaTime;

        if (_shotTimer < _bot.BurstInterval)
            return;

        _shotTimer = 0f;

        _bot.FireFromBothMuzzles();
        _shotsFiredInBurst++;

        if (_shotsFiredInBurst >= _bot.BurstCount)
        {
            _shotsFiredInBurst = 0;
            _cooldownTimer = 0f;
            _phase = AttackPhase.Cooldown;
        }
    }

    private void UpdateCooldown()
    {
        _cooldownTimer += Time.deltaTime;

        if (_cooldownTimer < _bot.BurstCooldown)
            return;

        _shotTimer = 0f;
        _phase = AttackPhase.Burst;
    }

    private void ApplyFloating()
    {
        _floatTimer += Time.deltaTime;

        float yOffset = Mathf.Sin(_floatTimer * _bot.AttackFloatSpeed) * _bot.AttackFloatAmplitude;

        Vector3 finalPos = _basePosition;
        finalPos.y += yOffset;

        _bot.transform.position = finalPos;
    }

    public void Exit()
    {
        Debug.Log("Attack Exit");
    }
}