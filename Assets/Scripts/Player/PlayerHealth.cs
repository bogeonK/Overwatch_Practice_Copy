using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealable
{
    [Header("Health")]
    [SerializeField] private float maxHp = 200f;
    [SerializeField] private float hp = 200f;

    [Header("UI")]
    [SerializeField] private PlayerHUD playerHUD;

    private void Start()
    {
        hp = maxHp;
        UpdateHpUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(12f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsDead())
            return;

        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        UpdateHpUI();

        if (IsDead())
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead())
            return;

        hp += amount;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        UpdateHpUI();
    }

    public bool IsDead()
    {
        return hp <= 0f;
    }

    private void UpdateHpUI()
    {
        if (playerHUD != null)
        {
            playerHUD.SetHp(Mathf.RoundToInt(hp));
        }
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
    }
}