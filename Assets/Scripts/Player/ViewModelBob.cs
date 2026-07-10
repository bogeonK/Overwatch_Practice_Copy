using UnityEngine;

public class ViewModelBob : MonoBehaviour
{
    [Header("Bob Settings")]
    [SerializeField] private float bobSpeed = 8f;
    [SerializeField] private float bobAmount = 0.03f;
    [SerializeField] private float returnSpeed = 15f;

    private Vector3 initialLocalPos;
    private float timer;
    private bool activeBob;

    private void Start()
    {
        initialLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (activeBob)
        {
            timer += Time.deltaTime * bobSpeed;

            float yOffset = Mathf.Sin(timer) * bobAmount;
            Vector3 targetPos = initialLocalPos + new Vector3(0f, yOffset, 0f);

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPos,
                Time.deltaTime * 10f
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

    public void SetActiveBob(bool value)
    {
        activeBob = value;
    }

    public void ResetBobImmediate()
    {
        activeBob = false;
        timer = 0f;
        transform.localPosition = initialLocalPos;
    }
}