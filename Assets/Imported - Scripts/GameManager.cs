using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI waveText;
    private int score = 0;
    private float gameTimer = 0f;
    private bool gameActive = true;
    private int wave = 1;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!gameActive) return;

        gameTimer += Time.deltaTime;
        timerText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "s";

        // Every 30 seconds increase wave
        int newWave = Mathf.FloorToInt(gameTimer / 30f) + 1;
        if (newWave > wave)
        {
            wave = newWave;
            waveText.text = "Wave " + wave;
            EnemySpawner.instance.IncreaseSpawnRate();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameActive = false;
        AudioManager.instance.PlayGameOver();
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = "Score: " + score;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}