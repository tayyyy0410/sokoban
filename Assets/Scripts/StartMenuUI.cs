using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    public void OnClickStart()
    {
        GameManager.I.StartGame();
    }

    public void OnClickQuit()
    {
        GameManager.I.QuitGame();
    }
}