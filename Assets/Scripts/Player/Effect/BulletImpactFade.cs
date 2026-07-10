using UnityEngine;

public class BulletImpactFade : MonoBehaviour
{
    [SerializeField] private float stayTime = 5f;
    [SerializeField] private float fadeTime = 1f;

    private ParticleSystem particle;
    private ParticleSystemRenderer particleRenderer;
    private Material material;
    private Color baseColor;

    private float timer;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        material = particleRenderer.material;
        baseColor = material.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < stayTime)
            return;

        float fadeTimer = timer - stayTime;
        float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeTime);

        Color color = baseColor;
        color.a = alpha;

        material.color = color;

        if (timer >= stayTime + fadeTime)
        {
            Destroy(gameObject);
        }
    }
}