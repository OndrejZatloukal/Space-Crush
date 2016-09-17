using UnityEngine;
using System.Collections;

public class DestroyOnAnimationEnd : MonoBehaviour
{
    public float delay;

	void Start ()
    {
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }
}
