using System.Collections.Generic;
using UnityEngine;

public class DecalPool : baseManager
{
    private DecalPoolConfigSO config;
    private Queue<DecalFade> pool = new Queue<DecalFade>();
    private Transform poolParent;

    public DecalPool(DecalPoolConfigSO config)
    {
        this.config = config;
    }

    public override void Init()
    {
        GameObject parentObj = new GameObject("DecalPool");
        Object.DontDestroyOnLoad(parentObj);
        poolParent = parentObj.transform;

        for (int i = 0; i < config.poolSize; i++)
        {
            CreateDecal();
        }
    }

    private DecalFade CreateDecal()
    {
        GameObject obj = Object.Instantiate(config.decalPrefab, poolParent);
        obj.SetActive(false);

        DecalFade decal = obj.GetComponent<DecalFade>();
        decal.SetPool(this);

        pool.Enqueue(decal);
        return decal;
    }

    public DecalFade Spawn(Vector3 position, Quaternion rotation, Transform parent)
    {
        DecalFade decal = pool.Count > 0 ? pool.Dequeue() : CreateDecal();

        decal.transform.SetParent(parent);
        decal.transform.SetPositionAndRotation(position, rotation);
        decal.gameObject.SetActive(true);

        return decal;
    }

    public void Return(DecalFade decal)
    {
        decal.gameObject.SetActive(false);
        decal.transform.SetParent(poolParent);
        pool.Enqueue(decal);
    }

    public override void Update() { }

    public override void Destory() { }
}