using UnityEngine;

public class SkillManager : baseManager
{
    private SkillManagerConfigSO config;
    private PlayerSkillManager playerSkillManager;

    public SkillManager(SkillManagerConfigSO config)
    {
        this.config = config;
    }

    public override void Init()
    {
        FindPlayerSkillManager();
    }

    public override void Update()
    {
        if (playerSkillManager == null)
        {
            FindPlayerSkillManager();
            return;
        }

        playerSkillManager.TickSkills(Time.deltaTime);

        HandleInput();
    }

    public override void Destory()
    {
        playerSkillManager = null;
    }

    public override void ActiveOff()
    {
    }

    private void FindPlayerSkillManager()
    {
        playerSkillManager =
            Object.FindAnyObjectByType<PlayerSkillManager>();

        if (playerSkillManager == null)
        {
            Debug.LogWarning("[SkillManager] PlayerSkillManager를 찾지 못했습니다.");
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (playerSkillManager.UseShiftAsSkill)
            {
                playerSkillManager.UseSkill(SkillSlot.Shift);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerSkillManager.UseSkill(SkillSlot.E);
        }

        if (Input.GetMouseButtonDown(1))
        {
            playerSkillManager.UseSkill(SkillSlot.RightClick);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerSkillManager.UseSkill(SkillSlot.Ultimate);
        }
    }

    public void SetPlayerSkillManager(PlayerSkillManager manager)
    {
        playerSkillManager = manager;
    }
}