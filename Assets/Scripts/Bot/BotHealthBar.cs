using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BotHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject root;

    [Header("HP Cells")]
    [SerializeField] private Image[] hpCells;
    [SerializeField] private Color activeColor = new Color(0.7f, 0f, 0f, 1f);
    [SerializeField] private Color damagedColor = new Color(0.7f, 0f, 0f, 0.25f);

    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);
    [SerializeField] private float visibleTime = 5f;

    private Coroutine _hideRoutine;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;

        if (root != null)
            root.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;

        if (_mainCamera != null)
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Show(int currentHp, int maxHp)
    {
        if (root == null || hpCells == null || hpCells.Length == 0)
            return;

        UpdateCells(currentHp, maxHp);

        root.SetActive(true);

        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    public void HideImmediately()
    {
        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }

        if (root != null)
            root.SetActive(false);
    }

    private void UpdateCells(int currentHp, int maxHp)
    {
        int cellCount = hpCells.Length;

        float hpPerCell = (float)maxHp / cellCount;

        int activeCellCount = Mathf.CeilToInt(currentHp / hpPerCell);

        for (int i = 0; i < cellCount; i++)
        {
            if (hpCells[i] == null)
                continue;

            if (i < activeCellCount)
            {
                hpCells[i].color = activeColor;
            }
            else
            {
                hpCells[i].color = damagedColor;
            }
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(visibleTime);

        if (root != null)
            root.SetActive(false);

        _hideRoutine = null;
    }
}
