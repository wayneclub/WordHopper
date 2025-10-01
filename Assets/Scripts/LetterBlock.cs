using UnityEngine;
using TMPro;  // ← 很重要：使用 TextMeshPro

[RequireComponent(typeof(BoxCollider2D))]
public class LetterBlock : MonoBehaviour
{
    [Header("把方塊上的 Text (TMP) 拖進來")]
    public TMP_Text letterText;

    private char letter;
    private bool assigned = false;
    private bool revealed = false;

    void Awake()
    {
        // 確保一開始不顯示
        if (letterText) letterText.gameObject.SetActive(false);
    }

    // BlockLetterAssigner 會呼叫這個方法來指定字母
    public void SetLetter(char c)
    {
        letter = c;
        assigned = true;
        if (letterText)
        {
            letterText.text = letter.ToString();
            letterText.gameObject.SetActive(false); // 撞到後才顯示
        }
    }

    // 方塊用「非 Trigger」Collider：用 OnCollisionEnter2D
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!revealed && col.gameObject.CompareTag("Player"))
        {
            Reveal();
        }
    }

    // 如果你的方塊改成 IsTrigger = true，請把上面改成 OnTriggerEnter2D(Collider2D other)
    // 並用 other.CompareTag("Player")

    private void Reveal()
    {
        if (!assigned)
        {
            Debug.LogWarning("[LetterBlock] 尚未被指派字母就被撞到。請確認 BlockLetterAssigner 是否在 Start 分配。");
        }
        revealed = true;
        if (letterText) letterText.gameObject.SetActive(true);
        UIHUD.I?.AddLetter(letter); // ← 通知 HUD 新增字母
    }
}