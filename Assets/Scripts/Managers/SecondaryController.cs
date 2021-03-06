﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SecondaryController : MonoBehaviour
{
    public const int gridX = 8;
    public const int gridY = 12;

    [HideInInspector]
    public int ySpawn;

    public Image shieldBar;
    public Image fireRateBar;
    public Image doubleFireBar;
    public Image speedBar;

    public new Camera camera;

    private int scoreBlue;
    private int scoreRed;
    private int scoreYellow;
    private int scoreGreen;
    private int scoreWhite;

    public float zOffset;
    public int matchScore;
    public GameObject[] powerups;

    public int targetBlue;
    public int targetRed;
    public int targetYellow;
    public int targetGreen;

    private GameObject[][] grid = new GameObject[gridX][];
    private List<GameObject> Match = new List<GameObject>();
    private List<int> Selected = new List<int>();

    private GameController gameController;
    private PlayerController player;

    [HideInInspector]
    public bool countScore = false;

    private bool mouseDeactive = false;
    private bool mouseActive = false;
    private bool mouseDown;
    private Vector3 mouseVector;
    private int mousePositionX;
    private int mousePositionY;

    void Start()
    {
        // Get GameController
        gameController = gameObject.GetComponent<GameController>();

        // Try to find the player
        try
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }
        catch (System.NullReferenceException)
        {
            player = null;
        }

        // Set spawn point for match objects
        ySpawn = gridY + 1;

        // Initialize mouse
        mouseDown = false;

        // Initialize score and score text
        scoreRed = 0;
        scoreBlue = 0;
        scoreYellow = 0;
        scoreGreen = 0;
        scoreWhite = 0;

        shieldBar.rectTransform.anchorMax = new Vector2((float)scoreBlue / (float)targetBlue, 1);
        fireRateBar.rectTransform.anchorMax = new Vector2((float)scoreRed / (float)targetRed, 1);
        doubleFireBar.rectTransform.anchorMax = new Vector2((float)scoreYellow / (float)targetYellow, 1);
        speedBar.rectTransform.anchorMax = new Vector2((float)scoreGreen / (float)targetGreen, 1);
        //whiteScore.text = "Turret On: " + scoreWhite + " / " + targetWhite;

        // Create 2D grid
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new GameObject[gridY];
        }

        // Fill empty grid with match objects
        StartCoroutine(InitSpawnPowerups());
    } // end function start

    void Update()
    {
        // get mouse position relative to icons position on screen
        mouseVector = camera.ScreenToWorldPoint(Input.mousePosition);
        mousePositionX = Mathf.RoundToInt(mouseVector.x);
        mousePositionY = Mathf.RoundToInt(mouseVector.y);

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

        // if the user clicks
        if ((Input.GetMouseButtonDown(0) || mouseDown) && mousePositionX >= 0 && mousePositionX < gridX && mousePositionY >= 0 && mousePositionY < gridY && mouseActive)
        {
            mouseDown = true;

            if (Selected.Count == 0)
            {
                // switch to selected sprite
                grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController>().SetSelected();

                Selected.Add(mousePositionX);
                Selected.Add(mousePositionY);
            }
            else if (Selected[0] == mousePositionX && (Selected[1] == (mousePositionY - 1) || Selected[1] == (mousePositionY + 1))
              || Selected[1] == mousePositionY && (Selected[0] == (mousePositionX - 1) || Selected[0] == (mousePositionX + 1)))
            {
                mouseActive = false;
                mouseDown = false;

                MatchObjectController oldSelectedObject = grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController>();
                MatchObjectController newSelectedObject = grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController>();

                oldSelectedObject.ClearSelected();

                GameObject Swap = grid[mousePositionX][mousePositionY];

                grid[mousePositionX][mousePositionY] = grid[Selected[0]][Selected[1]];
                oldSelectedObject.xPos = mousePositionX;
                oldSelectedObject.yPos = mousePositionY;
                oldSelectedObject.swap = true;
                oldSelectedObject.SortUp();

                grid[Selected[0]][Selected[1]] = Swap;
                newSelectedObject.xPos = Selected[0];
                newSelectedObject.yPos = Selected[1];
                newSelectedObject.swap = true;

                if (checkMatch())
                {
                    StartCoroutine(checkCycle());
                }
                else
                {
                    StartCoroutine(revertSwap(Selected[0], Selected[1], mousePositionX, mousePositionY, oldSelectedObject, newSelectedObject, Swap));
                }

                Swap = null;

                Selected.Clear();
                Selected.TrimExcess();

            }
            else if (mousePositionX != Selected[0] || mousePositionY != Selected[1])
            {
                // switch back to unselected sprite
                grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController>().ClearSelected();

                Selected.Clear();
                Selected.TrimExcess();
            }
        }
    } // end function Update

    // Spawn initial playfield
    IEnumerator InitSpawnPowerups()
    {
        for (int i = 0; i < gridX; i++)
        {

            for (int ii = 0; ii < gridY; ii++)
            {
                GameObject powerup = powerups[Random.Range(0, powerups.Length)];
                Vector3 spawnPosition = new Vector3(i, ii, zOffset);
                Quaternion spawnRotation = Quaternion.identity;
                GameObject spawned = Instantiate(powerup, spawnPosition, spawnRotation) as GameObject;
                spawned.GetComponent<MatchObjectController>().xPos = i;
                spawned.GetComponent<MatchObjectController>().yPos = ii;
                grid[i][ii] = spawned;
            }
        }

        bool run = checkMatch();
        //Debug.Log(run);
        while (run)
        {
            //Debug.Log("InitLoop");
            deleteMatches();
            yield return new WaitForSeconds(0.05f);

            for (int i = 0; i < gridX; i++)
            {
                bool spawnNew = false;
                for (int ii = 0; ii < gridY; ii++)
                {
                    if (!spawnNew && grid[i][ii] == null)
                    {
                        spawnNew = true;
                        for (int iii = ii + 1; iii < gridY; iii++)
                        {
                            if (grid[i][iii] != null)
                            {
                                grid[i][ii] = grid[i][iii];
                                grid[i][ii].GetComponent<MatchObjectController>().yPos = ii;
                                grid[i][ii].transform.position = new Vector3(i, ii, zOffset);
                                grid[i][iii] = null;
                                spawnNew = false;
                                break;
                            }
                        } // end for
                    }

                    if (spawnNew)
                    {
                        GameObject powerup = powerups[Random.Range(0, powerups.Length)];
                        Vector3 spawnPosition = new Vector3(i, ii, zOffset);
                        Quaternion spawnRotation = Quaternion.identity;
                        GameObject spawned = Instantiate(powerup, spawnPosition, spawnRotation) as GameObject;
                        spawned.GetComponent<MatchObjectController>().xPos = i;
                        spawned.GetComponent<MatchObjectController>().yPos = ii;
                        grid[i][ii] = spawned;
                    }
                } // end inner for
            } // end outer for

            run = checkMatch();
        } // end while

        countScore = true;
        mouseActive = true;
    } // end co-routine InitSpawnPowerups

    // Powerup Spawner
    void spawnPowerupsNoRoutine()
    {
        for (int i = 0; i < gridX; i++)
        {
            bool spawnNew = false;
            for (int ii = 0; ii < gridY; ii++)
            {
                if (!spawnNew && grid[i][ii] == null)
                {
                    spawnNew = true;
                    for (int iii = ii + 1; iii < gridY; iii++)
                    {
                        if (grid[i][iii] != null)
                        {
                            grid[i][ii] = grid[i][iii];
                            grid[i][ii].GetComponent<MatchObjectController>().yPos = ii;
                            grid[i][iii] = null;
                            spawnNew = false;
                            break;
                        }
                    }
                }

                if (spawnNew)
                {
                    GameObject powerup = powerups[Random.Range(0, powerups.Length)];
                    Vector3 spawnPosition = new Vector3(i, gridY, zOffset);
                    Quaternion spawnRotation = Quaternion.identity;
                    GameObject spawned = Instantiate(powerup, spawnPosition, spawnRotation) as GameObject;
                    spawned.GetComponent<MatchObjectController>().xPos = i;
                    spawned.GetComponent<MatchObjectController>().yPos = ii;
                    grid[i][ii] = spawned;
                }
            }
        }
    } // end spawnPowerups

    // Check for matches
    bool checkMatch()
    {
        bool ret = false;
        // Check for horizontal matches
        for (int i = 0; i < gridY; i++)
        {
            for (int ii = 0; ii < gridX; ii++)
            {
                //Debug.Log("grid["+i+"]["+ii+"]");
                List<GameObject> tempMatch = new List<GameObject>();
                tempMatch.Add(grid[ii][i]);
                int iii;
                for (iii = ii + 1; iii < gridX; iii++)
                {
                    if (grid[ii][i].tag == grid[iii][i].tag)
                    {
                        tempMatch.Add(grid[iii][i]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (tempMatch.Count >= 3)
                {
                    ret = true;
                    //Debug.Log("Horizontal match on object y." + i + " x." + ii + "; counted: " + tempMatch.Count + "; of type: " + grid[ii][i].tag);
                    foreach (GameObject x in tempMatch)
                    {
                        Match.Add(x);
                    }
                    addScore(tempMatch[0].tag, tempMatch.Count);
                }

                ii = iii - 1;
            }
        }

        // Check for vertical matches
        for (int i = 0; i < gridX; i++)
        {
            for (int ii = 0; ii < gridY; ii++)
            {
                List<GameObject> tempMatch = new List<GameObject>();
                tempMatch.Add(grid[i][ii]);
                int iii;
                for (iii = ii + 1; iii < gridY; iii++)
                {
                    if (grid[i][ii].tag == grid[i][iii].tag)
                    {
                        tempMatch.Add(grid[i][iii]);
                    }
                    else
                    {
                        break;
                    }
                }

                if (tempMatch.Count >= 3)
                {
                    ret = true;
                    //Debug.Log("Vertical match on object x." + i + " y." + ii + "; counted: " + tempMatch.Count + "; of type: " + grid[i][ii].tag);
                    foreach (GameObject x in tempMatch)
                    {
                        Match.Add(x);
                    }
                    addScore(tempMatch[0].tag, tempMatch.Count);
                }

                ii = iii - 1;
            }
        }
        return ret;
    } // end function checkMatch

    // Delete matches
    void deleteMatches()
    {
        foreach (GameObject i in Match)
        {
            Destroy(i);
        }
        Match.Clear();
        Match.TrimExcess();
    } // end function deleteMatches

    IEnumerator checkCycle()
    {
        bool run = true;
        yield return new WaitForSeconds(0.2f);
        while (run)
        {
            yield return new WaitForSeconds(0.05f);
            deleteMatches();
            yield return new WaitForSeconds(0.05f);
            //yield return StartCoroutine(spawnPowerups());
            spawnPowerupsNoRoutine();
            yield return new WaitForSeconds(0.05f);
            //Debug.Log("Finsihed checkCycle");
            run = checkMatch();
        }

        if (!mouseDeactive)
        {
            mouseActive = true;
        }
    } // end co-routine checkCycle

    IEnumerator revertSwap(int x, int y, int mousePositionX, int mousePositionY, MatchObjectController oldSelectedObject, MatchObjectController newSelectedObject, GameObject Swap)
    {
        yield return new WaitForSeconds(0.25f);
        grid[x][y] = grid[mousePositionX][mousePositionY];
        oldSelectedObject.xPos = x;
        oldSelectedObject.yPos = y;
        oldSelectedObject.swap = true;

        grid[mousePositionX][mousePositionY] = Swap;
        newSelectedObject.xPos = mousePositionX;
        newSelectedObject.yPos = mousePositionY;
        newSelectedObject.swap = true;
        newSelectedObject.SortUp();

        if (!mouseDeactive)
        {
            mouseActive = true;
        }
    } // end co-routine revertSwap

    void addScore(string tag, int count)
    {
        if (countScore && tag != "PowerupBlock")
        {
            if (tag == "PowerupShield")
            {
                scoreBlue += matchScore * (count - 2);

                // display score in progress bar
                shieldBar.rectTransform.anchorMax = new Vector2((float)scoreBlue / (float)targetBlue, 1);

                if (scoreBlue >= targetBlue && player != null)
                {
                    player.StartPowerup(1);
                    scoreBlue -= targetBlue;
                }
            }
            else if (tag == "PowerupFireRate")
            {
                scoreRed += matchScore * (count - 2);

                // display score in progress bar
                fireRateBar.rectTransform.anchorMax = new Vector2((float)scoreRed / (float)targetRed, 1);

                if (scoreRed >= targetRed && player != null)
                {
                    player.StartPowerup(2);
                    scoreRed -= targetRed;
                }
            }
            else if (tag == "PowerupDoubleFire")
            {
                scoreYellow += matchScore * (count - 2);

                // display score in progress bar
                doubleFireBar.rectTransform.anchorMax = new Vector2((float)scoreYellow / (float)targetYellow, 1);

                if (scoreYellow >= targetYellow && player != null)
                {
                    player.StartPowerup(3);
                    scoreYellow -= targetYellow;
                }
            }
            else if (tag == "PowerupSpeed")
            {
                scoreGreen += matchScore * (count - 2);

                // display score in progress bar
                speedBar.rectTransform.anchorMax = new Vector2((float)scoreGreen / (float)targetGreen, 1);

                if (scoreGreen >= targetGreen && player != null)
                {
                    player.StartPowerup(4);
                    scoreGreen -= targetGreen;
                }
            }

            // count total score in white
            scoreWhite += matchScore * (count - 2);

            // Add score to combined total
            gameController.AddScore(matchScore * (count - 2));
        }
    } // end function addScore

    public void DeactivateMouse()
    {
        mouseDeactive = true;
        mouseActive = false;

        //banner.SetActive(true);
    }

    public void ReactivateMouse()
    {
        mouseDeactive = false;
        mouseActive = true;

        //banner.SetActive(false);
    }
}
