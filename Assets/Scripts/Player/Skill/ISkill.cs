public interface ISkill
{
    bool CanUse();

    void Use();

    void Tick(float deltaTime);

    float CurrentCooldown { get; }

    float MaxCooldown { get; }
}