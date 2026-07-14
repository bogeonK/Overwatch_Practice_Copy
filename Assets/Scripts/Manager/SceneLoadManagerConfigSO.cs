using UnityEngine;

[CreateAssetMenu(
    fileName = "SceneLoadManagerConfigSO",
    menuName = "Config/SceneLoadManagerConfigSO"
)]
public class SceneLoadManagerConfigSO : BaseScriptableObject
{
    [Header("Scene Names")]
    public string lobbySceneName = "LobbyScene";
    public string inGameSceneName = "3rdPerson+Fly 1";
}