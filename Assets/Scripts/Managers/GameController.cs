﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject levelManager; // mainly here for testing main scene
    public GameObject soundManager; // mainly here for testing main scene
    public GameObject cursorManager; // mainly here for testing main scene
    public GameObject databaseManager; // mainly here for testing main scene
    public GameObject vfx; // for debugging

    public GameObject[] hazards;
    public int hazardCount;
    public int hazardCountInc;
    public int hazardsActive;
    public int difficultyIncrease;

    public Vector2 spawnValues;
    public float spawnWait;
    public float startWait;
    public float waveWait;
    public float restartWait;
    public int targetSpecial;
    public float targetSpecialIncrement;

    public bool debug;
    [HideInInspector]
    public bool paused;

    public Text player1NameText;
    public Text player2NameText;

    public Text scoreText;
    public Text waveText;
    public Image specialBar;
    public GameObject gameOverOverlay;
    public Text finalScoreText;
    public Text finalWaveText;
    public Text[] topScoresRanksText;
    public Text[] topScoresNamesText;
    public Text[] topScoresScoresText;
    public Text[] surroundingScoresRanksText;
    public Text[] surroundingScoresNamesText;
    public Text[] surroundingScoresScoresText;

    private SecondaryController secondaryController;
    private PowerupUI powerupUI;
    private PlayerController playerController;

    private float hazardSpeed;
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

        if (CursorManager.instance == null)
        {
            Instantiate(cursorManager);
        }

        if (DatabaseManager.instance == null)
        {
            Instantiate(databaseManager);
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

        // set default cursor for browsers that don't pick up Unity's custom cursor
        CursorManager.instance.SetDefaultCursor();

        // initialize game variables
        hazardSpeed = 1;
        paused = false;
        gameOver = false;
        restart = false;

        // initialize UI variables
        gameOverOverlay.SetActive(false);
        if (DatabaseManager.instance.player1Name != "" && DatabaseManager.instance.player2Name != "")
        {
            player1NameText.text = DatabaseManager.instance.player1Name;
            player2NameText.text = DatabaseManager.instance.player2Name;
        }
        finalScoreText.text = "";
        finalWaveText.text = "";
        for (int i = 0; i < topScoresRanksText.Length; ++i)
        {
            topScoresRanksText[i].text = "";
            topScoresNamesText[i].text = "";
            topScoresScoresText[i].text = "";

            surroundingScoresRanksText[i].text = "";
            surroundingScoresNamesText[i].text = "";
            surroundingScoresScoresText[i].text = "";
        } // end for

        // initialize score values
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
                CursorManager.instance.SetDefaultCursor();
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
                GameObject hazard = hazards[Random.Range(0, hazardsActive)];
                Vector2 spawnPosition = new Vector2(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y);
                Quaternion spawnRotation = Quaternion.identity;
                if (hazardSpeed == 1)
                {
                    Instantiate(hazard, spawnPosition, spawnRotation);
                }
                else
                {
                    GameObject newHazard = Instantiate(hazard, spawnPosition, spawnRotation) as GameObject;
                    newHazard.GetComponent<Mover>().speed *= hazardSpeed;
                }
                yield return new WaitForSeconds(spawnWait);
            }

            yield return new WaitForSeconds(waveWait);

            // increase the amount and types of hazards for next wave
            hazardCount += hazardCountInc;
            if (wave % difficultyIncrease == 0)
            {
                if (hazardsActive < hazards.Length)
                {
                    ++hazardsActive;
                }
                else
                {
                    hazardSpeed += 0.2f;
                    if (spawnWait > 0.05f)
                    {
                        spawnWait -= 0.02f;
                        waveWait -= 0.05f;
                    }
                }
            }

            if (gameOver)
            {
                break;
            }
        }
    }

    public void AddScore(int newScoreValue)
    {
        if (!gameOver)
        {
            score += newScoreValue;
            scoreSpecial += newScoreValue;
            UpdateScore(); // update score in UI
        }
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
        powerupUI.TurretDeactive(); // in case the turret overlay was active when the player got destroyed
        secondaryController.DeactivateMouse();
        secondaryController.countScore = false; // stop counting matches after player death

        CursorManager.instance.SetDefaultCursor();  // in case the turret cursor was active when the player got destroyed

        SoundManager.instance.StopBackgroundMusic();

        if (!debug)
        {
            StartCoroutine(DatabaseManager.instance.UploadScore(score, wave));
        }

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

    public void passTop5Scores(string[] textArray)
    {
        for (int i = 0, j = 0; i < topScoresRanksText.Length && j < textArray.Length; ++i)
        {
            topScoresRanksText[i].text = System.Convert.ToString(i + 1);
            topScoresNamesText[i].text = textArray[j++] + " & " + textArray[j++];
            topScoresScoresText[i].text = textArray[j++];
        } // end for
    }

    public void passTop10Scores(uint rank, string[] textArray)
    {
        if (rank != 8)
        {
            // display player score in yellow
            if (rank <= 5)
            {
                topScoresRanksText[rank - 1].color = surroundingScoresRanksText[2].color;
                topScoresNamesText[rank - 1].color = surroundingScoresNamesText[2].color;
                topScoresScoresText[rank - 1].color = surroundingScoresScoresText[2].color;
            }
            else
            {
                surroundingScoresRanksText[rank - 6].color = surroundingScoresRanksText[2].color;
                surroundingScoresNamesText[rank - 6].color = surroundingScoresNamesText[2].color;
                surroundingScoresScoresText[rank - 6].color = surroundingScoresScoresText[2].color;
            }

            // reset text color to white
            surroundingScoresRanksText[2].color = Color.white;
            surroundingScoresNamesText[2].color = Color.white;
            surroundingScoresScoresText[2].color = Color.white;
        }

        int j = 0;
        for (int i = 0; i < topScoresRanksText.Length && j < textArray.Length; ++i)
        {
            topScoresRanksText[i].text = System.Convert.ToString(i + 1);
            topScoresNamesText[i].text = textArray[j++] + " & " + textArray[j++];
            topScoresScoresText[i].text = textArray[j++];
        } // end for

        for (int i = 0; i < surroundingScoresScoresText.Length && j < textArray.Length; ++i)
        {
            surroundingScoresRanksText[i].text = System.Convert.ToString(i + 6);
            surroundingScoresNamesText[i].text = textArray[j++] + " & " + textArray[j++];
            surroundingScoresScoresText[i].text = textArray[j++];
        } // end for
    }

    public void passSurroundingScores(uint rank, string[] textArray)
    {
        for (int i = 0, j = 0; i <surroundingScoresScoresText.Length && j < textArray.Length; ++i)
        {
            surroundingScoresRanksText[i].text = System.Convert.ToString(rank - 2 + i);
            surroundingScoresNamesText[i].text = textArray[j++] + " & " + textArray[j++];
            surroundingScoresScoresText[i].text = textArray[j++];
        } // end for
    }
}