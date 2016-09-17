using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum Side { Bow, Port, Starboard, Stern}

// adds class boundary to set the bounds in which the player can move around
// data members can be set in Unity editor
[System.Serializable]
public class Boundary
{
    public float xMin, xMax, yMin, yMax;
}

public class PlayerController : MonoBehaviour
{
    private PowerupUI powerupUI;
    private Rigidbody2D rb;
    private new PolygonCollider2D collider;
    private Animator shotFlashBowanimator;
    private Animator portThrusterAnimator;
    private Animator starboardThrusterAnimator;

    public float speed;
    public float tilt;
    public Boundary boundary;

    public GameObject shotFlash;
    public GameObject portThruster;
    public GameObject starboardThruster;
    public GameObject shield;

    public GameObject shot;
    public Transform[] shotSpawns;
    public float fireRate;
    public float powerupTime;

    public bool startShield = false;
    public bool debugPlayer = true;

    private float nextFire;
    private float fireRateDown;
    private float doubleFireDown;
    private float speedDown;

    private bool fireRatePower;
    private List<int> shotSpawnsActive = new List<int>();
    private bool speedUpPower;

    void Start()
    {
        // Find the Game Controller
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            powerupUI = gameControllerObject.GetComponent<PowerupUI>();
        }
        if (powerupUI == null)
        {
            Debug.Log("Cannot find 'PowerupUI' Script");
        }

        // get components
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        portThrusterAnimator = portThruster.GetComponent<Animator>();
        starboardThrusterAnimator = starboardThruster.GetComponent<Animator>();

        // set up power up data
        shield.SetActive(false);
        fireRatePower = false;
        shotSpawnsActive.Add((int)Side.Bow);

        // set shield state
        if (startShield)
        {
            StartPowerup(1);
        }
    }

	void Update()
    {
        // shoot
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            // shoot from all active guns
            foreach (int i in shotSpawnsActive)
            {
                (Instantiate(shotFlash, shotSpawns[i].position, Quaternion.Euler(0.0f, 0.0f, shotSpawns[i].rotation.z * Mathf.Rad2Deg)) as GameObject).transform.parent = transform;
                Instantiate(shot, shotSpawns[i].position, Quaternion.Euler(0.0f, 0.0f, shotSpawns[i].rotation.z * Mathf.Rad2Deg));
            };
        }

        // Debug commands
        if (debugPlayer)
        {
            // Activate Powerup Shield
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartPowerup(1);
            }

            // Activate Powerup Fire Rate
            if (Input.GetKeyDown(KeyCode.O))
            {
                StartPowerup(2);
            }

            // Activate Powerup Double Fire
            if (Input.GetKeyDown(KeyCode.I))
            {
                StartPowerup(3);
            }

            // Activate Powerup Speed
            if (Input.GetKeyDown(KeyCode.U))
            {
                StartPowerup(4);
            }
        } // end debug commands
    } // end Update

    // Player Movement 
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.velocity = movement * speed;

        // defines range of players movement
        rb.position = new Vector2
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax)
        );

        // ship rotation
        rb.transform.rotation = Quaternion.Euler(0.0f, rb.velocity.x * -tilt, 0.0f);
        // counter rotate shield
        shield.transform.rotation = Quaternion.Euler(0.0f, rb.velocity.x * +tilt, 0.0f);
    } // end FixedUpdate

    // ---------------------------------
    // Power-up Functions
    // ---------------------------------

    public void StartPowerup(int index)
    {
        if (index == 1)
        {
            Shield();
        }
        else if (index == 2)
        {
            StartCoroutine(FireRate());
        }
        else if (index == 3)
        {
            StartCoroutine(DoubleFire());
        }
        else if (index == 4)
        {
            StartCoroutine(SpeedPower());
        }
        else if (index == 5)
        {
            //StartCoroutine(Turret());
        }
    } // end StartPowerup

    private void Shield()
    {
        if (shield.activeSelf)
        {
            return;
        }
        else
        {
            // power up
            shield.SetActive(true);
            collider.enabled = false;
            powerupUI.Active(1);
        }
    } // end Shield

    public void ShieldDown()
    {
        // power down
        shield.SetActive(false);
        collider.enabled = true;
        powerupUI.Deactive(1);
    } // end ShieldDown

    private IEnumerator FireRate()
    {
        fireRateDown = Time.time + powerupTime;
        if (fireRatePower == false)
        {
            // power up
            fireRatePower = true;
            fireRate = fireRate / 2;
            powerupUI.Active(2);

            // wait for timer to run out
            yield return new WaitWhile(() => fireRateDown > Time.time);

            // power down
            fireRate = fireRate * 2;
            fireRatePower = false;
            powerupUI.Deactive(2);
        }
    } // end IEnumerator Firerate

    private IEnumerator DoubleFire()
    {
        doubleFireDown = Time.time + powerupTime;
        if (shotSpawnsActive.Count < 2)
        {
            // power up
            shotSpawnsActive[0] = (int)Side.Port;
            shotSpawnsActive.Add((int)Side.Starboard);
            powerupUI.Active(3);

            // wait for timer to run out
            yield return new WaitWhile(() => doubleFireDown > Time.time);

            // power down
            shotSpawnsActive.Clear();
            shotSpawnsActive.Add((int)Side.Bow);
            powerupUI.Deactive(3);
        }
    } // end IEnumerator FireDouble

    public IEnumerator SpeedPower()
    {
        speedDown = Time.time + powerupTime;
        if (speedUpPower == false)
        {
            // power up
            speedUpPower = true;
            speed = speed * 1.5f;
            powerupUI.Active(4);
            portThrusterAnimator.SetTrigger("engageBoost");
            starboardThrusterAnimator.SetTrigger("engageBoost");

            // wait for timer to run out
            yield return new WaitWhile(() => speedDown > Time.time);

            // power down
            speed = speed / 1.5f;
            speedUpPower = false;
            powerupUI.Deactive(4);
            portThrusterAnimator.SetTrigger("disengageBoost");
            starboardThrusterAnimator.SetTrigger("disengageBoost");
        }
    } // end IEnumerator SpeedPower
} // end class PlayerController
