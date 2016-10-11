using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Text versionText;
    public GameObject nameOverlay;
    public InputField name1;
    public InputField name2;

    void Start()
    {
        versionText.text += DatabaseManager.instance.version;

        if (DatabaseManager.instance.player1Name != "" && DatabaseManager.instance.player2Name != "")
        {
            nameOverlay.SetActive(false);
        }
    }

    public void SubmitNames()
    {
        Debug.Log("Name 1: " + name1.text + " Name 2: " + name2.text);
        if (name1.text != "" && name2.text != "")
        {
            DatabaseManager.instance.player1Name = name1.text;
            DatabaseManager.instance.player2Name = name2.text;

            nameOverlay.SetActive(false);
        }
    }
}
