using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardGround : MonoBehaviour, IDamageable {

	public ShardItem shardItemPrefab;
	public ParticleSystem getHitPS;
	public int shardAmount;
	public float health;
	private float currentHealth;

	private Player player;

	private void Start()
	{
		currentHealth = health;
		player = GameMaster.Instance.Player;
	}

	public void GetHit(float dmg)
	{
		//Sets hit position to own position, less spaghetti
		GetHit(dmg, transform.position);
	}
	public void GetHit(float dmg, Vector2 pos)
	{
		currentHealth -= dmg;
		if (getHitPS)
		{
			getHitPS.transform.position = pos;
			getHitPS.Play();
		}

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		if (shardItemPrefab == null)
		{
			Debug.LogWarning("No shard prefab!");
			return;
		}
		//Spawn shards
		for (int i = 0; i < shardAmount; i++)
		{
			Vector3 pos = transform.position;
			pos += Random.insideUnitSphere; // Adds some randomization.
			pos.z = 0;
			var temp = Instantiate(shardItemPrefab, pos, Random.rotation);
			temp.Player = player;
		}

		//Destroy object
		Destroy(this.gameObject);
	}
}
