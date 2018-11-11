using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public ParticleSystem activationPS;
	public Material ActiveCheckpoint;
	public Transform spawnPos;
	public Transform visuals;
	public TextMesh shardCountText;


	private int shardCount = 0;
	public int ShardCount
	{
		get { return shardCount; }
		set
		{
			shardCount = value;
			SetText();
		}
	}


	private void Awake()
	{
		SetText();
	}

	private void Update()
	{
		visuals.Rotate(new Vector3(0, 20f * Time.deltaTime, 0), Space.World);
	}

	private void SetText()
	{
		if (ShardCount <= 0)
			shardCountText.text = "";
		else
			shardCountText.text = ShardCount.ToString();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Player>())
		{
			//Player spawnposition will be set to this.
			var player = other.GetComponent<Player>();
			player.ActivateCheckpoint(this);
			if (activationPS)
				activationPS.Play();

			//Set checkpoint material active.
			var rend = visuals.GetComponentInChildren<Renderer>();
			rend.material = ActiveCheckpoint;
		}
	}
}
