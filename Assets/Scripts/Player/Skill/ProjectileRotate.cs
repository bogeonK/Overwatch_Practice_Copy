using UnityEngine;

public class ProjectileRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotateAxis = Vector3.forward;
    [SerializeField] private float rotateSpeed = 720f;

    private void Update()
    {
        transform.Rotate(
            rotateAxis.normalized,
            rotateSpeed * Time.deltaTime,
            Space.Self
        );
    }
}