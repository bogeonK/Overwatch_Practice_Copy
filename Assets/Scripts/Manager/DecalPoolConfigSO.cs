using UnityEngine;

[CreateAssetMenu(fileName = "DecalPoolConfigSO", menuName = "Config/DecalPoolConfigSO")]
public class DecalPoolConfigSO : BaseScriptableObject
{
    public GameObject decalPrefab;
    public int poolSize = 50;
}