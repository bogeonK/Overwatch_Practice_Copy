using UnityEngine;

public enum HealPackType
{
    Small,
    Big
}

public class HealPack : MonoBehaviour
{
    [SerializeField] private float healAmount = 50f;

    [SerializeField]
    float rotationSpeedX, rotationSpeedY, rotationSpeedZ;

    private HealPackSpawner spawner;

    public void Init(HealPackSpawner spawner)
    {
        this.spawner = spawner;
    }

    void Update()
    {
        transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        IHealable healable = other.GetComponent<IHealable>();

        if (healable == null)
            healable = other.GetComponentInParent<IHealable>();

        if (healable == null)
            return;

        healable.Heal(healAmount);

        if (spawner != null)
            spawner.OnPickedUp(gameObject);
    }
}