using UnityEngine;

public class BotProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;


    private BotProjectilePool _pool;
    private float _lifeTimer;
    private bool _hasHit;

    public void SetPool(BotProjectilePool pool)
    {
        _pool = pool;
    }

    public void Launch()
    {
        _lifeTimer = lifeTime;
        _hasHit = false;
    }

    private void Update()
    {
        if (_hasHit)
            return;

        transform.position += transform.forward * speed * Time.deltaTime;

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit)
            return;
        if (other.GetComponentInParent<BioField>() != null)
            return;
        BotController hitBot = other.GetComponentInParent<BotController>();

        if (hitBot != null)
        {
            if (hitBot.IsAlly())
            {
                _hasHit = true;
                hitBot.TakeDamage(damage);
                ReturnToPool();
                return;
            }

            _hasHit = true;
            ReturnToPool();
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            _hasHit = true;
            playerHealth.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        _hasHit = true;
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);

        if (_pool != null)
        {
            _pool.ReturnToPool(this);
        }

    }
}