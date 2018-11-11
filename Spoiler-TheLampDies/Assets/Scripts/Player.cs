using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	public GameObject visuals; // Reference to gameObject containing player mesh & textures.
	public ParticleSystem diePS; // Reference under player object
	public ParticleSystem takeDamagePS; // Reference under player object
	public float magnetDistance; // Max distance from where player attracts items.
	public float maxHealth;

	[HideInInspector]public PlayerControl control;
	[HideInInspector]public PlayerDashAttack dashAttack;
	[HideInInspector]public PlayerProjectileAttack projectileAttack;


	private Checkpoint lastCheckpoint;
	private Vector2 startPos;
	private float currentHealth;
	private int shardItemCount = 0;



	#region  Getters & Setters
	public float CurrentHealth
	{
		get { return currentHealth; }
		set
		{
			if (value <= 0)
				currentHealth = 0;
			if (value > maxHealth)
				currentHealth = maxHealth;
			else
				currentHealth = value;
			
			if (GameMaster.Instance.PostProcessingHandler)
			{
				float val = currentHealth/maxHealth;
				GameMaster.Instance.PostProcessingHandler.SetSaturationIntensity(val);
				GameMaster.Instance.PostProcessingHandler.SetVignetteIntensity( 1f -val);

			}
		}
	}


	public Rigidbody2D rb
	{
		get { return GetComponent<Rigidbody2D>(); }
	}

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
				GameMaster.Instance.GameCanvas.UpdateItemCount();
		}
	}

	#endregion


	// Use this for initialization
	private void Awake () {
		maxHealth = Mathf.Max(1f, maxHealth); // Make sure health is at least 1 
		currentHealth = maxHealth;
		startPos = transform.position;
		control = GetComponent<PlayerControl>();
		dashAttack = GetComponent<PlayerDashAttack>();
		projectileAttack = GetComponent<PlayerProjectileAttack>();
	}


	//GameMaster calls update if game is not paused.
	public void MasterUpdate()
	{
		//Decides which script updates run first.
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
	public void CollectShardItem(int value)
	{
		ShardItemCount += value;
	}

	public void ActivateCheckpoint(Checkpoint checkpoint)
	{
		lastCheckpoint = checkpoint;
		CurrentHealth = maxHealth;
		checkpoint.ShardCount = ShardItemCount;
	}

	private void Die()
	{
		visuals.SetActive(false);
		if (diePS)
			diePS.Play();

		rb.simulated = false;
		GameMaster.Instance.CameraHandler.CameraShake.StartShake(0.75f, 2f, EasingCurves.Curve.easeOut, 1f, 0);
		StartCoroutine(Respawn(1f));
	}

	private IEnumerator Respawn (float delay)
	{

		yield return new WaitForSeconds(delay);

		if (lastCheckpoint)
		{
			ShardItemCount = lastCheckpoint.ShardCount;
			transform.position = lastCheckpoint.spawnPos.position;
		}
		else
		{
			ShardItemCount = 0;
			transform.position = startPos;
		}
		
		visuals.SetActive(true);
		rb.simulated = true;
		CurrentHealth = maxHealth;
		

		yield break;
	}

	public void FinishLevel()
	{
		GameMaster.Instance.IsFinished = true;
	}
}
