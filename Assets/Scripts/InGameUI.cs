using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("HUD")]
    public GameObject hudRoot; //timer + restartbutton
    public TMP_Text timerText;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject finalWinPanel;
    public GameObject failPanel;

    void Start()
    {
        // 初始状态
        if (hudRoot) hudRoot.SetActive(true);
        if (winPanel) winPanel.SetActive(false);
        if (finalWinPanel) finalWinPanel.SetActive(false);
        if (failPanel) failPanel.SetActive(false);
    }

    // 给计时器更新显示
    public void UpdateTimer(float timeLeft)
    {
        if (!timerText) return;

        if (timeLeft < 0) timeLeft = 0;
        int t = Mathf.CeilToInt(timeLeft);
        int m = t / 60;
        int s = t % 60;
        timerText.text = $"{m:00}:{s:00}";
    }

    // === 按钮回调 ===
    public void OnClickRestartLevel()
    {
        GameManager.I.RestartLevel();
    }

    public void OnClickRestartGame()
    {
        GameManager.I.RestartGame();
    }

    public void OnClickNextLevel()
    {
        GameManager.I.NextLevel();
    }

    // === GameManager 调用 ===

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