using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public Vector2 direction;
	public float damage;
	public float lifetime;
	public float startSpeed;
	public float endSpeed;


	private float currentSpeed;
	private float timeAlive;
	private Rigidbody rb;

	// Use this for initialization
	void Awake () 
	{
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeAlive += Time.deltaTime;
		currentSpeed = Mathf.Lerp(startSpeed, endSpeed, timeAlive / lifetime);
	}

	void FixedUpdate()
	{
		//Apply current speed in physics update.
		rb.velocity = direction * currentSpeed;
	}
	void OnCollisionEnter(Collision col)
	{
		var newDir = Vector2.Reflect(rb.velocity.normalized, col.contacts[0].normal);
		transform.up = newDir.normalized;
		direction = newDir.normalized;
		rb.velocity = direction * currentSpeed;
	}
}
