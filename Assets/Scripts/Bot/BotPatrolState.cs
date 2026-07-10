using UnityEngine;

public class BotPatrolState : IBotState
{
    private readonly BotController _bot;
    private readonly BotStateMachine _stateMachine;

    private int _currentIndex;
    private Vector3 _basePosition;
    private float _floatTimer;

    public BotPatrolState(BotController bot, BotStateMachine stateMachine)
    {
        _bot = bot;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Patrol Enter");

        if (_bot.PatrolPoints == null || _bot.PatrolPoints.Length < 2)
        {
            return;
        }

        _currentIndex = 0;

        Vector3 startPos = _bot.PatrolPoints[_currentIndex].position;
        _basePosition = startPos;
        _basePosition.y = GetGroundFollowY(startPos);

        _bot.transform.position = _basePosition;

        _currentIndex = GetNextIndex(_currentIndex);
        _floatTimer = 0f;
    }

    public void Update()
    {
        if (_bot.IsDead())
        {
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
            return;
        }

        if (_bot.PatrolPoints == null || _bot.PatrolPoints.Length < 2)
            return;

        Transform target = _bot.PatrolPoints[_currentIndex];

        if (target == null)
            return;

        Vector3 targetPos = target.position;

        Vector3 currentXZ = new Vector3(_basePosition.x, 0f, _basePosition.z);
        Vector3 targetXZ = new Vector3(targetPos.x, 0f, targetPos.z);

        Vector3 dir = targetXZ - currentXZ;

        if (dir.magnitude <= _bot.ArriveDistance)
        {
            _currentIndex = GetNextIndex(_currentIndex);
            return;
        }

        RotateToDirection(dir);

        Vector3 nextXZ = Vector3.MoveTowards(
            currentXZ,
            targetXZ,
            _bot.MoveSpeed * Time.deltaTime
        );

        _basePosition.x = nextXZ.x;
        _basePosition.z = nextXZ.z;
        _basePosition.y = GetGroundFollowY(_basePosition);

        ApplyFloating();


        //죽음테스트
        if (Input.GetKeyDown(KeyCode.K))
        {
            _bot.ForceDead();
            _stateMachine.ChangeState(new BotDeadState(_bot, _stateMachine));
        }
    }

    private float GetGroundFollowY(Vector3 position)
    {
        Vector3 rayOrigin = position + Vector3.up * _bot.GroundRayStartHeight;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            _bot.GroundCheckDistance,
            _bot.GroundLayer))
        {
            return hit.point.y + _bot.HoverHeight;
        }

        return position.y;
    }

    private void RotateToDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude <= 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);

        _bot.transform.rotation = Quaternion.RotateTowards(
            _bot.transform.rotation,
            targetRot,
            _bot.RotateSpeed * Time.deltaTime
        );
    }

    //공중부양
    private void ApplyFloating()
    {
        _floatTimer += Time.deltaTime;

        float yOffset = Mathf.Sin(_floatTimer * _bot.FloatSpeed) * _bot.FloatAmplitude;

        Vector3 finalPos = _basePosition;
        finalPos.y += yOffset;

        _bot.transform.position = finalPos;
    }

    private int GetNextIndex(int currentIndex)
    {
        return (currentIndex + 1) % _bot.PatrolPoints.Length;
    }



    public void Exit()
    {
        Debug.Log("Patrol Exit");
    }
}
