using UnityEngine;
using System.Collections;

public class EvasiveManouver : MonoBehaviour 
{
	public float dodge;
	public float smoothing;
	public float tilt;
	public Vector2 startWait;
	public Vector2 manouverTime;
	public Vector2 manouverWait;
	public Boundary boundary;

	private float currentSpeed;
    protected float targetManouver;
	private Rigidbody2D rb;

	protected virtual void Start() 
	{
		rb = GetComponent <Rigidbody2D> ();
		currentSpeed = rb.velocity.y;
		StartCoroutine (SetManouver());
	}

	protected virtual IEnumerator SetManouver()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));

		while (true) 
		{
			targetManouver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			yield return new WaitForSeconds (Random.Range (manouverTime.x, manouverTime.y));
			targetManouver = 0;
			yield return new WaitForSeconds (Random.Range (manouverWait.x, manouverWait.y));
		}
	}
		
	void FixedUpdate() 
	{
		float newManouver = Mathf.MoveTowards (rb.velocity.x, targetManouver, Time.deltaTime * smoothing); 
		rb.velocity = new Vector2 (newManouver, currentSpeed);
		rb.position = new Vector2 (
			Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax),
			Mathf.Clamp (rb.position.y, boundary.yMin, boundary.yMax)
		);

		rb.transform.rotation = Quaternion.Euler (0.0f,rb.velocity.x * -tilt, 0.0f);
	}
}