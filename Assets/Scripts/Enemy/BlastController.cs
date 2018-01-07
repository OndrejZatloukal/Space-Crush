using UnityEngine;
using System.Collections;

public class BlastController : MonoBehaviour
{
    [SerializeField] GameObject explosion;

    [SerializeField] float smoothing;

    [HideInInspector] public float blastRadius;

    void Start()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0, 359)));
    }

    void FixedUpdate()
    {
        float scale = Mathf.MoveTowards(transform.localScale.x, blastRadius, Time.deltaTime * smoothing);
        transform.localScale = new Vector3(scale, scale, 1);

        if (scale == blastRadius)
        {
            Destroy(gameObject);
        }
    }
}
