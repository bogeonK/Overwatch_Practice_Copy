using System.Collections;
using UnityEngine;

public class HealPackSpawner : MonoBehaviour
{
    [SerializeField] private HealPack healPackPrefab;
    [SerializeField] private float respawnTime = 10f;

    private HealPack currentHealPack;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        currentHealPack = Instantiate(
            healPackPrefab,
            transform.position,
            transform.rotation
        );

        currentHealPack.Init(this);
    }

    public void OnPickedUp(GameObject pickedHealPack)
    {
        Destroy(pickedHealPack);
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        Spawn();
    }
}