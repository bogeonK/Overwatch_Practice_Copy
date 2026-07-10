using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private Rigidbody[] parts;
    [SerializeField] private float explosionForce = 6f;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float torqueForce = 8f;

    private bool _exploded;

    private Vector3[] originalLocalPositions;
    private Quaternion[] originalLocalRotations;
    private Transform[] originalParents;

    private void Awake()
    {
        originalLocalPositions = new Vector3[parts.Length];
        originalLocalRotations = new Quaternion[parts.Length];
        originalParents = new Transform[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            Rigidbody part = parts[i];
            if (part == null) continue;

            originalLocalPositions[i] = part.transform.localPosition;
            originalLocalRotations[i] = part.transform.localRotation;
            originalParents[i] = part.transform.parent;

            part.isKinematic = true;
            part.useGravity = false;
        }
    }

    public void Die()
    {
        if (_exploded) return;
        _exploded = true;

        foreach (var part in parts)
        {
            if (part == null) continue;

            part.transform.SetParent(null);

            part.isKinematic = false;
            part.useGravity = true;

            Vector3 randomDir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            part.AddForce(randomDir * explosionForce + Vector3.up * upwardForce, ForceMode.Impulse);
            part.AddTorque(Random.insideUnitSphere * torqueForce, ForceMode.Impulse);
        }
    }

    public void Respawn()
    {
        _exploded = false;

        for (int i = 0; i < parts.Length; i++)
        {
            Rigidbody part = parts[i];
            if (part == null) continue;

            part.linearVelocity = Vector3.zero;
            part.angularVelocity = Vector3.zero;

            part.isKinematic = true;
            part.useGravity = false;

            part.transform.SetParent(originalParents[i]);
            part.transform.localPosition = originalLocalPositions[i];
            part.transform.localRotation = originalLocalRotations[i];

            part.gameObject.SetActive(true);
        }
    }
}