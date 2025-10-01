using UnityEngine;

public class RooftopMover : MonoBehaviour
{
    private bool isVisible = false;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;         // 移動速度
    public float offset = 2f;            // 以起始X為中心，左右各 offset
    private float startX;
    private bool movingRight = true;

    void Start()
    {
        startX = transform.position.x;
    }

    void OnBecameVisible()
    {
        isVisible = true;
    }

    void Update()
    {
        if (!isVisible) return;

        Vector3 pos = transform.position;

        if (movingRight)
        {
            pos.x += moveSpeed * Time.deltaTime;
            if (pos.x >= startX + offset)
            {
                pos.x = startX + offset;
                movingRight = false;
            }
        }
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;
            if (pos.x <= startX - offset)
            {
                pos.x = startX - offset;
                movingRight = true;
            }
        }

        transform.position = pos;
    }
}
