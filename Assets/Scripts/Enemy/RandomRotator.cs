using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    public float rotationSpeed;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(-1.0f, 1.0f) * rotationSpeed;
    }
}
