using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance = null;

    public Texture2D defaultCursor;
    public Texture2D turretCursor;

    public bool customCursors;

    private CursorMode cursorMode;
    private bool defaultCursorActive;

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
        cursorMode = CursorMode.Auto;
        SetDefaultCursor();
    }

    public void SetDefaultCursor()
    {
        if (customCursors)
        {
            Cursor.SetCursor(defaultCursor, new Vector2(16, 8), cursorMode);
            defaultCursorActive = true;
        }
    }

    public void SetTurretCursor()
    {
        if (customCursors)
        {
            Cursor.SetCursor(turretCursor, new Vector2(20, 20), cursorMode);
            defaultCursorActive = false;
        }
    }

    public void EnableCustomCursors()
    {
        DebugLog();

        customCursors = true;
        RefreshCursor();
    }

    public void DisableCustomCursors()
    {
        DebugLog();

        customCursors = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void SetCursoModeAuto()
    {
        cursorMode = CursorMode.Auto;
        RefreshCursor();
    }

    public void SetCursorModeForceSoftware()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        cursorMode = CursorMode.ForceSoftware;

        Invoke("RefreshCursor", 0.05f);

        //EnableCustomCursors();

        //RefreshCursor();
    }

    public void RefreshCursor()
    {
        if (customCursors)
        {
            if (defaultCursorActive)
            {
                SetDefaultCursor();
            }
            else
            {
                SetTurretCursor();
            }
        }
    }

    public void DebugLog()
    {
        Debug.Log("Custom cursor: " + (customCursors ? "On" : "Off") + ", Current cursor: " + (defaultCursorActive ? "default" : "turret"));
    }
}
