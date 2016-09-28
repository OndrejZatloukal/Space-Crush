using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject levelManager; // mainly here for testing main scene
    public GameObject soundManager; // mainly here for testing main scene
    public GameObject vfx; // for debugging

    public Texture2D defaultCursor;
    public Texture2D turretCursor;

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
    public Image specialBar;
    public GameObject gameOverOverlay;
    public Text finalScoreText;
    public Text finalWaveText;
    public int targetSpecial;
    public float targetSpecialIncrement;
    public bool debug;
    [HideInInspector]
    public bool paused;

    private SecondaryController secondaryController;
    private PowerupUI powerupUI;
    private PlayerController playerController;

    private bool gameOver;
    private bool restart;
    private int score;
    private int wave;
    private int scoreSpecial;
    private int baseTargetSpecial;

    void Awake() // mainly here for testing main scene
    {
        if (LevelManager.instance == null)
        {
            Instantiate(levelManager);
        }

        if (SoundManager.instance == null)
        {
            Instantiate(soundManager);
        }
    }

    void Start()
    {
        // get secondary controller
        secondaryController = gameObject.GetComponent<SecondaryController>();
        powerupUI = gameObject.GetComponent<PowerupUI>();

        // try to find the player
        try
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        catch (System.NullReferenceException)
        {
            playerController = null;
        }

        // initialize variables and UI
        paused = false;
        gameOver = false;
        restart = false;
        gameOverOverlay.SetActive(false);
        finalScoreText.text = "";
        finalWaveText.text = "";
        score = 0;
        wave = 0;
        scoreSpecial = 0;
        baseTargetSpecial = targetSpecial;
        UpdateScore();
        UpdateWave();

        specialBar.rectTransform.anchorMax = new Vector2(scoreSpecial / targetSpecial, 1);

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
        // Or press R to unpause the game when paused
        else if (paused)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                Unpause();
            }
        }

        // Press escape to go back to menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            // Or press escape to pause the game when in debug mode
            if (debug && !paused && !gameOver)
            {
                Pause();
            }
            else
            {
                Unpause();
                LevelManager.instance.LoadLevel("01a Main Menu");
            }
        }

        // Debug VFX
        if (debug && Input.GetKeyDown(KeyCode.X) && vfx != null)
        {
            Instantiate(vfx, new Vector3(0, 2, 0), Quaternion.Euler(0,0,0));
        }

        // Activate debug mode
        if (!debug && Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            debug = true;
            playerController.debugPlayer = true;
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
        scoreSpecial += newScoreValue;
        UpdateScore(); // update score in UI
    }

    void UpdateScore()
    {
        scoreText.text = "" + score;
        specialBar.rectTransform.anchorMax = new Vector2((float)scoreSpecial / (float)targetSpecial, 1);

        if (scoreSpecial >= targetSpecial && playerController != null)
        {
            playerController.StartPowerup(5);
            scoreSpecial -= targetSpecial;

            // increase target
            targetSpecial = targetSpecial + Mathf.FloorToInt(baseTargetSpecial * targetSpecialIncrement);
        }
    }

    void UpdateWave()
    {
        waveText.text = "" + wave;
    }

    public void GameOver()
    {
        gameOver = true;

        // deactivate Crush game
        powerupUI.TurretOverlayDeactive();
        secondaryController.DeactivateMouse();
        SetDefaultCursor(); // in case the turret cursor was active when the play got destroyed

        SoundManager.instance.StopBackgroundMusic();

        Invoke("Restart", restartWait);
    }

    public void Restart()
    {
        gameOverOverlay.SetActive(true);
        finalScoreText.text = "" + score;
        finalWaveText.text = "" + wave;
        restart = true;
    }

    public void Pause()
    {
        if (!paused)
        {
            Time.timeScale = 0.0f;
            paused = true;
            secondaryController.DeactivateMouse();
        }
    }

    public void Unpause()
    {
        if (paused)
        {
            Time.timeScale = 1.0f;
            paused = false;
            secondaryController.ReactivateMouse();
        }
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, new Vector2(20, 10), CursorMode.Auto);
    }

    public void SetTurretCursor()
    {
        Cursor.SetCursor(turretCursor, new Vector2(25, 25), CursorMode.Auto);
    }
}
