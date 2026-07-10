using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float dashSpeed = 7f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float riseMultiplier = 2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float stickToGroundForce = 20f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;


    [Header("Fire Settings")]
    [SerializeField] private float fireRate = 0.12f;
    [SerializeField] private float fireStateDuration = 0.08f;
    private bool reloadPressed;
    //[SerializeField] private float fireDamage = 10f;
    //[SerializeField] private float fireRange = 100f;
    //[SerializeField] private LayerMask fireHitMask;

    //[Header("Ultimate Gauge")]
    //[SerializeField] private UltimateGauge ultimateGauge;

    [Header("Weapon Attack")]
    [SerializeField] private MonoBehaviour weaponAttackObj;

    private IWeaponAttack weaponAttack;

    [Header("Character Option")]
    [SerializeField] private bool useShiftAsDash = true;

    //[Header("View Model")]
    //[SerializeField] private ViewModelController viewModelController;

    private bool firePressed;
    private float nextFireTime;
    private int fireHash;

    private int jumpHash;
    private int groundedHash;

    private bool jumpPressed;

    private bool isDash;
    private int dashHash;

    private Rigidbody rb;
    private Animator anim;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    public Vector2 MoveInput => moveInput;

    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }

    public PlayerDashState DashState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }

    public PlayerStateMachine WeaponStateMachine { get; private set; }

    public WeaponIdleState WeaponIdleState { get; private set; }
    public WeaponFireState WeaponFireState { get; private set; }

    private int speedHash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        speedHash = Animator.StringToHash("Speed");
        dashHash = Animator.StringToHash("Dash");
        jumpHash = Animator.StringToHash("Jump");
        groundedHash = Animator.StringToHash("Grounded");
        //fireHash = Animator.StringToHash("Fire");

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        DashState = new PlayerDashState(this);
        JumpState = new PlayerJumpState(this);

        WeaponStateMachine = new PlayerStateMachine();
        WeaponIdleState = new WeaponIdleState(this);
        WeaponFireState = new WeaponFireState(this);

        weaponAttack = weaponAttackObj as IWeaponAttack;
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
        WeaponStateMachine.Initialize(WeaponIdleState);
    }

    private void Update()
    {
        ReadInput();
        StateMachine.Update();
        WeaponStateMachine.Update();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void ReadInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        reloadPressed = Input.GetKeyDown(KeyCode.R);


        isDash = useShiftAsDash && Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        firePressed = Input.GetMouseButton(0);

        if (cameraTransform == null)
        {
            moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            return;
        }

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;
        moveDirection.Normalize();

    }

    //이동관련
    public Vector3 GetVelocity()
    {
        return rb.linearVelocity;
    }
    public void StopHorizontalMovement()
    {
        rb.linearVelocity = new Vector3(
            0f,
            rb.linearVelocity.y,
            0f
        );
    }

    public bool HasMoveInput()
    {
        return moveInput.sqrMagnitude > 0.01f;
    }

    public void HandleMovement()
    {
        if (!HasMoveInput())
        {
            StopHorizontalMovement();
            return;
        }

        Vector3 velocity = moveDirection * moveSpeed;

        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );
    }

    //public void HandleRotation()
    //{
    //    if (!HasMoveInput())
    //        return;

    //    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //    Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    //    rb.MoveRotation(newRotation);
    //}

    //점프
    public bool IsJumpPressed()
    {
        return jumpPressed;
    }
    public bool IsGrounded()
    {
        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);
    }
    public void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            jumpForce,
            rb.linearVelocity.z
        );
    }

    public void PlayJumpAnimation()
    {
        anim.SetTrigger(jumpHash);
    }
    public void HandleAirMovement(Vector3 airVelocity)
    {
        rb.linearVelocity = new Vector3(
            airVelocity.x,
            rb.linearVelocity.y,
            airVelocity.z
        );
    }

    public void ApplyBetterGravity()
    {
        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (riseMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

    }

    public void StickToGround()
    {
        if (!IsGrounded())
            return;

        if (rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }

        rb.AddForce(Vector3.down * stickToGroundForce, ForceMode.Acceleration);
    }

    //대쉬
    public bool IsDashPressed()
    {
        return isDash;
    }
    public void HandleDashMovement()
    {
        if (!HasMoveInput())
        {
            StopHorizontalMovement();
            return;
        }

        Vector3 velocity = moveDirection * dashSpeed;

        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );
    }

    //발사
    public bool IsFirePressed()
    {
        return firePressed;
    }

    public bool CanFire()
    {
        return Time.time >= nextFireTime &&
               weaponAttack != null &&
               weaponAttack.CanFire();
    }
    public bool IsReloadPressed()
    {
        return reloadPressed;
    }
    public void ReloadWeapon()
    {
        if (weaponAttack != null)
            weaponAttack.Reload();
    }


    //public void PlayFireAnimation()
    //{
    //    anim.SetTrigger(fireHash);
    //}

    public void FireRifle()
    {
        //nextFireTime = Time.time + fireRate;

        //PlayFireAnimation();

        //if (viewModelController != null)
        //{
        //    viewModelController.PlayFire();
        //}

        //ShootRay();
        nextFireTime = Time.time + fireRate;

        if (weaponAttack != null)
            weaponAttack.Fire();
    }

    //private void ShootRay()
    //{
    //    if (cameraTransform == null)
    //    {
    //        Debug.LogWarning("카메라 Transform이 PlayerController에 연결되지 않았습니다.");
    //        return;
    //    }

    //    Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

    //    if (Physics.Raycast(ray, out RaycastHit hit, fireRange, fireHitMask))
    //    {
    //        Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f);

    //        Debug.Log("총알 명중: " + hit.collider.name);

    //        BotController bot = hit.collider.GetComponentInParent<BotController>();

    //        if (bot != null)
    //        {
    //            bot.TakeDamage((int)fireDamage);

    //            if (ultimateGauge != null)
    //            {
    //                ultimateGauge.AddGauge(1f);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.DrawRay(ray.origin, ray.direction * fireRange, Color.red, 0.2f);
    //    }
    //}

    public float GetFireStateDuration()
    {
        return fireStateDuration;
    }

    public void SetMoveSpeed(float value)
    {
        anim.SetFloat(speedHash, value);
    }

    //캐릭터 옵션
    public void SetUseShiftAsDash(bool value)
    {
        useShiftAsDash = value;
    }

    private void UpdateAnimator()
    {
        float targetSpeed = HasMoveInput() ? 1f : 0f;
        anim.SetFloat(speedHash, targetSpeed, 0.1f, Time.deltaTime);
        anim.SetBool(dashHash, IsDashPressed() && HasMoveInput());
        anim.SetBool(groundedHash, IsGrounded());
    }

  

}