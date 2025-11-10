using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    // 写成你真实的关卡场景名，注意和 Build Settings 完全一致
    // 顺序 = 实际游玩的顺序
    public string[] levelNames = { "Level1", "Level2", "Level3" };

    // 用来表示“当前在第几个关卡”（对应上面数组的索引，从 0 开始）
    int currentLevelIndex = -1;

    void Awake()
    {
        // 单例 & 跨场景保留
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        // Debug 模式：如果没配 levelNames，或者你单独开关卡场景运行
        if (levelNames == null || levelNames.Length == 0)
        {
            Debug.LogWarning("GameManager: levelNames is empty, using active scene as single level (debug mode)");
            levelNames = new string[] { SceneManager.GetActiveScene().name };
            currentLevelIndex = 0;
        }
    }

    // 从 StartScene 的 Start 按钮调用
    public void StartGame()
    {
        currentLevelIndex = 0;      // 对应 levelNames[0]，也就是 Level1
        LoadCurrentLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
        // Editor 里不会退出，正常
    }

    // 根据 currentLevelIndex 加载对应关卡
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

    // 在当前关卡内点“Restart Level”按钮
    public void RestartLevel()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levelNames.Length)
            return;

        string sceneName = levelNames[currentLevelIndex];
        SceneManager.LoadScene(sceneName);
        Debug.Log("GameManager: RestartLevel -> " + sceneName);
    }

    // 从 FinalWinPanel / 任意需要的地方“重新开始整个游戏”
    public void RestartGame()
    {
        currentLevelIndex = 0;
        LoadCurrentLevel();
        Debug.Log("GameManager: RestartGame -> " + levelNames[0]);
    }

    // 从 WinPanel 的 Next Level 按钮调用
    public void NextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelNames.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
            // 理论上不会被用到：最后一关的 UI 不该再调 NextLevel
            currentLevelIndex = levelNames.Length - 1;
            Debug.LogWarning("GameManager: NextLevel called past last level. Clamped to last index.");
        }
    }

    // 被 MatchManager / 其它胜利条件调用
    public void LevelCompleted()
    {
        var ui = FindObjectOfType<InGameUI>();
        if (ui == null)
        {
            Debug.LogWarning("GameManager: LevelCompleted called but no InGameUI found in scene.");
            return;
        }

        // 还没到最后一关：显示普通胜利面板（有 Next Level）
        if (currentLevelIndex >= 0 && currentLevelIndex < levelNames.Length - 1)
        {
            ui.ShowLevelWin();
        }
        // 最后一关：显示最终胜利面板（只提供 Restart Game 等）
        else
        {
            ui.ShowFinalWin();
        }
    }

    // 被计时器 / 失败条件调用
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
