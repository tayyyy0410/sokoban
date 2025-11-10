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


        if (levelNames == null || levelNames.Length == 0)
        {
            Debug.LogWarning("GameManager: levelNames is empty, using active scene as single level (debug mode)");
            levelNames = new string[] { SceneManager.GetActiveScene().name };
            currentLevelIndex = 0;
        }
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
            string sceneName = levelNames[currentLevelIndex];
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("GameManager: Invalid currentLevelIndex = " + currentLevelIndex);
        }
    }

    public void RestartLevel()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levelNames.Length)
            return;

        string sceneName = levelNames[currentLevelIndex];
        SceneManager.LoadScene(sceneName);
        Debug.Log("GameManager: RestartLevel -> " + sceneName);
    }


    public void RestartGame()
    {
        currentLevelIndex = 0;
        LoadCurrentLevel();
        Debug.Log("GameManager: RestartGame -> " + levelNames[0]);
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
            Debug.LogWarning("GameManager: NextLevel called past last level. Clamped to last index.");
        }
    }

    public void LevelCompleted()
    {
        var ui = FindObjectOfType<InGameUI>();
        if (ui == null)
        {
            Debug.LogWarning("GameManager: LevelCompleted called but no InGameUI found in scene.");
            return;
        }

    
        if (currentLevelIndex >= 0 && currentLevelIndex < levelNames.Length - 1)
        {
            ui.ShowLevelWin();
        }
 
        else
        {
            ui.ShowFinalWin();
        }
    }


    public void LevelFailed()
    {
        var ui = FindObjectOfType<InGameUI>();
        if (ui == null)
        {
            Debug.LogWarning("GameManager: LevelFailed called but no InGameUI found in scene.");
            return;
        }

        ui.ShowFail();
    }
}
