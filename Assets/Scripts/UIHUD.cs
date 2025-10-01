// Assets/Scripts/UIHUD.cs
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UIHUD : MonoBehaviour
{
    public static UIHUD I;

    [Header("Refs")]
    public TMP_Text livesText;   // 左上：♥♥♥
    public TMP_Text wordText;    // 中上：已收集字母
    public TMP_Text timerText;   // 右上：01:00
    public TMP_Text gameOverText;  // 中央：GAME OVER（預設關閉）
    public GameObject playAgainButton; // Play Again 按鈕，初始關閉

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel; // ESC 顯示的暫停選單（Resume/Restart/Exit）
    [SerializeField] private string startSceneName = "StartScene"; // Exit 目標場景名（請在 Build Settings 中存在）
    [SerializeField] private string pauseMenuObjectName = "PauseMenuPanel"; // 若未指定，會用這個名稱自動尋找

    [Header("Settings")]
    public int startLives = 3;      // 初始生命
    public float startSeconds = 60; // 1 分鐘

    private int lives;
    private float timeLeft;
    private bool ticking;
    private StringBuilder collected = new StringBuilder();
    private bool isPaused = false;

    public int Lives => lives;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        // 若忘了拖引用，嘗試用名稱在此 Canvas 底下尋找
        if (pauseMenuPanel == null && !string.IsNullOrEmpty(pauseMenuObjectName))
        {
            var trs = GetComponentsInChildren<Transform>(true);
            foreach (var t in trs)
            {
                if (t.name == pauseMenuObjectName)
                {
                    pauseMenuPanel = t.gameObject;
                    break;
                }
            }
        }

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false); // 一開始隱藏暫停選單
        lives = startLives;
        timeLeft = startSeconds;
        ticking = true;

        UpdateLivesUI();
        UpdateWordUI();
        UpdateTimerUI(force: true);
        if (gameOverText) gameOverText.gameObject.SetActive(false);
        if (playAgainButton) playAgainButton.SetActive(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("[UIHUD] ESC pressed");
            bool gameEnded = (gameOverText && gameOverText.gameObject.activeSelf);
            if (!gameEnded)
            {
                if (isPaused)
                {
                    Debug.Log("[UIHUD] Resume() called");
                    Resume();
                }
                else
                {
                    Debug.Log("[UIHUD] Pause() called");
                    Pause();
                }
            }
            else
            {
                Debug.Log("[UIHUD] ESC ignored (game over/win screen active)");
            }
        }

        if (!ticking) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            ticking = false;
            ShowGameOver();
        }
        UpdateTimerUI();
    }

    // === Public API ===
    public void AddLetter(char c)
    {
        if (collected.Length > 0) collected.Append(' ');
        collected.Append(char.ToUpperInvariant(c));
        UpdateWordUI();
    }

    public void LoseLife(int amount = 1)
    {
        lives = Mathf.Max(0, lives - amount);
        UpdateLivesUI();
        if (lives == 0)
        {
            ticking = false;
            ShowGameOver();
        }
    }

    public void ResetHUD(string initialWordHint = null)
    {
        lives = startLives;
        timeLeft = startSeconds;
        ticking = true;
        collected.Clear();
        if (!string.IsNullOrEmpty(initialWordHint))
            collected.Append(initialWordHint); // 例如底線提示 "_ _ _ _"
        UpdateLivesUI();
        UpdateWordUI();
        UpdateTimerUI(force: true);
        if (gameOverText) gameOverText.gameObject.SetActive(false);
    }

    // === UI Helpers ===
    void UpdateLivesUI()
    {
        // 使用 Unicode ♥，避免某些字型不支援 ❤
        livesText.text = new string('\u2665', lives) + " "; // \u2665 = ♥
        // 也可：livesText.text = $"Lives: {lives}";
    }

    void UpdateWordUI()
    {
        wordText.text = $"{collected}";
    }

    void UpdateTimerUI(bool force = false)
    {
        // mm:ss
        int m = Mathf.FloorToInt(timeLeft / 60f);
        int s = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }

    public void ShowGameOver()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        isPaused = false;
        if (gameOverText)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
        }
        if (playAgainButton) playAgainButton.SetActive(true);
        Time.timeScale = 0f; // 停止遊戲
    }

    public void ShowWin()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        isPaused = false;
        if (gameOverText) // 你可以重用這個欄位顯示通關，也可新增一個 winText
        {
            gameOverText.text = "YOU WIN!";
            gameOverText.gameObject.SetActive(true);
        }
        if (playAgainButton) playAgainButton.SetActive(true);
        Time.timeScale = 0f; // 停止遊戲
    }

    public void PlayAgain()
    {
        // 恢復遊戲速度並重新載入目前場景
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // === Pause Menu ===
    public void Pause()
    {
        if (isPaused) return;
        if (pauseMenuPanel == null)
        {
            Debug.LogWarning("[UIHUD] pauseMenuPanel is not assigned. Attempting to find by name: " + pauseMenuObjectName);
            if (!string.IsNullOrEmpty(pauseMenuObjectName))
            {
                var trs = GetComponentsInChildren<Transform>(true);
                foreach (var t in trs)
                {
                    if (t.name == pauseMenuObjectName)
                    {
                        pauseMenuPanel = t.gameObject;
                        break;
                    }
                }
            }
        }
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        else Debug.LogError("[UIHUD] PauseMenuPanel not found. Create a UI Panel named '" + pauseMenuObjectName + "' under GameUICanvas and assign it.");
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (!isPaused) return;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(startSceneName))
            SceneManager.LoadScene(startSceneName);
        else
            SceneManager.LoadScene(0); // 後備：回到 Build Settings 的第一個場景
    }

    [ContextMenu("_Test/Show Pause Menu")] public void __TestShowPause() { Pause(); }
}