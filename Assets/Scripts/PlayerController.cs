using UnityEngine;
using System.Collections;

// adds class boundary to set the bounds in which the player can move around
// data members can be set in Unity editor
[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    private new PolygonCollider2D collider;

    public float speed;
    public float tilt;
    public Boundary boundary;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Player Movement 
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * speed;

        //defines range of players movement
        rb.position = new Vector2
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax)
        );

        //ship rotation
        rb.transform.rotation = Quaternion.Euler(0.0f, rb.velocity.x * -tilt, 0.0f);
    }
}
