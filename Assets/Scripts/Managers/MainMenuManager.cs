using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Text versionText;

	void Start ()
    {
        versionText.text += DatabaseManager.instance.version;
    }
}
