using UnityEngine;

public class Live2DTouchPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string touchTriggerName = "Touch";

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryTouch(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryTouch(Input.GetTouch(0).position);
        }
    }

    private void TryTouch(Vector2 screenPosition)
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Main Camera가 없습니다.");
            return;
        }

        Vector2 worldPoint = cam.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);

        if (hit == null)
            return;

        if (hit.transform == transform || hit.transform.IsChildOf(transform))
        {
            PlayTouchAnimation();
        }
    }

    private void PlayTouchAnimation()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator가 연결되지 않았습니다.");
            return;
        }

        animator.ResetTrigger(touchTriggerName);
        animator.SetTrigger(touchTriggerName);
    }
}