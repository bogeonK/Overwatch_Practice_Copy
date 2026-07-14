using UnityEngine;

public class MissileSkill : MonoBehaviour, ISkill
{
    [Header("Missile")]
    [SerializeField] private MissileProjectile missilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Camera Aim")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float aimRange = 100f;
    [SerializeField] private LayerMask aimMask;

    [Header("Damage")]
    [SerializeField] private float normalAttackDamage = 10f;
    [SerializeField] private float damageMultiplier = 3f;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 8f;

    [Header("ViewModel")]
    [SerializeField] private ViewModelController viewModelController;

    [Header("Ultimate Gauge")]
    [SerializeField] private UltimateGauge ultimateGauge;

    private float cooldownTimer;

    public float CurrentCooldown => cooldownTimer;
    public float MaxCooldown => cooldown;

    public void Tick(float deltaTime)
    {
        if (cooldownTimer <= 0f)
            return;

        cooldownTimer -= deltaTime;

        if (cooldownTimer < 0f)
            cooldownTimer = 0f;
    }

    public bool CanUse()
    {
        return cooldownTimer <= 0f;
    }

    public void Use()
    {
        if (!CanUse())
            return;

        if (missilePrefab == null || firePoint == null)
            return;

        Vector3 targetPoint = GetAimTargetPoint();

        Vector3 fireDirection =
            (targetPoint - firePoint.position).normalized;

        GameController.instance.GetManager<SoundManager>().PlaySFX(SoundId.MissileFire);

        MissileProjectile missile = Instantiate(
            missilePrefab,
            firePoint.position,
            Quaternion.LookRotation(fireDirection)
        );

        missile.SetDamage(normalAttackDamage * damageMultiplier);
        missile.SetUltimateGauge(ultimateGauge);

        if (viewModelController != null)
            viewModelController.PlayMissile();

        cooldownTimer = cooldown;
    }

    private Vector3 GetAimTargetPoint()
    {
        if (playerCamera == null)
            return firePoint.position + firePoint.forward * aimRange;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, aimRange, aimMask))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * aimRange;
    }
}