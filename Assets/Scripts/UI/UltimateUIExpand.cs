using System.Collections;
using UnityEngine;

public class UltimateUIExpand : MonoBehaviour
{
    [SerializeField] private RectTransform bracket;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Expand")]
    [SerializeField] private float startWidth = 0f;
    [SerializeField] private float endWidth = 250f;
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private AnimationCurve expandCurve;

    private Coroutine routine;
     
    private void Awake()
    {
        SetWidth(startWidth);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

    }

    public void Play()
    {
        gameObject.SetActive(true);

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayRoutine());
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);

        SetWidth(startWidth);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
    }

    private IEnumerator PlayRoutine()
    {
        float time = 0f;

        SetWidth(startWidth);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);
            float curveT = expandCurve != null ? expandCurve.Evaluate(t) : t;

            float width = Mathf.Lerp(startWidth, endWidth, curveT);
            SetWidth(width);

            if (canvasGroup != null)
                canvasGroup.alpha = t;

            yield return null;
        }

        SetWidth(endWidth);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private void SetWidth(float width)
    {
        if (bracket == null)
            return;

        bracket.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}