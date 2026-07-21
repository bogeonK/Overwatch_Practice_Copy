using UnityEngine;
using Unity.Cinemachine;

public class FallDeathCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineCamera firstPersonCamera;
    [SerializeField] private CinemachineCamera fallCamera;
    [SerializeField] private Transform fallLookTarget;

    [Header("Priority")]
    [SerializeField] private int firstPersonPriority = 20;
    [SerializeField] private int fallInactivePriority = 0;
    [SerializeField] private int fallActivePriority = 30;

    private bool isActive;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (firstPersonCamera != null)
            firstPersonCamera.Priority = firstPersonPriority;

        if (fallCamera != null)
            fallCamera.Priority = fallInactivePriority;
    }

    public void Activate(Transform fallTarget)
    {
        if (isActive || fallCamera == null || mainCamera == null)
            return;

        isActive = true;
        fallCamera.transform.SetPositionAndRotation(
            mainCamera.transform.position,
            mainCamera.transform.rotation
        );

        Transform target =
            fallLookTarget != null
                ? fallLookTarget
                : fallTarget;

        fallCamera.Target.TrackingTarget = target;
        fallCamera.Priority = fallActivePriority;

        if (firstPersonCamera != null)
            firstPersonCamera.Priority = firstPersonPriority;
    }

    public void ResetCamera()
    {
        isActive = false;

        if (fallCamera != null)
        {
            fallCamera.Priority = fallInactivePriority;
            fallCamera.Target.TrackingTarget = null;
        }

        // 원래 1인칭 카메라 복귀
        if (firstPersonCamera != null)
            firstPersonCamera.Priority = firstPersonPriority;
    }
}