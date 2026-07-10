using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SkillData")]
public class SkillDataSO : ScriptableObject
{
    public string skillName;

    public float cooldown;

    public Sprite icon;
}