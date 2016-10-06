using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerupUI : MonoBehaviour {

    public Sprite[] sprites;
    public Image[] icons;
    public GameObject turretIcon;
    public GameObject turretOverlay;
    public GameObject continueButton;

    private SecondaryController secondaryController;

    void Start()
    {
        secondaryController = gameObject.GetComponent<SecondaryController>();

        turretIcon.SetActive(false);
        turretOverlay.SetActive(false);
        continueButton.SetActive(false);
    }

    public void Active(int index)
    {
        --index;
        if (index >= 0 && index < icons.Length)
        {
            icons[index].sprite = sprites[index * 2 + 1];
        }
    }

    public void Deactive(int index)
    {
        --index;
        if (index >= 0 && index < icons.Length)
        {
            icons[index].sprite = sprites[index * 2];
        }
    }

    public void TurretActive()
    {
        if (!turretOverlay.activeSelf)
        {
            secondaryController.DeactivateMouse();
            turretOverlay.SetActive(true);
            continueButton.SetActive(true);
            turretIcon.SetActive(true);
        }
    }

    public void TurretDeactive()
    {
        TurretOverlayDeactive();
        turretIcon.SetActive(false);
    }

    public void TurretOverlayDeactive()
    {
        if (turretOverlay.activeSelf)
        {
            turretOverlay.SetActive(false);
            continueButton.SetActive(false);
            secondaryController.ReactivateMouse();
        }
    }
}
