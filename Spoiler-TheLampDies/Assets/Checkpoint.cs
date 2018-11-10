using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public Material ActiveCheckpoint;
	public Transform spawnPos;
	private Transform visuals;

	private void Awake()
	{
		visuals = GetComponentInChildren<Renderer>().transform;
	}

	private void Update()
	{
		visuals.Rotate(new Vector3(0, 20f * Time.deltaTime, 0), Space.World);
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Player>())
		{
			//Player spawnposition will be set to this.
			var player = other.GetComponent<Player>();
			player.ActivateCheckpoint(this);

			//Set checkpoint material active.
			var rend = GetComponentInChildren<Renderer>();
			rend.material = ActiveCheckpoint;
		}
	}
}
