using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
    private Transform playerTransform;

    [SerializeField] Animator flashAnimator;

    [SerializeField] GameObject blastRadiusExplosion;

    [SerializeField] float flashSpeed;
    [SerializeField] float flashIncrement;

    public float triggerRadius;

    void Start()
    {
        FindPlayer();
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // Missle flashing light setup
            flashAnimator.speed = Mathf.Max(1, flashSpeed - (Vector3.Distance(transform.position, playerTransform.position) / flashIncrement));

            // If player enters trigger radius then...
            if (Vector3.Distance(transform.position, playerTransform.position) < triggerRadius)
            {
                DestroyMissile();
            }

        }
        else
        {
            flashAnimator.speed = 1.0f;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.Log(this.GetType() + " Error: Couldn't find the Player object");
        }
    }

    private void DestroyMissile()
    {
        this.tag = "Dead";
        GameObject Explosion = Instantiate(blastRadiusExplosion, transform.position, transform.rotation) as GameObject;
        Explosion.GetComponent<BlastController>().blastRadius = triggerRadius;
        Destroy(gameObject);
    }
}
