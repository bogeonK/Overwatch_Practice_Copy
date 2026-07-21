using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("First Person Camera")]
    [SerializeField] private CinemachineCamera firstPersonCamera;

    [Header("UI")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject gameplayHUD;

    [Header("Character Buttons")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button buttonPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Camera gameplayCamera;

    [Header("Gameplay UI References")]
    [SerializeField] private PlayerHUD playerHUD;

    [Header("Fall Death")]
    [SerializeField] private FallDeathCamera fallDeathCameraController;

    [Header("Preview")]
    [SerializeField] private Transform previewSpawnPoint;
    [SerializeField] private CinemachineCamera selectionCamera;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text selectedHeroNameText;

    public FallDeathCamera FallDeathCameraController =>
        fallDeathCameraController;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public Camera GameplayCamera => gameplayCamera;
    public CinemachineCamera FirstPersonCamera =>
    firstPersonCamera;
    public Transform PreviewSpawnPoint =>
    previewSpawnPoint;

    public PlayerHUD PlayerHUD => playerHUD;

    private void Start()
    {
        if (GameController.instance == null)
        {
            return;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(
                OnClickConfirm
            );

            confirmButton.onClick.AddListener(
                OnClickConfirm
            );

            confirmButton.interactable = false;
        }


        GameController.instance
            .GetManager<CharacterSelectionManager>()
            .BindUI(this);
    }

    private void OnClickConfirm()
    {
        GameController.instance
            .GetManager<CharacterSelectionManager>()
            .ConfirmCharacter();
    }

    public void BuildCharacterButtons(
        List<CharacterData> characters)
    {
        for (int i = buttonContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < characters.Count; i++)
        {
            int characterIndex = i;
            CharacterData character = characters[i];

            Button button = Instantiate(
                buttonPrefab,
                buttonContainer
            );

            Image portrait = button.transform
                .Find("Portrait")
                ?.GetComponent<Image>();

            if (portrait != null)
                portrait.sprite = character.portrait;

            TMP_Text nameText = button.GetComponentInChildren<TMP_Text>();

            if (nameText != null)
                nameText.text = character.characterName;

            button.onClick.AddListener(
                () => SelectCharacter(characterIndex)
            );
        }
    }

    private void SelectCharacter(int characterIndex)
    {
        GameController.instance
            .GetManager<CharacterSelectionManager>()
            .SelectCharacter(characterIndex);
    }

    public void OpenSelection()
    {
        selectionPanel.SetActive(true);

        if (gameplayHUD != null)
            gameplayHUD.SetActive(false);

        if (selectionCamera != null)
            selectionCamera.Priority = 40;

        ClearSelectedCharacter();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSelection()
    {
        selectionPanel.SetActive(false);

        if (gameplayHUD != null)
            gameplayHUD.SetActive(true);

        if (selectionCamera != null)
            selectionCamera.Priority = 0;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowSelectedCharacter(string characterName)
    {
        if (selectedHeroNameText != null)
            selectedHeroNameText.text = characterName;

        if (confirmButton != null)
            confirmButton.interactable = true;
    }

    public void ClearSelectedCharacter()
    {
        if (selectedHeroNameText != null)
            selectedHeroNameText.text = "영웅을 선택하세요";

        if (confirmButton != null)
            confirmButton.interactable = false;
    }
}