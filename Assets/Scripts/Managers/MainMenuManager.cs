using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Text versionText;

    public Button startButton;
    public Button optionsButton;
    public Button helpButton;
    public Button leaderboardsButton;
    public Button creditsButton;

    public GameObject nameOverlay;
    public InputField name1;
    public InputField name2;
    public Text placeholder1;
    public Text placeholder2;

    private InputField[] inputFields = new InputField[2];
    private int inputIndex;

    void Start()
    {
        versionText.text += DatabaseManager.instance.version;

        // check if name overlay should be active
        if (DatabaseManager.instance.player1Name != "" && DatabaseManager.instance.player2Name != "")
        {
            nameOverlay.SetActive(false);
        }
        else
        {
            mainButtonsNotInteractable();

            inputFields[0] = name1;
            inputFields[1] = name2;

            inputIndex = 0;
            inputFields[inputIndex].ActivateInputField();
        }
    }

    void Update()
    {
        if (nameOverlay.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitNames();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inputIndex = 1 - inputIndex;
                inputFields[inputIndex].ActivateInputField();
            }
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
            mainButtonsInteractable();
        }
        else
        {
            placeholder1.text = "Enter name";
            placeholder2.text = "Enter name";
            placeholder1.color = new Color(0.77f, 0.055f, 0.055f, 1.0f); //C40E0EFF
            placeholder2.color = new Color(0.77f, 0.055f, 0.055f, 1.0f); //C40E0EFF
        }
    }

    void mainButtonsInteractable()
    {
        startButton.interactable = true;
        optionsButton.interactable = true;
        helpButton.interactable = true;
        //leaderboardsButton.interactable = true;
        creditsButton.interactable = true;
    }

    void mainButtonsNotInteractable()
    {
        startButton.interactable = false;
        optionsButton.interactable = false;
        helpButton.interactable = false;
        leaderboardsButton.interactable = false;
        creditsButton.interactable = false;
    }
}
