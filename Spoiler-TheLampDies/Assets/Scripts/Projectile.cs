using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileData
{
	[HideInInspector]
	public Vector2 startPosition;
	[HideInInspector]
	public Vector2 direction;
	[HideInInspector]
	public Vector2 playerSpeed;
	public float damage;
	public Vector2 minMaxLifetime;
	public Vector2 startToEndSpeed;
	public Vector2 startToEndSize;
	public EasingCurves.Curve easing;
	public LayerMask reflectiveLayers;
	public LayerMask collidingLayers;


}
public class Projectile : MonoBehaviour, IDamageable {

	public GameObject visuals;
	public Collider2D col;
	[HideInInspector]
	public ProjectileData data;
	
	
	private Vector2 rbVelocityEffect;
	private Vector3 origSize;
	private float speed;
	private float size;
	private float lifetime;
	private float startTime;
	private float lerpTime;
	private bool active;
	private bool stopping;


	#region ParticleSystems
	public ParticleSystem trailPS;
	public ParticleSystem destroyPS;
	private float destroyPSOrigsize;
	private float trailPSOrigsize;
	private ParticleSystem.MainModule destroyMain;
	private ParticleSystem.MainModule trailMain;


	#endregion



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

		if (destroyPS)
		{
			destroyMain = destroyPS.main;
			destroyPSOrigsize = destroyMain.startSizeMultiplier;
		}
		if (trailPS)
		{
			trailMain = trailPS.main;
			trailPSOrigsize = trailMain.startSizeMultiplier;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If lifetime reached, start deactivation process
		if (active && !stopping && startTime + lifetime < Time.time)
			Deactivate();
		
		if (stopping)
		{
			//Checks that all particles are dead before deactivating
			if ((!trailPS || !trailPS.IsAlive()) && (!destroyPS || !destroyPS.IsAlive()))
			{
				active = false;
				stopping = false;
			}
		}
		
		if (active && !stopping)
		{
			//Calculate current lerp with easing applied to lifetime.
			lerpTime = EasingCurves.Easing((Time.time-startTime) / lifetime, data.easing);

			//Calculate speed and size with min-max values relative to time.
			speed = Mathf.Lerp(data.startToEndSpeed.x, data.startToEndSpeed.y, lerpTime);
			size = Mathf.Lerp(data.startToEndSize.x, data.startToEndSize.y, lerpTime);
			//Velocity effect is the added speed of player at the time of shot.
			rbVelocityEffect = Vector2.Lerp(data.playerSpeed, Vector2.zero, lerpTime);

			//Rotate towards movement direction.
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, data.direction), Time.deltaTime * 15f);

			//Set sizes.
			transform.localScale = origSize * size;
			if (trailPS)
				trailMain.startSizeMultiplier = trailPSOrigsize * size;


			//Apply current speed.
			var vel = speed * data.direction;

			//Add player speed effect into velocity if it is helpful.
			if ((rbVelocityEffect + vel).magnitude > vel.magnitude)
				rb.velocity = (data.direction * speed) + rbVelocityEffect;
			else
				rb.velocity = data.direction * speed;
		}

	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (!active || stopping) //Should not do anything if not active
			return;

		//The reflective layers bounce projectile.
		if (data.reflectiveLayers == (data.reflectiveLayers | (1 << col.gameObject.layer)))
		{
			var newDir = Vector2.Reflect(data.direction, col.contacts[0].normal);
			transform.up = newDir.normalized;
			data.direction = newDir.normalized;
			rb.velocity = data.direction * speed;
		}
		//Colliding layers destroy projectile. Enemies should be in colliding layers as well.
		if (data.collidingLayers == (data.collidingLayers | (1 << col.gameObject.layer)))
		{
			IDamageable iDmg = col.gameObject.GetComponent<IDamageable>();
			if (iDmg != null)
			{
				iDmg.GetHit(data.damage, col.contacts[0].point);
			}
			GameMaster.Instance.CameraHandler.CameraShake.StartShake(0.075f, 10f,EasingCurves.Curve.easeOut, 0.35f, 0);
			BlowUp();
		}
	}

	public void Activate()
	{
		stopping = false;
		active = true;
		startTime = Time.time;
		lifetime = Random.Range(data.minMaxLifetime.x,data.minMaxLifetime.y);
		rb.simulated = true;
		col.enabled = true;
		visuals.SetActive(true);
		transform.localScale = origSize;
		transform.position = data.startPosition;
		transform.rotation = Quaternion.LookRotation(Vector3.forward, data.direction);

		if (trailPS)
			trailPS.Play();	
	}

	public void GetHit(float dmg)
	{
		BlowUp();
	}
	public void GetHit(float dmg, Vector2 pos)
	{
		BlowUp();
	}

	public void BlowUp()
	{

		if (destroyPS)
		{
			destroyMain.startSizeMultiplier = destroyPSOrigsize * size;
			destroyPS.Play();
		}
		Deactivate();
	}

	public void Deactivate()
	{
		
		if (trailPS)
			trailPS.Stop();
		stopping = true;
		col.enabled = false;
		visuals.SetActive(false);
		rb.velocity = Vector2.zero;
		rb.simulated = false;
	}
}
