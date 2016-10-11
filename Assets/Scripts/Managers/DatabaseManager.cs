using UnityEngine;
using System.Collections;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance = null;

    private string privateKey = "NotTheFinalHashKey!";
    private string addScoreURL = "http://www.sugoientertainment.ca/development/php/AddScore.php?";
    private string topScoresURL = "http://www.sugoientertainment.ca/development/php/TopScores.php";
    private string grabRankURL = "http://www.sugoientertainment.ca/development/php/GrabRank.php?";
    private string surroundingScoresURL = "http://www.sugoientertainment.ca/development/php/SurroundingScores.php?";
    private uint sessionID;
    private uint currentRank;

    [HideInInspector]
    public string player1Name;
    [HideInInspector]
    public string player2Name;

    public string version;

    //private string[] randomName = {"Alfred", "Barry", "Charlie", "Dave", "Eric", "Fred", "Garry", "Harrold"};

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //player1Name = randomName[Random.Range(0, randomName.Length)];
        player1Name = "";
        player2Name = "";
        sessionID = 0;
        currentRank = 0;
    }

    public IEnumerator UploadScore(int score)
    {
        if (player1Name == "" || player2Name == "")
        {
            Debug.Log("Error: Missing player names,");
            yield break;
        }

        string hash = Md5Sum(player1Name + score + version + privateKey);
        WWW postScore = new WWW(addScoreURL + "sessionID=" + sessionID + "&name=" + WWW.EscapeURL(player1Name) + "&score=" + score + "&version=" + version + "&hash=" + hash);
        //Debug.Log(addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&version=" + version + "&hash=" + hash);
        Debug.Log("Posted Session ID: " + sessionID + " Name: " + player1Name + " Score: " + score + " Version " + version);
        yield return postScore;

        if(postScore.error == null)
        {
            Debug.Log("Uploading score succesful.");

            if (postScore.text.Substring(0,(postScore.text.Length >= 5 ? 5 : postScore.text.Length)) == "Error")
            {
                Debug.Log(postScore.text);
            }
            else
            {
                if (sessionID == 0)
                {
                    sessionID = System.Convert.ToUInt32(postScore.text);
                }
                else
                {
                    Debug.Log(postScore.text);
                }
                Debug.Log("Session ID: " + sessionID);

                StartCoroutine(DisplayTopScores());
            }
        }
        else
        {
            Debug.Log("Error in uploading score: " + postScore.error);
        }
    }

    public IEnumerator DisplayTopScores()
    {
        WWW getScores = new WWW(topScoresURL);
        yield return getScores;

        if (getScores.error == null)
        {
            if (getScores.text.Substring(0, (getScores.text.Length >= 6 ? 6 : getScores.text.Length)) == "Error:")
            {
                Debug.Log(getScores.text);
            }
            else
            {
                Debug.Log("Downloading scores succesful.");
                string[] textArray = getScores.text.Split(new string[] { "\n", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);

                // Find the new Game Controller since it will be destroyed after each reset
                GameObject gameControllerObject = GameObject.FindWithTag("GameController");
                GameController gameController = null;
                if (gameControllerObject != null)
                {
                    gameController = gameControllerObject.GetComponent<GameController>();
                }

                if (gameController == null)
                {
                    Debug.Log("Cannot find 'GameController' Script");
                }
                else
                {
                    gameController.passTop5Scores(textArray);
                }

                StartCoroutine(CountRank());
            }
        }
        else
        {
            Debug.Log("Error in downloading scores.");
        }
    }

    public IEnumerator CountRank()
    {
        WWW getRank = new WWW(grabRankURL + "sessionID=" + sessionID);
        yield return getRank;

        if (getRank.error == null)
        {
            if (getRank.text.Substring(0, (getRank.text.Length >= 5 ? 5 : getRank.text.Length)) == "Error")
            {
                Debug.Log(getRank.text);
            }
            else
            {
                currentRank = System.Convert.ToUInt32(getRank.text);
                Debug.Log("Rank: " + currentRank);

                StartCoroutine(DisplaySurroundingScores());
            }
        }
        else
        {
            Debug.Log("Error in downloading rank.");
        }
    }

    public IEnumerator DisplaySurroundingScores()
    {
        if (currentRank > 2) // otherwise display in Top 10
        {
            WWW getScores = new WWW(surroundingScoresURL + "sessionID=" + sessionID);
            yield return getScores;

            if (getScores.error == null)
            {
                if (getScores.text.Substring(0, (getScores.text.Length >= 6 ? 6 : getScores.text.Length)) == "Error:")
                {
                    Debug.Log(getScores.text);
                }
                else
                {
                    Debug.Log("Downloading scores succesful.");
                    string[] textArray = getScores.text.Split(new string[] { "\n", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);

                    // Find the new Game Controller since it will be destroyed after each reset
                    GameObject gameControllerObject = GameObject.FindWithTag("GameController");
                    GameController gameController = null;
                    if (gameControllerObject != null)
                    {
                        gameController = gameControllerObject.GetComponent<GameController>();
                    }

                    if (gameController == null)
                    {
                        Debug.Log("Cannot find 'GameController' Script");
                    }
                    else
                    {
                        gameController.passSurroundingScores(currentRank, textArray);
                    }
                }
            }
            else
            {
                Debug.Log("Error in downloading scores.");
            }
        }
    }

    private string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}
