using UnityEngine;

public class blockSfx : MonoBehaviour
{
    
    public void BlockMoved(Vector2Int dir)
    {
        SfxManager.PlayPush();
    }
}