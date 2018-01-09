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
        FindGameController();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If collided object is not a Player all Player is already dead then do nothing
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy") || this.tag == "Dead")//|| other.CompareTag("Powerup")
        {
            return;
        }

        if (other.tag == "Player")
        {
            CreatePlayerExplosion(other);
            gameController.GameOver();
        }
        else
        {
            gameController.AddScore(scoreValue);
        }

        ReactToExplosion();

        // Destroys object collision with player
        if (other.tag != "Explosion" && other.tag != "Shield")
        {
            DestroyOther(other);
        }

        ReactToShieldExplosion(other);

        // Destroys player on collison with other object
        if (!CompareTag("Explosion"))
        {
            DestroyPlayer();
        }
    }

    private void FindGameController()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log(this.GetType() + " Error: Cannot find 'GameController' Script");
        }
    }

    private void CreatePlayerExplosion(Collider2D other)
    {
        Instantiate(playerExplosion, other.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        SoundManager.instance.PlayPlayerExplosion();
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

    private void CreateMissileExplosion(MissileController missileController)
    {
        GameObject Explosion = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
        Explosion.GetComponent<BlastController>().blastRadius = missileController.triggerRadius;
    }

    private void CreateOtherExplosion()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 359)));
    }

    private void PlayOtherExplosionSound()
    {
        SoundManager.instance.PlayExplosion(explosionSound);
    }

    private static void DestroyOther(Collider2D other)
    {
        Destroy(other.gameObject);
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

    private static void DelayShield(Collider2D other)
    {
        other.GetComponentInParent<PlayerController>().ShieldDownDelayed(.2f);
    }

    private static void DestroyShield(Collider2D other)
    {
        other.GetComponentInParent<PlayerController>().ShieldDown();
    }

    private void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}
