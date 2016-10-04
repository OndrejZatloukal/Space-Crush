using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public GameObject explosion;
    public AudioClip explosionSound;
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
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy") || this.tag == "Dead")//|| other.CompareTag("Powerup")
        {
            return;
        }

        // Explosion
		if (explosion != null) 
		{
            MissileController missileController = GetComponent<MissileController>();

            if (missileController != null)
            {
                GameObject Explosion = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
                Explosion.GetComponent<BlastController>().blastRadius = missileController.triggerRadius;
            }
            else
            {
                Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 359)));
            }

            if (explosionSound != null)
            {
                SoundManager.instance.PlayExplosion(explosionSound);
            }
        }

        //Player Explosion
        if (other.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (--playerController.health <= 0 || this.tag == "Explosion")
            {
                Destroy(other.gameObject);
                Instantiate(playerExplosion, other.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                SoundManager.instance.PlayPlayerExplosion();
                gameController.GameOver();
            }
            else
            {
                playerController.StopPowerup(Random.Range(1,4));
            }
        }
        else
        {
            gameController.AddScore(scoreValue);
        }

        if (other.tag != "Explosion" && other.tag != "Shield" && other.tag != "Player")
        {
            Destroy(other.gameObject);
        }

        if (other.tag == "Shield")
        {
            if (CompareTag("Explosion"))
            {
                other.GetComponentInParent<PlayerController>().ShieldDownDelayed(.2f);
            }
            else
            {
                other.GetComponentInParent<PlayerController>().ShieldDown();
            }
        }

        if (!CompareTag("Explosion"))
        {
            Destroy(gameObject);
        }
    }
}
