using UnityEngine;
using System.Collections;

public class OptionButtonsManager : MonoBehaviour
{
    public void DisableCustomCursor()
    {
        CursorManager.instance.DisableCustomCursors();
    }

    public void EnableCustomCursors()
    {
        CursorManager.instance.EnableCustomCursors();
    }

    public void CursorModeAuto()
    {
        CursorManager.instance.SetCursoModeAuto();
    }

    public void CursorModeForceSoftware()
    {
        CursorManager.instance.SetCursorModeForceSoftware();
    }
}
