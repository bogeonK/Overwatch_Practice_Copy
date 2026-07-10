using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    [Header("Character Option")]
    [SerializeField] private bool useShiftAsSkill;

    [Header("Skills")]
    [SerializeField] private MonoBehaviour shiftSkill;
    [SerializeField] private MonoBehaviour eSkill;
    [SerializeField] private MonoBehaviour rightClickSkill;
    [SerializeField] private MonoBehaviour ultimateSkill;

    private readonly Dictionary<SkillSlot, ISkill> skills = new();

    public bool UseShiftAsSkill => useShiftAsSkill;

    private void Awake()
    {
        RegisterSkill(SkillSlot.Shift, shiftSkill);
        RegisterSkill(SkillSlot.E, eSkill);
        RegisterSkill(SkillSlot.RightClick, rightClickSkill);
        RegisterSkill(SkillSlot.Ultimate, ultimateSkill);
    }

    public void TickSkills(float deltaTime)
    {
        foreach (ISkill skill in skills.Values)
        {
            skill?.Tick(deltaTime);
        }
    }

    public void UseSkill(SkillSlot slot)
    {
        if (!skills.TryGetValue(slot, out ISkill skill))
            return;

        if (skill == null)
            return;

        if (!skill.CanUse())
            return;

        skill.Use();
    }

    private void RegisterSkill(SkillSlot slot, MonoBehaviour skillBehaviour)
    {
        if (skillBehaviour == null)
            return;

        if (skillBehaviour is not ISkill skill)
        {
            Debug.LogWarning($"{skillBehaviour.name}은 ISkill이 아닙니다.");
            return;
        }

        skills[slot] = skill;
    }
}