using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public float timeLimit = 60f;   
    public bool autoStart = true;   

    float timeLeft;
    bool running;
    InGameUI ui;

    void Start()
    {
        ui = FindObjectOfType<InGameUI>();
        if (autoStart)
            StartTimer();
    }

    public void StartTimer()
    {
        timeLeft = timeLimit;
        running = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        running = false;
        UpdateUI();
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        UpdateUI();

        if (timeLeft <= 0f)
        {
            running = false;
            GameManager.I.LevelFailed();
        }
    }

    void UpdateUI()
    {
        if (ui != null)
            ui.UpdateTimer(timeLeft);
    }
}