using UnityEngine;

public class ViewModelDash : MonoBehaviour
{
    [Header("Dash Pose")]
    [SerializeField] private Vector3 dashLocalRotation = new Vector3(-74.85f, -61.4f, 0f);

    [Header("Dash Bob")]
    [SerializeField] private float dashBobSpeed = 16f;
    [SerializeField] private float dashBobAmount = 0.04f;

    [Header("Smooth")]
    [SerializeField] private float poseSpeed = 18f;
    [SerializeField] private float bobSpeed = 14f;
    [SerializeField] private float returnSpeed = 20f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private bool dashPoseActive;
    private bool dashBobActive;
    private float timer;

    private void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    private void Update()
    {
        Quaternion targetRot = dashPoseActive
            ? Quaternion.Euler(dashLocalRotation)
            : initialLocalRot;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRot,
            Time.deltaTime * poseSpeed
        );

        if (dashPoseActive && dashBobActive)
        {
            timer += Time.deltaTime * dashBobSpeed;

            float yOffset = Mathf.Sin(timer) * dashBobAmount;

            Vector3 targetPos = initialLocalPos + new Vector3(0f, yOffset, 0f);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPos,
                Time.deltaTime * bobSpeed
            );
        }
        else
        {
            timer = 0f;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                initialLocalPos,
                Time.deltaTime * returnSpeed
            );
        }
    }

    public void SetDash(bool poseActive, bool bobActive)
    {
        dashPoseActive = poseActive;
        dashBobActive = bobActive;
    }

    public void ResetDashImmediate()
    {
        dashPoseActive = false;
        dashBobActive = false;
        timer = 0f;
        transform.localPosition = initialLocalPos;
        transform.localRotation = initialLocalRot;
    }
}