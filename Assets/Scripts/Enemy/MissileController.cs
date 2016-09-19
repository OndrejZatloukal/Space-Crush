using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour
{
    private Transform playerTransform;

    public Animator flashAnimator;

    public GameObject blastRadiusExplosion;
    public float triggerRadius;
    public float flashSpeed;
    public float flashIncrement;

    // Use this for initialization
    void Start()
    {
        // find player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            flashAnimator.speed = Mathf.Max(1, flashSpeed - (Vector3.Distance(transform.position, playerTransform.position) / flashIncrement));

            if (Vector3.Distance(transform.position, playerTransform.position) < triggerRadius)
            {
                this.tag = "Dead";
                GameObject Explosion = Instantiate(blastRadiusExplosion, transform.position, transform.rotation) as GameObject;
                Explosion.GetComponent<BlastController>().blastRadius = triggerRadius;
                Destroy(gameObject);
            }

        }
        else
        {
            flashAnimator.speed = 1.0f;
        }
    }
}
