using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFade : MonoBehaviour
{
    [SerializeField] private float stayTime = 5f;
    [SerializeField] private float fadeTime = 1f;

    private DecalProjector decal;
    private DecalPool pool;
    private float timer;

    private void Awake()
    {
        decal = GetComponent<DecalProjector>();
    }

    public void SetPool(DecalPool pool)
    {
        this.pool = pool;
    }

    private void OnEnable()
    {
        timer = 0f;

        if (decal != null)
            decal.fadeFactor = 1f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < stayTime)
            return;

        float fadeT = Mathf.Clamp01((timer - stayTime) / fadeTime);

        if (decal != null)
            decal.fadeFactor = Mathf.Lerp(1f, 0f, fadeT);

        if (fadeT >= 1f)
        {
            if (pool != null)
                pool.Return(this);
            else
                gameObject.SetActive(false);
        }
    }
}