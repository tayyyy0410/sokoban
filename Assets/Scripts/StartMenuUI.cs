using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    public void OnClickStart()
    {
        SfxManager.PlayButton();
        GameManager.I.StartGame();
    }

    public void OnClickQuit()
    {
        SfxManager.PlayButton();
        GameManager.I.QuitGame();
    }
}