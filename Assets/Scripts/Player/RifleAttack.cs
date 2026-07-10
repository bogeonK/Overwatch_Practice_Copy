using System.Collections;
using UnityEngine;

public class RifleAttack : MonoBehaviour, IWeaponAttack
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private ViewModelController viewModelController;
    [SerializeField] private UltimateGauge ultimateGauge;
    [SerializeField] private UltimateSkill ultimateSkill;

    [SerializeField] private WeaponVFX weaponVFX;
    [SerializeField] private PlayerHUD playerHUD;

    [Header("Fire Settings")]
    [SerializeField] private float fireDamage = 1f;
    [SerializeField] private float fireRange = 100f;
    [SerializeField] private LayerMask fireHitMask;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string fireTriggerName = "Fire";
    //[SerializeField] private string reloadTriggerName = "Reload";

    [Header("Reload Motion")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private float liftAngle = -18f;
    [SerializeField] private float pushForward = 0.08f;
    [SerializeField] private float pullBack = -0.12f;
    [SerializeField] private float reloadMotionSpeed = 12f;

    [Header("Ultimate Auto Aim")]
    [SerializeField] private float autoAimRadius = 0.35f;
    [SerializeField] private LayerMask targetMask;

    [Header("Impact Decal")]
    [SerializeField] private DecalPool decalPool;
    [SerializeField] private float impactOffset = 0.01f;

    private int currentAmmo;
    private bool isReloading;

    private int fireHash;
    //private int reloadHash;

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;
    private Coroutine reloadMotionCoroutine;

    private void Awake()
    {
        fireHash = Animator.StringToHash(fireTriggerName);
        //reloadHash = Animator.StringToHash(reloadTriggerName);

        currentAmmo = maxAmmo;
    }

    private void Start()
    {
        if (weaponTransform != null)
        {
            originLocalPosition = weaponTransform.localPosition;
            originLocalRotation = weaponTransform.localRotation;
        }
        UpdateAmmoUI();
    }


    public void Fire()
    {
        if (!CanFire())
        {
            if (currentAmmo <= 0)
                Reload();

            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (animator != null)
            animator.SetTrigger(fireHash);

        if (viewModelController != null)
            viewModelController.PlayFire();

        Vector3 endPoint = ShootRay();

        if (weaponVFX != null)
            weaponVFX.PlayFireEffect(endPoint);

        GameController.instance.GetManager<SoundManager>().PlaySFX(SoundId.RifleFire);

        if (currentAmmo <= 0)
            Reload();
    }

    public bool CanFire()
    {
        return !isReloading && currentAmmo > 0;
    }

    public void Reload()
    {
        if (isReloading) return;
        if (currentAmmo >= maxAmmo) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (reloadMotionCoroutine != null)
            StopCoroutine(reloadMotionCoroutine);

        reloadMotionCoroutine = StartCoroutine(ReloadMotionRoutine());

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateAmmoUI();
    }

    private IEnumerator ReloadMotionRoutine()
    {
        if (weaponTransform == null)
            yield break;

        Vector3 liftPos = originLocalPosition + new Vector3(0f, 0.04f, pushForward);
        Quaternion liftRot = originLocalRotation * Quaternion.Euler(liftAngle, 0f, 0f);

        Vector3 pullPos = originLocalPosition + new Vector3(0f, 0.02f, pullBack);
        Quaternion pullRot = originLocalRotation * Quaternion.Euler(liftAngle * 0.5f, 0f, 0f);

        yield return MoveWeapon(liftPos, liftRot, 0.18f);

        yield return MoveWeapon(pullPos, pullRot, 0.12f);

        yield return MoveWeapon(liftPos, liftRot, 0.08f);

        yield return MoveWeapon(originLocalPosition, originLocalRotation, 0.18f);
    }

    private IEnumerator MoveWeapon(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = weaponTransform.localPosition;
        Quaternion startRot = weaponTransform.localRotation;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            weaponTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            weaponTransform.localRotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        weaponTransform.localPosition = targetPos;
        weaponTransform.localRotation = targetRot;
    }

    private void UpdateAmmoUI()
    {
        if (playerHUD != null)
            playerHUD.SetAmmo(currentAmmo, maxAmmo);
    }


    private Vector3 ShootRay()
    {
        if (cameraTransform == null)
            return transform.position + transform.forward * fireRange;

        Vector3 fireDirection = cameraTransform.forward;

        if (ultimateSkill != null && ultimateSkill.IsActive)
        {
            fireDirection = GetUltimateFireDirection();
        }

        Ray ray = new Ray(cameraTransform.position, fireDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, fireRange, fireHitMask))
        {
            BotController bot = hit.collider.GetComponentInParent<BotController>();

            if (bot != null)
            {
                if (bot.IsAlly())
                    return hit.point;

                bot.TakeDamage((int)fireDamage);

                if (ultimateGauge != null)
                    ultimateGauge.AddGauge(1f);
            }
            else
            {
                SpawnImpactDecal(hit);
            }

            return hit.point;
        }

        return ray.origin + ray.direction * fireRange;
    }

    private void SpawnImpactDecal(RaycastHit hit)
    {
        Vector3 spawnPosition = hit.point + hit.normal * impactOffset;
        Quaternion spawnRotation = Quaternion.LookRotation(-hit.normal);

        GameController.instance
            .GetManager<DecalPool>()
            .Spawn(spawnPosition, spawnRotation, hit.collider.transform);
    }

    private Vector3 GetUltimateFireDirection()
    {
        BotController target = FindClosestTargetToCenter();

        if (target == null)
            return cameraTransform.forward;

        Vector3 targetPoint = GetTargetPoint(target);

        return (targetPoint - cameraTransform.position).normalized;
    }

    private BotController FindClosestTargetToCenter()
    {
        Camera cam = cameraTransform.GetComponent<Camera>();

        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return null;

        Collider[] hits = Physics.OverlapSphere(
            cameraTransform.position,
            fireRange,
            targetMask
        );

        BotController bestTarget = null;
        float bestScreenDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            BotController bot = hit.GetComponentInParent<BotController>();

            if (bot == null)
                continue;

            Vector3 targetPoint = GetTargetPoint(bot);
            Vector3 viewport = cam.WorldToViewportPoint(targetPoint);

            if (viewport.z <= 0f)
                continue;

            Vector2 center = new Vector2(0.5f, 0.5f);
            Vector2 targetScreenPos = new Vector2(viewport.x, viewport.y);

            float screenDistance = Vector2.Distance(center, targetScreenPos);

            if (screenDistance > autoAimRadius)
                continue;

            if (screenDistance < bestScreenDistance)
            {
                bestScreenDistance = screenDistance;
                bestTarget = bot;
            }
        }

        return bestTarget;
    }

    private Vector3 GetTargetPoint(BotController bot)
    {
        Collider col = bot.GetComponentInChildren<Collider>();

        if (col != null)
            return col.bounds.center;

        return bot.transform.position + Vector3.up;
    }
}