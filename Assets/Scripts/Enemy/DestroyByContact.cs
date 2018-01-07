using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject playerExplosion;

    [SerializeField] AudioClip explosionSound;

    [SerializeField] int scoreValue;

    private GameController gameController;

    void Start()
    {
        CheckIfGameControllerExists();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If collided object is not a Player all Player is already dead then do nothing
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy") || this.tag == "Dead")//|| other.CompareTag("Powerup")
        {
            return;
        }

        // Explosion
        ReactToExplosion();

        //Player Explosion
        if (other.tag == "Player")
        {
            CreatePlayerExplosion(other);
            EndTheGame();
        }
        else
        {
            gameController.AddScore(scoreValue);
        }

        if (other.tag != "Explosion" && other.tag != "Shield")
        {
            DestroyOtherOnCollisonWithPlayer(other);
        }

        ReactToShieldExplosion(other);

        if (!CompareTag("Explosion"))
        {
            DestroyPlayerOnCollisionWithOther();
        }
    }

    private void ReactToShieldExplosion(Collider2D other)
    {
        if (other.tag == "Shield")
        {
            if (CompareTag("Explosion"))
            {
                DelayShield(other);
            }
            else
            {
                DestroyShield(other);
            }
        }
    }

    private void DestroyPlayerOnCollisionWithOther()
    {
        Destroy(gameObject);
    }

    private static void DestroyShield(Collider2D other)
    {
        other.GetComponentInParent<PlayerController>().ShieldDown();
    }

    private static void DelayShield(Collider2D other)
    {
        other.GetComponentInParent<PlayerController>().ShieldDownDelayed(.2f);
    }

    private static void DestroyOtherOnCollisonWithPlayer(Collider2D other)
    {
        Destroy(other.gameObject);
    }

    private void CreatePlayerExplosion(Collider2D other)
    {
        Instantiate(playerExplosion, other.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        SoundManager.instance.PlayPlayerExplosion();
    }

    private void EndTheGame()
    {
        gameController.GameOver();
    }

    private void ReactToExplosion()
    {
        if (explosion != null)
        {
            MissileController missileController = GetComponent<MissileController>();

            if (missileController != null)
            {
                CreateMissileExplosion(missileController);
            }
            else
            {
                CreateOtherExplosion();
            }

            if (explosionSound != null)
            {
                PlayOtherExplosionSound();
            }
        }
    }

    private void CreateOtherExplosion()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 359)));
    }

    private void CreateMissileExplosion(MissileController missileController)
    {
        GameObject Explosion = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
        Explosion.GetComponent<BlastController>().blastRadius = missileController.triggerRadius;
    }

    private void PlayOtherExplosionSound()
    {
        SoundManager.instance.PlayExplosion(explosionSound);
    }

    private void CheckIfGameControllerExists()
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
}
