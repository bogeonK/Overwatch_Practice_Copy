using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/SoundManagerConfig")]
public class SoundManagerConfigSO : BaseScriptableObject
{
    public List<SoundData> sounds = new List<SoundData>();
}