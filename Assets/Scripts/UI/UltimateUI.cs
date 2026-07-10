using System.Collections;
using UnityEngine;

public class UltimateUI : MonoBehaviour
{
    [SerializeField] RectTransform circle;

    [SerializeField]
    float expandTime = 0.2f;

    [SerializeField]
    float maxScale = 1f;

    [SerializeField] private AnimationCurve expandCurve;


    Coroutine routine;

    public void Show()
    {
        gameObject.SetActive(true);

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Expand());
    }

    IEnumerator Expand()
    {
        circle.localScale = Vector3.zero;

        float t = 0;

        while (t < expandTime)
        {
            t += Time.deltaTime;

            float progress = t / expandTime;

            float value =
                expandCurve.Evaluate(progress) * maxScale;

            circle.localScale = Vector3.one * value;

            yield return null;
        }

        circle.localScale = Vector3.one * maxScale;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}