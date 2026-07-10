using System.Collections;
using UnityEngine;

public class WeaponVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem coreFlash;
    [SerializeField] private ParticleSystem blueOuterFlash;
    [SerializeField] private ParticleSystem sparks;

    [Header("Tracer")]
    [SerializeField] private LineRenderer[] tracerSegments;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float tracerTime = 0.03f;
    [SerializeField] private float segmentLength = 0.25f;

    private Coroutine tracerCoroutine;

    public void PlayFireEffect(Vector3 endPoint)
    {
        PlayParticle(coreFlash);
        PlayParticle(blueOuterFlash);
        PlayParticle(sparks);
        PlayTracer(endPoint);
    }

    private void PlayParticle(ParticleSystem particle)
    {
        if (particle == null) return;

        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
    }

    private void PlayTracer(Vector3 endPoint)
    {
        if (tracerSegments == null || tracerSegments.Length == 0 || muzzlePoint == null)
            return;

        if (tracerCoroutine != null)
            StopCoroutine(tracerCoroutine);

        tracerCoroutine = StartCoroutine(TracerRoutine(endPoint));
    }

    private IEnumerator TracerRoutine(Vector3 endPoint)
    {
        Vector3 startPoint = muzzlePoint.position;
        Vector3 direction = (endPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, endPoint);

        foreach (LineRenderer line in tracerSegments)
        {
            float randomStart = Random.Range(0.1f, 0.9f) * distance;
            float length = Random.Range(segmentLength * 0.5f, segmentLength * 1.5f);

            Vector3 p1 = startPoint + direction * randomStart;
            Vector3 p2 = startPoint + direction * Mathf.Min(randomStart + length, distance);

            line.positionCount = 2;
            line.SetPosition(0, p1);
            line.SetPosition(1, p2);
            line.enabled = true;
        }

        yield return new WaitForSeconds(tracerTime);

        foreach (LineRenderer line in tracerSegments)
        {
            line.enabled = false;
        }
    }
}