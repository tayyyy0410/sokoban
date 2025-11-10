using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    [Header("HUD")]
    public GameObject hudRoot; 
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject finalWinPanel;
    public GameObject failPanel;

    void Start()
    {
      
        if (hudRoot) hudRoot.SetActive(true);
        if (winPanel) winPanel.SetActive(false);
        if (finalWinPanel) finalWinPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(false);
    }

 
    public void UpdateTimer(float timeLeft)
    {
        if (!timerText) return;

        if (timeLeft < 0) timeLeft = 0;
        int t = Mathf.CeilToInt(timeLeft);
        int m = t / 60;
        int s = t % 60;
        timerText.text = $"{m:00}:{s:00}";
    }


    public void OnClickRestartLevel()
    {
     
        SfxManager.PlayButton();
        GameManager.I.RestartLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }   

    public void OnClickRestartGame()
    {
        SfxManager.PlayButton();   
        GameManager.I.RestartGame();
    }

    public void OnClickNextLevel()
    {
        SfxManager.PlayButton();
        GameManager.I.NextLevel();
    }



    public void ShowLevelWin()
    {
        if (hudRoot) hudRoot.SetActive(false);
        if (winPanel) winPanel.SetActive(true);
    }

    public void ShowFinalWin()
    {
        if (hudRoot) hudRoot.SetActive(false);
        if (finalWinPanel) finalWinPanel.SetActive(true);
    }

    public void ShowFail()
    {
        if (hudRoot) hudRoot.SetActive(false);
        if (failPanel) failPanel.SetActive(true);
    }
}