using UnityEngine;

public class blockSfx : MonoBehaviour
{
    // 被 Block.BroadcastMessage("BlockMoved", moveChange) 调用（签名要匹配）
    public void BlockMoved(Vector2Int dir)
    {
        SfxManager.PlayPush();
    }
}