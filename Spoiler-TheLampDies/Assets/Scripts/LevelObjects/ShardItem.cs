using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardItem : MonoBehaviour {

	public int value = 1;
	private float maxSpeed = 10f;
	private Rigidbody2D rb;
	private Vector3 dist;

	private Player player;
	public Player Player
	{
		set { player = value; }
	}

	private void Awake()
	{
		if (rb == null)
		{
			rb = GetComponent<Rigidbody2D>();
			if (rb == null)
				rb = gameObject.AddComponent<Rigidbody2D>();
		}
	}

	private void Update()
	{

		if (player && player.CurrentHealth > 0)
		{
			dist = player.transform.position - transform.position;
			if (dist.magnitude < 1.5f)
			{
				player.CollectShardItem(value);
				Destroy(this.gameObject);
			}
		}
	}

	private void FixedUpdate() 
	{
		if (player)
		{
			if (dist.magnitude < player.magnetDistance)
			{
				rb.gravityScale = 0;
				dist *= 5f; // Gives more kick to magnet effect.
				rb.AddForce(dist);
				rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
			}
			else
			{
				//When away from player, gravity is normal and force cleared.
				rb.gravityScale = 1f;
				rb.velocity = new Vector2(rb.velocity.x*0.85f, rb.velocity.y);
			}
		}
	}
}
