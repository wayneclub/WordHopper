// ---------- GoalGate.cs (minimal)
using UnityEngine;
using WordHopper.UI; // for GuessPanel

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GoalGate : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GuessPanel panel; // drag from scene (recommended)

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }
    }

    private void Awake()
    {
        if (panel == null)
            panel = FindObjectOfType<GuessPanel>(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (panel == null)
        {
            Debug.LogError("[GoalGate] GuessPanel not assigned.");
            return;
        }
        panel.Open(WordManager.I?.CurrentWord ?? string.Empty);
    }
}
