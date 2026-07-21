using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterSelectionManagerConfigSO",
    menuName = "Config/CharacterSelectionManagerConfigSO"
)]
public class CharacterSelectionManagerConfigSO : BaseScriptableObject
{
    public List<CharacterData> characters = new();
}

[Serializable]
public class CharacterData
{
    public string characterName;
    public Sprite portrait;

    [Header("Prefabs")]
    public GameObject previewPrefab;
    public GameObject playerPrefab;
}