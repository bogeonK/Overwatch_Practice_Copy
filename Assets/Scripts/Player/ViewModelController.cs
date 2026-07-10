using UnityEngine;

public class ViewModelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ViewModelBob bob;
    [SerializeField] private ViewModelRecoil recoil;
    [SerializeField] private ViewModelDash dash;

    [Header("Fire Lock")]
    [SerializeField] private float fireBlockDuration = 0.18f;

    private float fireBlockTimer;

    private void Update()
    {
        if (player == null)
            return;

        if (fireBlockTimer > 0f)
            fireBlockTimer -= Time.deltaTime;

        bool isMoving = player.HasMoveInput();
        bool isGrounded = player.IsGrounded();
        bool dashInput = player.IsDashPressed();

        bool isFireBlocking = fireBlockTimer > 0f || player.IsFirePressed();

        bool dashPose = dashInput && isMoving && !isFireBlocking;
        bool dashBob = dashPose && isGrounded;

        if (dash != null)
            dash.SetDash(dashPose, dashBob);

        bool canWalkBob =
            isMoving &&
            isGrounded &&
            !dashPose &&
            !isFireBlocking;

        if (bob != null)
            bob.SetActiveBob(canWalkBob);
    }

    public void PlayFire()
    {
        fireBlockTimer = fireBlockDuration;

        if (bob != null)
            bob.ResetBobImmediate();

        if (dash != null)
            dash.ResetDashImmediate();

        if (recoil != null)
            recoil.PlayRecoil();
    }
}