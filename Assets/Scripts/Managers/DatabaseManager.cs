using UnityEngine;
using System.Collections;

public class DatabaseManager : MonoBehaviour {

    public static DatabaseManager instance = null;

    private string privateKey = "NotTheFinalHashKey!";
    private string addScoreURL = "http://www.sugoientertainment.ca/development/php/AddScore.php?";
    private string topScoresURL = "http://www.sugoientertainment.ca/development/php/TopScores.php";
    private string name;

    public string version;

    private string[] randomName = {"Alfred", "Barry", "Charlie", "Dave", "Eric", "Fred", "Garry", "Harrold"};

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
        name = randomName[Random.Range(0, randomName.Length - 1)];
    }

    public IEnumerator UploadScore(int score)
    {
        string hash = Md5Sum(name + score + version + privateKey);
        WWW postScore = new WWW(addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&version=" + version + "&hash=" + hash);
        Debug.Log(addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&version=" + version + "&hash=" + hash);
        yield return postScore;

        if(postScore.error == null)
        {
            Debug.Log("Uploading score succesful.");
            StartCoroutine(DisplayTopScores());
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
                gameController.passTopScores(textArray);
            }
        }
        else
        {
            Debug.Log("Error in downloading scores.");
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
