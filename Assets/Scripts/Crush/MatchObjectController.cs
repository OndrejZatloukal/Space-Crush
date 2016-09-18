using UnityEngine;
using System.Collections;

public class MatchObjectController : MonoBehaviour
{
    public SpriteRenderer icon;
    public Sprite defaultIcon;
    public Sprite selectedIcon;
    public float smoothing;
    public float acceleration;

    [HideInInspector]
    public int yPos, xPos;
    [HideInInspector]
    public bool swap;

    //private Transform cubeTransform;
    private int gridY;
    private float zOffset;

    void Start()
    {
        swap = false;
        //cubeTransform = GetComponentsInChildren<Transform> ()[1];
        gridY = GameObject.FindWithTag("GameController").GetComponent<SecondaryController>().ySpawn;
        zOffset = GameObject.FindWithTag("GameController").GetComponent<SecondaryController>().zOffset;

        
    }

    void FixedUpdate()
    {
        // Fall into position
        if (!(transform.position.y == yPos || swap))
        {
            float newPosition = Mathf.MoveTowards(transform.position.y, yPos, Time.deltaTime * (smoothing * ((gridY - transform.position.y) * acceleration)));
            transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
        }

        // Swap activated by user
        if (swap)
        {
            if (transform.position.x != xPos)
            {
                float newPosition = Mathf.MoveTowards(transform.position.x, xPos, Time.deltaTime * smoothing * .5f);
                transform.position = new Vector3(newPosition, transform.position.y, zOffset + (transform.position.x - xPos));
            }
            else if (transform.position.y != yPos)
            {
                float newPosition = Mathf.MoveTowards(transform.position.y, yPos, Time.deltaTime * smoothing * .5f);
                transform.position = new Vector3(transform.position.x, newPosition, zOffset + (transform.position.y - yPos));
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
                swap = false;
            }
        }
    } // end FixedUpdate

    public void SetSelected()
    {
        icon.sprite = selectedIcon;
    } // end SetSelected

    public void ClearSelected()
    {
        icon.sprite = defaultIcon;
    } // end ClearSelected
} // end class MatchObjectController
