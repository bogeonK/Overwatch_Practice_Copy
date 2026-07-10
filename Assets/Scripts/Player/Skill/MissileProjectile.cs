using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 35f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 30f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifeTime = 2f;

    private UltimateGauge ultimateGauge;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position += transform.forward * speed * Time.deltaTime;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BotController bot = other.GetComponentInParent<BotController>();

        if (bot != null)
        {
            bot.TakeDamage(Mathf.RoundToInt(damage));

            if (ultimateGauge != null)
                ultimateGauge.AddGauge(damage);

            Explode();
            return;
        }

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null && !damageable.IsDead())
        {
            damageable.TakeDamage(damage);
            Explode();
            return;
        }

        Explode();
    }

    public void SetUltimateGauge(UltimateGauge gauge)
    {
        ultimateGauge = gauge;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosion, explosionLifeTime);
        }

        Destroy(gameObject);
    }
}