using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

 
    public string[] levelNames = { "Level1", "Level2", "Level3" };

    int currentLevelIndex = -1;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }


    public void StartGame()
    {
        currentLevelIndex = 0;
        LoadCurrentLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void LoadCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelNames.Length)
        {
            SceneManager.LoadScene(levelNames[currentLevelIndex]);
        }
    }

 
    public void RestartLevel()
    {
        if (currentLevelIndex < 0) return;
        SceneManager.LoadScene(levelNames[currentLevelIndex]);
        Debug.Log("game restart!");
        
    }

  
    public void RestartGame()
    {
        currentLevelIndex = 0;
        LoadCurrentLevel();
    }


    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
           
            currentLevelIndex = levelNames.Length - 1;
        }
    }

    
    public void LevelCompleted()
    {
        var ui = FindObjectOfType<InGameUI>();
        if (ui == null) return;

        if (currentLevelIndex < levelNames.Length - 1)
            ui.ShowLevelWin();   // 有 Next Level 按钮
        else
            ui.ShowFinalWin();   // 最终胜利，有 Restart Game
    }

 
    public void LevelFailed()
    {
        var ui = FindObjectOfType<InGameUI>();
        if (ui != null)
            ui.ShowFail();
    }
}

