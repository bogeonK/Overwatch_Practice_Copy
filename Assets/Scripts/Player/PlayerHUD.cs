using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private Image[] hpFillImages;

    [Header("HP Text")]
    [SerializeField] private TMP_Text hpText;

    [SerializeField] private int maxHp = 200;
    [SerializeField] private int hpPerCell = 20;

    [Header("Ultimate")]
    [SerializeField] private Image ultimateGaugeFillImage;
    [SerializeField] private TMP_Text ultimateGaugeText;

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoText;

    public void SetHp(int currentHp)
    {
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        UpdateHpBar(currentHp);
        UpdateHpText(currentHp);
    }

    private void UpdateHpBar(int currentHp)
    {
        for (int i = 0; i < hpFillImages.Length; i++)
        {
            if (hpFillImages[i] == null)
                continue;

            int cellStartHp = i * hpPerCell;
            int hpInThisCell = currentHp - cellStartHp;

            float fillAmount = Mathf.Clamp01((float)hpInThisCell / hpPerCell);

            hpFillImages[i].fillAmount = fillAmount;
        }
    }

    private void UpdateHpText(int currentHp)
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    public void SetAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }

    public void SetUltimateGauge(float ratio, int percent)
    {
        if (ultimateGaugeFillImage != null)
        {
            ultimateGaugeFillImage.fillAmount = ratio;
        }

        if (ultimateGaugeText != null)
        {
            ultimateGaugeText.text = $"{percent}%";
        }
    }
}