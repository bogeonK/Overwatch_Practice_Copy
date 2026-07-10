using System.Collections.Generic;
using UnityEngine;

public class BioField : MonoBehaviour
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private float totalHealAmount = 100f;

    private float timer;
    private float healPerSecond;

    private readonly List<IHealable> targets = new List<IHealable>();

    private void Awake()
    {
        healPerSecond = totalHealAmount / duration;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float healAmount = healPerSecond * Time.deltaTime;

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }

            if (!targets[i].IsDead())
            {
                targets[i].Heal(healAmount);
            }
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IHealable target = other.GetComponentInParent<IHealable>();

        if (target != null && !targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IHealable target = other.GetComponentInParent<IHealable>();

        if (target != null)
        {
            targets.Remove(target);
        }
    }
}