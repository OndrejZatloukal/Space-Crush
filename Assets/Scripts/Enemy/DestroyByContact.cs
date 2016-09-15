using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
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

        //Player Explosion
        if (other.tag == "Player")
        {
            gameController.GameOver();
        }
        else
        {
            gameController.AddScore(scoreValue);
        }

        //if (other.tag != "Explosion" && other.tag != "Shield")
        //{
        Destroy(other.gameObject);
        //}

        //if (other.CompareTag("Shield"))
        //{
        //    other.GetComponentInParent<PlayerController>().ShieldDown();
        //}

        Destroy(gameObject);
    }
}
