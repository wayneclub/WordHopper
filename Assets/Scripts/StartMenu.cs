using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System

public class StartMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene"; // 也可留空用 index
    [SerializeField] private int gameSceneIndex = 1;             // 預設 1

    void Start()
    {
        Time.timeScale = 1f; // 確保未暫停
    }

    public void OnClickStart()
    {
        LoadGameScene();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
        {
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        // 你可用名稱或 index 載入；優先用名稱（填了就用）
        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(gameSceneIndex, LoadSceneMode.Single);
    }

    [ContextMenu("Start Game Now")]
    private void __ContextStart()
    {
        LoadGameScene();
    }
}