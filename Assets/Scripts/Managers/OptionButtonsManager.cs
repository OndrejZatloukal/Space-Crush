using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionButtonsManager : MonoBehaviour
{
    public GameObject volumeSlider;
    public Image volumeOff;
    public Sprite volumeOffOff;
    public Sprite volumeOffOn;
    public Image volumeOn;
    public Sprite volumeOnOn;
    public Sprite volumeOnOff;
    public GameObject customCursor;
    public GameObject cursorMode;

    void Start()
    {
        volumeSlider.GetComponent<Slider>().value = SoundManager.instance.masterVolume;
        SetVolumeIcons();

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
        SetVolumeIcons();
    }

    private void SetVolumeIcons()
    {
        if (SoundManager.instance.masterVolume == 0)
        {
            volumeOff.sprite = volumeOffOn;
            volumeOn.sprite = volumeOnOff;
        }
        else
        {
            volumeOff.sprite = volumeOffOff;
            volumeOn.sprite = volumeOnOn;
        }
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
