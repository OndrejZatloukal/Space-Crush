using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionButtonsManager : MonoBehaviour
{
    public GameObject volumeSlider;
    public GameObject customCursor;
    public GameObject cursorMode;

    void Start()
    {
        volumeSlider.GetComponent<Slider>().value = SoundManager.instance.masterVolume;

        if (CursorManager.instance.customCursors)
        {
            customCursor.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            customCursor.GetComponent<Toggle>().isOn = false;
        }

        if (CursorManager.instance.cursorMode == CursorMode.ForceSoftware)
        {
            cursorMode.GetComponent<Toggle>().isOn = true;
        }
        else
        {
            cursorMode.GetComponent<Toggle>().isOn = false;
        }
    }

    public void SetMasterVolume()
    {
        SoundManager.instance.SetMasterVolume(volumeSlider.GetComponent<Slider>().value);
    }

    public void ToggleCustomCursor()
    {
        if (customCursor.GetComponent<Toggle>().isOn)
        {
            CursorManager.instance.EnableCustomCursors();
        }
        else
        {
            CursorManager.instance.DisableCustomCursors();
        }
    }

    public void ToggleCursorMode()
    {
        if (cursorMode.GetComponent<Toggle>().isOn)
        {
            CursorManager.instance.SetCursorModeForceSoftware();
        }
        else
        {
            CursorManager.instance.SetCursoModeAuto();
        }
    }
}
