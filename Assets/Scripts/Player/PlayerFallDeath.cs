using System.Collections;
using UnityEngine;

public class PlayerFallDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private FallDeathCamera fallDeathCamera;
    [SerializeField] private GameObject viewModelObject;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform fallLookTarget;

    [Header("Fall Death")]
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Player Model")]
    [SerializeField] private GameObject playerModelObject;

    private bool isFallDead;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Coroutine respawnCoroutine;

    private Transform[] playerModelTransforms;
    private int[] originalModelLayers;
    private bool originalModelActive;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // 게임 시작 위치 저장
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        SavePlayerModelState();

        if (playerModelObject != null)
            playerModelObject.SetActive(false);
    }

    private void Update()
    {
        if (isFallDead)
            return;

        if (transform.position.y <= fallDeathY)
        {
            StartFallDeath();
        }
    }

    private void StartFallDeath()
    {
        if (isFallDead)
            return;

        isFallDead = true;

        if (playerController != null)
            playerController.SetControlLocked(true);

        if (viewModelObject != null)
            viewModelObject.SetActive(false);

        // 실제 캐릭터 모델 표시
        if (playerModelObject != null)
            playerModelObject.SetActive(true);

        if (fallDeathCamera != null)
        {
            Transform target =
                fallLookTarget != null
                    ? fallLookTarget
                    : transform;

            fallDeathCamera.Activate(target);
        }

        if (respawnCoroutine != null)
            StopCoroutine(respawnCoroutine);

        respawnCoroutine = StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    private void Respawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 처음 시작 위치로 복귀
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Physics.SyncTransforms();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 카메라를 원래 부모와 위치로 복구
        if (fallDeathCamera != null)
            fallDeathCamera.ResetCamera();

        // 실제 캐릭터 모델 다시 숨김
        if (playerModelObject != null)
            playerModelObject.SetActive(false);


        // 뷰모델 다시 표시
        if (viewModelObject != null)
            viewModelObject.SetActive(true);

        // 플레이어 조작 복구
        if (playerController != null)
            playerController.SetControlLocked(false);

        isFallDead = false;
        respawnCoroutine = null;
    }

    private void SavePlayerModelState()
    {
        if (playerModelObject == null)
            return;

        originalModelActive = playerModelObject.activeSelf;

        playerModelTransforms =
            playerModelObject.GetComponentsInChildren<Transform>(true);

        originalModelLayers = new int[playerModelTransforms.Length];

        for (int i = 0; i < playerModelTransforms.Length; i++)
        {
            originalModelLayers[i] =
                playerModelTransforms[i].gameObject.layer;
        }
    }



    private void RestorePlayerModel()
    {
        if (playerModelObject == null)
            return;

        if (playerModelTransforms != null &&
            originalModelLayers != null)
        {
            int count = Mathf.Min(
                playerModelTransforms.Length,
                originalModelLayers.Length
            );

            for (int i = 0; i < count; i++)
            {
                if (playerModelTransforms[i] != null)
                {
                    playerModelTransforms[i].gameObject.layer =
                        originalModelLayers[i];
                }
            }
        }

        playerModelObject.SetActive(originalModelActive);
    }

    public void SetFallDeathCamera( FallDeathCamera cameraController)
    {
        fallDeathCamera = cameraController;
    }

}