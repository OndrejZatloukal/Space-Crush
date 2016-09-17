using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerupUI : MonoBehaviour {

    public Sprite[] sprites;
    public Image[] icons;

    void Start()
    {
        //foreach (GUIText text in activeTexts)
        //{
        //    text.text = "";
        //}

        //img.sprite = 
    }

    public void Active(int index)
    {
        --index;
        icons[index].sprite = sprites[index * 2 + 1];
    }

    public void Deactive(int index)
    {
        --index;
        icons[index].sprite = sprites[index * 2];
    }
}
