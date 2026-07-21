using UnityEngine;

public class CharacterSelectionManager : baseManager
{
    private readonly CharacterSelectionManagerConfigSO config;

    private CharacterSelectionUI selectionUI;
    private GameObject currentPlayer;
    private GameObject previewObject;
    private int selectedCharacterIndex = -1;
    private bool characterSelected;

    public CharacterSelectionManager(
        CharacterSelectionManagerConfigSO config)
    {
        this.config = config;
    }

    public override void Init()
    {
        characterSelected = false;
        selectedCharacterIndex = -1;

        currentPlayer = null;
        previewObject = null;
    }

    public override void Update()
    {
    }

    public override void Destory()
    {
        selectionUI = null;
        currentPlayer = null;
    }

    public void BindUI(CharacterSelectionUI ui)
    {
        selectionUI = ui;
        characterSelected = false;

        selectionUI.BuildCharacterButtons(config.characters);
        selectionUI.OpenSelection();
    }

    public void SelectCharacter(int characterIndex)
    {
        if (characterSelected)
            return;

        if (selectionUI == null)
            return;

        if (characterIndex < 0 ||
            characterIndex >= config.characters.Count)
        {
            return;
        }

        CharacterData character =
            config.characters[characterIndex];

        selectedCharacterIndex = characterIndex;

        ShowPreview(character);

        selectionUI.ShowSelectedCharacter(
            character.characterName
        );
    }

    private void ShowPreview(CharacterData character)
    {
        if (previewObject != null)
        {
            Object.Destroy(previewObject);
            previewObject = null;
        }

        if (character.previewPrefab == null)
        {
            Debug.LogWarning(
                $"[CharacterSelectionManager] " +
                $"{character.characterName}의 Preview Prefab이 없습니다."
            );

            return;
        }

        Transform previewPoint =
            selectionUI.PreviewSpawnPoint;

        previewObject = Object.Instantiate(
            character.previewPrefab,
            previewPoint.position,
            previewPoint.rotation
        );
    }

    private void SpawnCharacter(GameObject playerPrefab)
    {
        Transform spawnPoint = selectionUI.PlayerSpawnPoint;

        currentPlayer = Object.Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        ConnectPlayer(currentPlayer);

        characterSelected = true;
        selectionUI.CloseSelection();
    }

    private void ConnectPlayer(GameObject player)
    {
        Camera gameplayCamera = selectionUI.GameplayCamera;

        PlayerHUD playerHUD = selectionUI.PlayerHUD;

        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.SetCameraTransform(
                gameplayCamera.transform
            );
        }

        RifleAttack rifleAttack = player.GetComponentInChildren<RifleAttack>(true);

        if (rifleAttack != null)
        {
            rifleAttack.SetCameraTransform(
                gameplayCamera.transform
            );

            rifleAttack.SetPlayerHUD(playerHUD);
        }

        MissileSkill missileSkill = player.GetComponentInChildren<MissileSkill>(true);

        if (missileSkill != null)
        {
            missileSkill.SetPlayerCamera(gameplayCamera);
        }

        UltimateGauge ultimateGauge = player.GetComponentInChildren<UltimateGauge>(true);

        if (ultimateGauge != null)
        {
            ultimateGauge.SetPlayerHUD(playerHUD);
        }

        PlayerHealth playerHealth = player.GetComponentInChildren<PlayerHealth>(true);

        if (playerHealth != null)
        {
            playerHealth.SetPlayerHUD(playerHUD);
        }

        FPSCameraLook fpsCameraLook = player.GetComponentInChildren<FPSCameraLook>(true);

        if (fpsCameraLook != null &&
            fpsCameraLook.CameraTarget != null &&
            selectionUI.FirstPersonCamera != null)
        {
            selectionUI.FirstPersonCamera
                .Target.TrackingTarget =
                fpsCameraLook.CameraTarget;
        }

        PlayerSkillManager playerSkillManager = player.GetComponentInChildren<PlayerSkillManager>(true);

        if (playerSkillManager != null)
        {
            controller
                .GetManager<SkillManager>()
                .SetPlayerSkillManager(playerSkillManager);
        }

        PlayerFallDeath playerFallDeath = player.GetComponentInChildren<PlayerFallDeath>(true);

        if (playerFallDeath != null)
        {
            playerFallDeath.SetFallDeathCamera(
                selectionUI.FallDeathCameraController
            );
        }

        controller.playerTransform = player.transform;
    }

    public void ConfirmCharacter()
    {
        if (characterSelected)
            return;

        if (selectedCharacterIndex < 0 ||
            selectedCharacterIndex >= config.characters.Count)
        {
            return;
        }

        CharacterData character =
            config.characters[selectedCharacterIndex];

        if (character.playerPrefab == null)
        {
            Debug.LogError(
                $"[CharacterSelectionManager] " +
                $"{character.characterName}의 Player Prefab이 없습니다."
            );

            return;
        }

        if (previewObject != null)
        {
            Object.Destroy(previewObject);
            previewObject = null;
        }

        SpawnCharacter(character.playerPrefab);
    }
}