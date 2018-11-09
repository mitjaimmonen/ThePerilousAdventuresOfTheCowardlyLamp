using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardGround : MonoBehaviour, IDamageable {

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
			Vector3 pos = transform.position;
			pos += Random.insideUnitSphere*2f; // Adds some randomization.
			pos.z = 0;
			Instantiate(shardItemPrefab, pos, Random.rotation);
		}

		//Destroy object
		Destroy(this.gameObject);
	}
}
