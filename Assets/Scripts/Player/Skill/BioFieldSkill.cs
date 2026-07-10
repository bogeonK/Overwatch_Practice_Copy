using UnityEngine;

public class BioFieldSkill : MonoBehaviour, ISkill
{
    [Header("Prefab")]
    [SerializeField] private GameObject bioFieldPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform player;

    [Header("Cooldown")]
    [SerializeField] private float cooldown = 15f;

    private float cooldownTimer;

    public float CurrentCooldown => cooldownTimer;
    public float MaxCooldown => cooldown;

    private void Awake()
    {
        if (player == null)
        {
            player = transform;
        }
    }

    public void Tick(float deltaTime)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;

            if (cooldownTimer < 0f)
            {
                cooldownTimer = 0f;
            }
        }
    }

    public bool CanUse()
    {
        return cooldownTimer <= 0f;
    }

    public void Use()
    {
        if (!CanUse())
            return;

        if (bioFieldPrefab == null)
        {
            Debug.LogWarning("BioField Prefab이 연결되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = player.position;

        Instantiate(
            bioFieldPrefab,
            spawnPosition,
            Quaternion.identity
        );

        cooldownTimer = cooldown;
    }

    public float GetCooldownRatio()
    {
        if (cooldown <= 0f)
            return 0f;

        return cooldownTimer / cooldown;
    }
}