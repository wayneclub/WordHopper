// Assets/Scripts/CameraFollow.cs
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;     // 拖 Player 進來
    public float smoothSpeed = 5f;
    public float offsetX = 0f;     // 水平偏移（可選）
    public float skyBias = 1f;     // 垂直向上偏移，讓畫面多看天空
    public bool clampYToInitial = true; // 不讓相機低於開場的 Y
    public float followThresholdX = 3f; // 只有玩家與相機X距離超過這個數值時才開始跟隨
    public float verticalTopMargin = 2f;    // 玩家超過相機上邊界這距離就上移
    public float verticalBottomMargin = 1f; // 玩家低於相機下邊界這距離就下移（受 minY 限制）
    private float minY;            // 允許的最低 Y（通常是開場相機的 Y）

    float fixedZ;                // 固定的 Z

    void Start()
    {
        // 以目前相機距離為基準固定下來
        fixedZ = transform.position.z; // 2D 通常是 -10
        minY = clampYToInitial ? transform.position.y : float.NegativeInfinity;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 只有當玩家真正超出鏡頭左右邊界時才水平跟隨
        float desiredX = transform.position.x;
        float deltaX = target.position.x - transform.position.x;
        if (deltaX > followThresholdX) // 玩家超出右邊界
        {
            desiredX = target.position.x - followThresholdX + offsetX;
        }
        else if (deltaX < -followThresholdX) // 玩家超出左邊界
        {
            desiredX = target.position.x + followThresholdX + offsetX;
        }
        // 垂直：使用上下「死區」視窗。超過上緣就上移，低於下緣就下移（不低於 minY）。
        float desiredY = transform.position.y; // 預設不動
        float targetYWithBias = target.position.y + skyBias;
        float upper = transform.position.y + verticalTopMargin;
        float lower = Mathf.Max(minY, transform.position.y - verticalBottomMargin);
        if (targetYWithBias > upper)
        {
            // 將相機上移，讓玩家回到視窗內的上緣位置
            desiredY = targetYWithBias - verticalTopMargin;
        }
        else if (targetYWithBias < lower)
        {
            // 將相機下移，但不低於 minY
            desiredY = Mathf.Max(minY, targetYWithBias + verticalBottomMargin);
        }
        var desired = new Vector3(desiredX, desiredY, fixedZ);
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}