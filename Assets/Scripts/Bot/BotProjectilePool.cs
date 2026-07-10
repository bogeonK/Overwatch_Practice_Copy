using System.Collections.Generic;
using UnityEngine;

public class BotProjectilePool : MonoBehaviour
{
    [SerializeField] private BotProjectile projectilePrefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<BotProjectile> _pool = new Queue<BotProjectile>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewProjectile();
        }
    }

    private BotProjectile CreateNewProjectile()
    {
        BotProjectile proj = Instantiate(projectilePrefab, transform);
        proj.gameObject.SetActive(false);
        proj.SetPool(this);
        _pool.Enqueue(proj);
        return proj;
    }

    public void Spawn(Vector3 position, Quaternion rotation)
    {
        if (_pool.Count == 0)
        {
            CreateNewProjectile();
        }

        BotProjectile proj = _pool.Dequeue();
        proj.transform.position = position;
        proj.transform.rotation = rotation;
        proj.gameObject.SetActive(true);
        proj.Launch();
    }

    public void ReturnToPool(BotProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _pool.Enqueue(projectile);
    }
}