using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrizeManager : MonoBehaviour
{
    private GameManager gameManager;
    private UI ui;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UI>();

        gameManager.gameObject.SetActive(false);
        ui.gameObject.SetActive(false);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadSceneAsync(1);

        gameManager.gameObject.SetActive(true);
        ui.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
