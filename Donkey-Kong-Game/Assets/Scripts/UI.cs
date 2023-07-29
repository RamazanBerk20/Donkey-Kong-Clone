using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private GameManager gameManager;

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private void Awake()
    {
        GetComponent<Canvas>().enabled = false;
        DontDestroyOnLoad(gameObject);
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(Enabler());
    }

    IEnumerator Enabler()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex >= 1);

        GetComponent<Canvas>().enabled = true;

        StopCoroutine(Enabler());
    }

    private void Update()
    {
        livesText.text = "Lives: " + gameManager.lives.ToString();
        scoreText.text = "Score: " + gameManager.score.ToString();
        highScoreText.text = "High Score: " + gameManager.highScore.ToString();
    }
}
