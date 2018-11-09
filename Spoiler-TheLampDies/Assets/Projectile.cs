using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileData
{
	public Vector2 startPosition;
	public Vector2 direction;
	public float damage;
	public float lifetime;
	public float startSpeed;
	public float endSpeed;
	public LayerMask reflectiveLayers;
	public LayerMask collidingLayers;


}
public class Projectile : MonoBehaviour {

	public GameObject visuals;
	public ParticleSystem trailPS;
	public ParticleSystem destroyPS;
	public Collider2D col;
	public ProjectileData data;
	
	
	private Vector3 origSize;
	private float currentSpeed;
	private float currentSizeMultiplier;
	private float timeAlive;
	private bool active;
	public bool IsActive
	{
		get { return active; }
	}
	private Rigidbody2D rb;

	// Use this for initialization
	void Awake () 
	{
		rb = GetComponent<Rigidbody2D>();
		if (!rb)
			rb = gameObject.AddComponent<Rigidbody2D>();
		
		origSize = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeAlive += Time.deltaTime;
		if (timeAlive > data.lifetime)
			Deactivate();
		
		if (active)
		{
			currentSizeMultiplier = Mathf.Clamp01(1f- (timeAlive / data.lifetime));
			currentSpeed = Mathf.Lerp(data.startSpeed, data.endSpeed, timeAlive / data.lifetime);

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, data.direction), Time.deltaTime * 15f);
			transform.localScale = origSize * currentSizeMultiplier;
		}

	}

	void FixedUpdate()
	{
		if (active)
		{
			//Apply current speed in physics update.
			rb.velocity = data.direction * currentSpeed;
		}
	}
	void OnCollisionEnter2D(Collision2D col)
	{
		if (!active) //Should not do anything if not active
			return;

		//The reflective layers bounce projectile.
		if (data.reflectiveLayers == (data.reflectiveLayers | (1 << col.gameObject.layer)))
		{
			var newDir = Vector2.Reflect(data.direction, col.contacts[0].normal);
			Debug.Log("Reflecting old dir: " + data.direction.normalized + ", new dir: " + newDir.normalized);
			transform.up = newDir.normalized;
			data.direction = newDir.normalized;
			rb.velocity = data.direction * currentSpeed;
		}
		//Colliding layers destroy projectile. Enemies should be in colliding layers as well.
		if (data.collidingLayers == (data.collidingLayers | (1 << col.gameObject.layer)))
		{
			IDamageable iDmg = col.gameObject.GetComponent<IDamageable>();
			if (iDmg != null)
			{
				iDmg.GetHit(data.damage);
			}
			Deactivate();
		}
	}

	public void Activate()
	{
		active = true;
		timeAlive = 0;
		rb.simulated = true;
		col.enabled = true;
		visuals.SetActive(true);
		transform.localScale = origSize;
		transform.position = data.startPosition;
		transform.rotation = Quaternion.LookRotation(Vector3.forward, data.direction);

		if (trailPS)
			trailPS.Play();	
	}

	public void Deactivate()
	{
		
		if (trailPS)
			trailPS.Stop();
		if (destroyPS)
			destroyPS.Play();
		active = false;
		col.enabled = false;
		visuals.SetActive(false);
		rb.velocity = Vector2.zero;
		rb.simulated = false;
	}
}
