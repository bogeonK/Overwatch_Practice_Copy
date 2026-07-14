using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : baseManager
{
    private readonly SceneLoadManagerConfigSO config;

    private bool isLoading;

    public SceneLoadManager(SceneLoadManagerConfigSO config)
    {
        this.config = config;
    }

    public override void Init()
    {
        isLoading = false;
    }

    public override void Update()
    {
    }

    public override void Destory()
    {
    }

    public void LoadLobbyScene()
    {
        LoadScene(config.lobbySceneName);
    }

    public void LoadInGameScene()
    {
        LoadScene(config.inGameSceneName);
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[SceneLoadManager] 씬 이름이 비어 있습니다.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[SceneLoadManager] '{sceneName}' 씬을 불러올 수 없습니다. " +
                "Build Profiles의 Scene List에 등록했는지 확인하세요."
            );

            return;
        }

        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    public void OnSceneLoaded()
    {
        isLoading = false;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}