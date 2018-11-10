using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	public GameObject visuals; // Reference to gameObject containing player mesh & textures.
	public ParticleSystem diePS; // Reference under player object
	public ParticleSystem takeDamagePS; // Reference under player object
	public float magnetDistance; // Max distance from where player attracts items.
	public float health;
	[HideInInspector]public PlayerControl control;
	[HideInInspector]public PlayerCrosshair crosshair;
	[HideInInspector]public PlayerDashAttack dashAttack;
	[HideInInspector]public PlayerProjectileAttack projectileAttack;


	private Vector2 spawnPosition;
	private float currentHealth;

	public float CurrentHealth
	{
		get { return currentHealth; }
		set
		{
			if (value <= 0)
				currentHealth = 0;
			if (value > health)
				currentHealth = health;
			else
				currentHealth = value;
			
			if (GameMaster.Instance.PostProcessingHandler)
			{
				GameMaster.Instance.PostProcessingHandler.SetSaturationIntensity(currentHealth/health);
				GameMaster.Instance.PostProcessingHandler.SetVignetteIntensity( 1f -(currentHealth/health));

			}
		}
	}


	public Rigidbody2D rb
	{
		get { return GetComponent<Rigidbody2D>(); }
	}

	private int shardItemCount = 0;
	public int ShardItemCount
	{
		get { return shardItemCount; }
		set
		{
			if (value < 0)
				shardItemCount = 0;
			else
				shardItemCount = value;
		
			if (GameMaster.Instance.GameCanvas)
				GameMaster.Instance.GameCanvas.HudCanvas.UpdateItemCount();
		}
	}

	// Use this for initialization
	private void Awake () {
		currentHealth = health;
		spawnPosition = transform.position;
		control = GetComponent<PlayerControl>();
		crosshair = GetComponent<PlayerCrosshair>();
		dashAttack = GetComponent<PlayerDashAttack>();
		projectileAttack = GetComponent<PlayerProjectileAttack>();
	}

	public void MasterUpdate()
	{
		//Decides which script updates run first.
		crosshair.PlayerUpdate();
		control.PlayerUpdate();
		projectileAttack.PlayerUpdate();
		dashAttack.PlayerUpdate();
	}

	public void GetHit(float dmg)
	{

	}
	public void GetHit(float dmg, Vector2 pos)
	{
		CurrentHealth -= dmg;
		if (takeDamagePS)
		{
			takeDamagePS.transform.position = pos;
			takeDamagePS.Play();
		}

		if (CurrentHealth <= 0)
		{
			Die();
		}
	}
	public void CollectShardItem()
	{
		++ShardItemCount;
	}

	public void ActivateCheckpoint(Checkpoint checkpoint)
	{
		spawnPosition = checkpoint.transform.position;
		CurrentHealth = health;
	}

	private void Die()
	{
		visuals.SetActive(false);
		if (diePS)
			diePS.Play();

		rb.simulated = false;
		StartCoroutine(Respawn(0.5f));
	}

	private IEnumerator Respawn (float delay)
	{

		yield return new WaitForSeconds(delay);

		transform.position = spawnPosition;
		visuals.SetActive(true);
		rb.simulated = true;
		CurrentHealth = health;
		ShardItemCount = ShardItemCount - 100;

		yield break;
	}
}
