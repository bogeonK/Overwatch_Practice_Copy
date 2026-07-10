using System.Collections;
using UnityEngine;

public class UltimateSkill : MonoBehaviour, ISkill
{
    [SerializeField] private UltimateGauge ultimateGauge;
    [SerializeField] private float duration = 5f;

    [SerializeField] private UltimateUI ultimateUI;
    [SerializeField] private UltimateUIExpand ultimateUIExpand;

    private bool isActive;
    private Coroutine routine;

    public bool IsActive => isActive;

    public float CurrentCooldown => 0f;
    public float MaxCooldown => 0f;

    public void Tick(float deltaTime) { }

    public bool CanUse()
    {
        return ultimateGauge != null && ultimateGauge.IsFull() && !isActive;
    }

    public void Use()
    {
        if (!CanUse())
            return;

        ultimateGauge.Consume();

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(UltimateRoutine());
    }

    private IEnumerator UltimateRoutine()
    {
        isActive = true;
        GameController.instance
    .GetManager<SoundManager>()
    .PlaySFX(SoundId.UltimateStart);
        if (ultimateUI != null)
            ultimateUI.Show();

        if (ultimateUIExpand != null)
            ultimateUIExpand.Play();

        Debug.Log("궁극기 활성화");

        yield return new WaitForSeconds(duration);

        isActive = false;

        if (ultimateUI != null)
            ultimateUI.Hide();

        if (ultimateUIExpand != null)
            ultimateUIExpand.Hide();

        Debug.Log("궁극기 종료");
    }
}