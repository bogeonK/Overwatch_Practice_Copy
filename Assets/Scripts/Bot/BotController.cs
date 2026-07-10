using UnityEngine;

public enum BotTeamType
{
    Ally,
    Enemy
}
public enum BotBehaviorType
{
    Idle,
    Patrol,
    Attack
}

public class BotController : MonoBehaviour, IHealable
{
    [SerializeField] private BotTeamType teamType;
    [SerializeField] private BotBehaviorType botType;

    [Header("Health")]
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float hp = 100;
    [SerializeField] private BotHealthBar healthBar;
    [SerializeField] private BotHealthBar healthBarPrefab;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arriveDistance = 0.05f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float floatAmplitude = 0.2f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Attack")]
    [SerializeField] private Transform leftMuzzle;
    [SerializeField] private Transform rightMuzzle;
    [SerializeField] private BotProjectilePool projectilePool;
    [SerializeField] private float attackFloatAmplitude = 0.2f;
    [SerializeField] private float attackFloatSpeed = 2f;
    [SerializeField] private float burstInterval = 0.2f;   // 탕탕탕 사이 간격
    [SerializeField] private float burstCooldown = 2f;     // 한 세트 끝나고 쉬는 시간
    [SerializeField] private int burstCount = 3;

    [Header("Ground Follow")]
    [SerializeField] private float hoverHeight = 1.5f;
    [SerializeField] private float groundRayStartHeight = 5f;
    [SerializeField] private float groundCheckDistance = 20f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Respawn")]
    [SerializeField] private float respawnTime = 5f;

    private BotStateMachine _stateMachine;
    public float HoverHeight => hoverHeight;
    public float GroundRayStartHeight => groundRayStartHeight;
    public float GroundCheckDistance => groundCheckDistance;

    public float RespawnTime => respawnTime;
    public LayerMask GroundLayer => groundLayer;

    public BotTeamType TeamType => teamType;
    public BotBehaviorType BotType => botType;
    public Transform[] PatrolPoints => patrolPoints;
    public float MoveSpeed => moveSpeed;
    public float ArriveDistance => arriveDistance;
    public float RotateSpeed => rotateSpeed;
    public float FloatAmplitude => floatAmplitude;
    public float FloatSpeed => floatSpeed;

    public Transform LeftMuzzle => leftMuzzle;
    public Transform RightMuzzle => rightMuzzle;
    public BotProjectilePool ProjectilePool => projectilePool;
    public float AttackFloatAmplitude => attackFloatAmplitude;
    public float AttackFloatSpeed => attackFloatSpeed;
    public float BurstInterval => burstInterval;
    public float BurstCooldown => burstCooldown;
    public int BurstCount => burstCount;


    private void Awake()
    {
        _stateMachine = new BotStateMachine();
    }

    private void Start()
    {
        hp = maxHp;

        CreateHealthBar();

        if (teamType == BotTeamType.Ally && botType == BotBehaviorType.Attack)
        {
            botType = BotBehaviorType.Idle;
        }

        switch (botType)
        {
            case BotBehaviorType.Idle:
                _stateMachine.Initialize(new BotIdleState(this, _stateMachine));
                break;

            case BotBehaviorType.Patrol:
                _stateMachine.Initialize(new BotPatrolState(this, _stateMachine));
                break;

            case BotBehaviorType.Attack:
                _stateMachine.Initialize(new BotAttackState(this, _stateMachine));
                break;
        }
    }

    private void Update()
    {
        _stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(100);
        }
    }

    public bool IsAlly()
    {
        return teamType == BotTeamType.Ally;
    }

    public bool IsEnemy()
    {
        return teamType == BotTeamType.Enemy;
    }

    public bool IsDead()
    {
        return hp <= 0;
    }

    public void TakeDamage(int damage)
    {

        if (IsDead())
            return;


        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, maxHp);


        if (hp <= 0f)
        {
            GameController.instance.GetManager<SoundManager>().PlaySFX(SoundId.BotDeath);

            if (healthBar != null)
                healthBar.HideImmediately();

            _stateMachine.ChangeState(new BotDeadState(this, _stateMachine));
            return;
        }

        if (healthBar != null)
            healthBar.Show(Mathf.RoundToInt(hp), Mathf.RoundToInt(maxHp));
    }

    //죽음테스트
    public void ForceDead()
    {
        hp = 0f;

        if (healthBar != null)
            healthBar.HideImmediately();
    }


    public void FireFromBothMuzzles()
    {
        if (projectilePool == null)
        {
            Debug.LogWarning($"{name} : ProjectilePool이 없음");
            return;
        }

        if (leftMuzzle != null)
            projectilePool.Spawn(leftMuzzle.position, leftMuzzle.rotation);

        if (rightMuzzle != null)
            projectilePool.Spawn(rightMuzzle.position, rightMuzzle.rotation);
    }

    private void CreateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetTarget(transform);
            healthBar.HideImmediately();
            return;
        }

        if (healthBarPrefab == null)
        {
            return;
        }

        healthBar = Instantiate(healthBarPrefab);
        healthBar.SetTarget(transform);
        healthBar.HideImmediately();
    }

    //리스폰
    public void Respawn()
    {
        hp = maxHp;

        var explosion = GetComponent<EnemyExplosion>();
        if (explosion != null)
            explosion.Respawn();

        switch (botType)
        {
            case BotBehaviorType.Idle:
                _stateMachine.ChangeState(new BotIdleState(this, _stateMachine));
                break;

            case BotBehaviorType.Patrol:
                _stateMachine.ChangeState(new BotPatrolState(this, _stateMachine));
                break;

            case BotBehaviorType.Attack:
                _stateMachine.ChangeState(new BotAttackState(this, _stateMachine));
                break;
        }
    }

    //힐
    public void Heal(float amount)
    {
        if (!IsAlly())
            return;

        if (IsDead())
            return;

        hp += amount;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        if (healthBar != null)
            healthBar.Show(Mathf.RoundToInt(hp), Mathf.RoundToInt(maxHp));
    }
}