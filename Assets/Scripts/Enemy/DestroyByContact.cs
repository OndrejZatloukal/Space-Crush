using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    public int scoreValue;

    private GameController gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' Script");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy") )//|| other.CompareTag("Powerup") || this.tag == "Dead")
        {
            return;
        }
        // Explosion
		if (explosion != null) 
		{
			Instantiate (explosion, transform.position, transform.rotation);
		}

        //Player Explosion
        if (other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }
        else
        {
            gameController.AddScore(scoreValue);
        }

        //if (other.tag != "Explosion" && other.tag != "Shield")
        if (other.tag != "Shield")
        {
            Destroy(other.gameObject);
        }

        if (other.tag == "Shield")
        {
            other.GetComponentInParent<PlayerController>().ShieldDown();
        }

        Destroy(gameObject);
    }
}
