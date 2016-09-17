using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameObject levelManager; // mainly here for testing main scene
    public GameObject vfx; // for debugging

    public GameObject[] hazards;
    public int hazardCount;
    public int hazardCountInc;
    public float hazardType;
    public float hazardIncrease;

    public Vector2 spawnValues;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public float restartWait;

    public Text scoreText;
    public Text waveText;
    public Text gameOverText;
    public Text restartText;

    private bool gameOver;
    private bool restart;
    private int score;
    private int wave;

    void Awake() // mainly here for testing main scene
    {
        if (LevelManager.instance == null)
        {
            Instantiate(levelManager);
        }
    }

    void Start()
    {
        // initialize variables and GUI texts
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        wave = 0;
        UpdateScore();
        UpdateWave();

        // start spawning hazards
        StartCoroutine(SpawnWaves());
    }
	
	void Update()
    {
        // Press R to restart when game over
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                LevelManager.instance.RestartLevel();
            }
        }

        // Press escape to go back to menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            LevelManager.instance.LoadLevel("01a Main Menu");
        }

        // Debug VFX
        if (Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(vfx, new Vector3(0, 2, 0), Quaternion.Euler(0,0,0));
        }
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            wave += 1;
            UpdateWave(); // update wave number in GUI text

            // spawn the appropriate amount of hazards for this wave
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, Mathf.FloorToInt(hazardType + .00001f))];
                Vector2 spawnPosition = new Vector2(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }

            yield return new WaitForSeconds(waveWait);

            // increase the amount and types of hazards for next wave
            hazardCount += hazardCountInc;
            hazardType = Mathf.Min(hazardType + (1 / hazardIncrease), hazards.Length);

            if (gameOver)
            {
                break;
            }
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore(); // update score in GUI text
    }

    void UpdateScore()
    {
        scoreText.text = "" + score;
    }

    void UpdateWave()
    {
        waveText.text = "" + wave;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over";
        gameOver = true;

        Invoke("Restart", restartWait);
    }

    public void Restart()
    {
        restartText.text = "Press 'R' for Restart or 'Esc' for Main Menu";
        restart = true;
    }
}
