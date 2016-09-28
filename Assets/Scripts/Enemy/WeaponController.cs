using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour 
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

    private float speed;

	void Start() 
	{
        speed = GetComponent<Mover>().speed;
		InvokeRepeating("Fire", delay, fireRate);
	}

	void Fire()
	{
		GameObject firedShot = Instantiate(shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
        firedShot.GetComponent<Mover>().speed = speed * -4;

        SoundManager.instance.PlayEnemyShot();
    }
}
