using UnityEngine;

public class ViewModelRecoil : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform recoilTarget;

    [Header("Normal Recoil")]
    [SerializeField] private Vector3 recoilPosition = new Vector3(0f, 0f, -0.06f);
    [SerializeField] private Vector3 recoilRotation = new Vector3(-4f, 1.2f, 0f);

    [Header("Heavy Recoil")]
    [SerializeField] private Vector3 heavyRecoilPosition = new Vector3(0f, -0.04f, -0.18f);
    [SerializeField] private Vector3 heavyRecoilRotation = new Vector3(-12f, 2f, 0f);

    [Header("Return Settings")]
    [SerializeField] private float returnSpeed = 18f;
    [SerializeField] private float snappiness = 25f;

    [Header("Limit")]
    [SerializeField] private Vector3 maxPositionOffset = new Vector3(0.12f, 0.12f, 0.25f);
    [SerializeField] private Vector3 maxRotationOffset = new Vector3(20f, 10f, 10f);

    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;

    private Vector3 targetPositionOffset;
    private Vector3 currentPositionOffset;

    private Vector3 targetRotationOffset;
    private Vector3 currentRotationOffset;

    private void Awake()
    {
        if (recoilTarget == null)
        {
            recoilTarget = transform;
        }

        originLocalPosition = recoilTarget.localPosition;
        originLocalRotation = recoilTarget.localRotation;
    }

    private void LateUpdate()
    {
        UpdateRecoil();
    }

    public void PlayRecoil()
    {
        AddRecoil(recoilPosition, recoilRotation);
    }

    public void PlayHeavyRecoil()
    {
        AddRecoil(heavyRecoilPosition, heavyRecoilRotation);
    }

    private void AddRecoil(Vector3 position, Vector3 rotation)
    {
        targetPositionOffset += position;
        targetRotationOffset += rotation;

        targetPositionOffset = ClampVector3(
            targetPositionOffset,
            -maxPositionOffset,
            maxPositionOffset
        );

        targetRotationOffset = ClampVector3(
            targetRotationOffset,
            -maxRotationOffset,
            maxRotationOffset
        );
    }

    private void UpdateRecoil()
    {
        targetPositionOffset = Vector3.Lerp(
            targetPositionOffset,
            Vector3.zero,
            returnSpeed * Time.deltaTime
        );

        currentPositionOffset = Vector3.Lerp(
            currentPositionOffset,
            targetPositionOffset,
            snappiness * Time.deltaTime
        );

        targetRotationOffset = Vector3.Lerp(
            targetRotationOffset,
            Vector3.zero,
            returnSpeed * Time.deltaTime
        );

        currentRotationOffset = Vector3.Lerp(
            currentRotationOffset,
            targetRotationOffset,
            snappiness * Time.deltaTime
        );

        recoilTarget.localPosition = originLocalPosition + currentPositionOffset;

        recoilTarget.localRotation =
            originLocalRotation *
            Quaternion.Euler(currentRotationOffset);
    }

    private Vector3 ClampVector3(Vector3 value, Vector3 min, Vector3 max)
    {
        value.x = Mathf.Clamp(value.x, min.x, max.x);
        value.y = Mathf.Clamp(value.y, min.y, max.y);
        value.z = Mathf.Clamp(value.z, min.z, max.z);

        return value;
    }
}