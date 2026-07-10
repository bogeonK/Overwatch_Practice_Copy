using UnityEngine;

public class UltimateGauge : MonoBehaviour
{
    [SerializeField] private float currentGauge;
    [SerializeField] private float maxGauge = 100f;
    [SerializeField] private PlayerHUD playerHUD;

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        //궁극기 테스트용
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddGauge(100f);
        }

    }

    public bool IsFull()
    {
        return currentGauge >= maxGauge;
    }

    public void AddGauge(float amount)
    {
        if (amount <= 0f)
            return;

        currentGauge += amount;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);

        UpdateUI();
    }

    public void Consume()
    {
        currentGauge = 0f;
        UpdateUI();
    }

    public float GetGaugeRatio()
    {
        return currentGauge / maxGauge;
    }

    public int GetGaugePercent()
    {
        return Mathf.FloorToInt(GetGaugeRatio() * 100f);
    }

    private void UpdateUI()
    {
        if (playerHUD != null)
        {
            playerHUD.SetUltimateGauge(GetGaugeRatio(), GetGaugePercent());
        }
    }
}