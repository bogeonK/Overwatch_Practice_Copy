using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnClickStartGame()
    {
        if (GameController.instance == null)
        {
            Debug.LogError(
                "[LobbyUI] GameController가 씬에 없습니다."
            );

            return;
        }

        GameController.instance
            .GetManager<SceneLoadManager>()
            .LoadInGameScene();
    }

    public void OnClickQuitGame()
    {
        if (GameController.instance == null)
            return;

        GameController.instance
            .GetManager<SceneLoadManager>()
            .QuitGame();
    }
}