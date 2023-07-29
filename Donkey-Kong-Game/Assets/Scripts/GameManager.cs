using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private int score;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }
    
    private void NewGame()
    {
        lives = 3;
        score = 0;
        
        LoadLevel(1);
    }

    private void LoadLevel(int index)
    {
        level = index;

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        Camera camera = Camera.main;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);
        if (camera != null)
            camera.cullingMask = 0;

        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(level);

        Time.timeScale = 1f;
    }

    public void LevelCompleted()
    {
        score += 1000;

        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            nextLevel = 1;
            LoadLevel(nextLevel);
        }
    }

    public void LevelFailed()
    {
        lives--;

        if (lives <= 0)
        {
            NewGame();
        }
        else
        {
            LoadLevel(level);
        }
    }
}
