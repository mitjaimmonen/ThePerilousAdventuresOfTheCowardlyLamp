using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	[SerializeField] private ParticleSystem activationPS;
	[SerializeField] private Material ActiveCheckpoint;
	[SerializeField] private Transform spawnTrans;
	[SerializeField] private Transform visuals;
	[SerializeField] private TextMesh shardCountText;


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
	public Vector3 SpawnPos
	{
		get{ return spawnTrans.position; }
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

			//Play feedback sound and visuals
			GameMaster.Instance.SoundMaster.PlayCheckpoint(transform.position);
			if (activationPS)
				activationPS.Play();

			//Set checkpoint material active.
			var rend = visuals.GetComponentInChildren<Renderer>();
			rend.material = ActiveCheckpoint;
		}
	}
}
