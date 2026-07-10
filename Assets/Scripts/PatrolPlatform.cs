using UnityEngine;

public class PatrolPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2f;

    private Transform _currentTarget;

    private void Start()
    {
        transform.position = pointA.position;
        _currentTarget = pointB;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _currentTarget.position) < 0.05f)
        {
            _currentTarget = _currentTarget == pointA ? pointB : pointA;
        }
    }
}