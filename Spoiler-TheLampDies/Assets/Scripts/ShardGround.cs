using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardGround : MonoBehaviour {

	public GameObject shardItemPrefab;
	public int shardAmount;
	public float health;


	private float currentHealth;

	public void GetHit(float dmg)
	{
		currentHealth -= dmg;
		if (currentHealth <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		//Spawn shards
		for (int i = 0; i < shardAmount; i++)
		{
			Vector2 pos = transform.position;
			pos += Random.insideUnitCircle*2f; // Adds some randomization.
			Instantiate(shardItemPrefab, pos, Random.rotation);
		}

		//Destroy object
		Destroy(this.gameObject);
	}
}
