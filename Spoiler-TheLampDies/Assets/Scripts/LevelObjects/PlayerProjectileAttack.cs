using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileAttack : MonoBehaviour {

	[Range(0f,1f)]
	public float accuracy;
	public float cooldown;
	public int amountPerShot;
	public ProjectileData projectileData;

	

	private Player player;
	private float shotTime;	

	[Header("Pool")]
	public Projectile projectilePrefab;
	public int maxProjectileAmount;
	public bool growPool;

	private Transform poolParent;
	private List<Projectile> projectiles = new List<Projectile>();
	private Projectile currentProjectile;
	private int index;
	private int Index
	{
		get { return index; }
		set
		{
			if (value >= projectiles.Count)
				index = 0;
			else if (value < 0)
				index = projectiles.Count-1;
			else
				index = value;
		}
	}


	// Use this for initialization
	void Awake () {
		player = GetComponent<Player>();
		for (int i = 0; i < maxProjectileAmount; i++)
		{
			CreateProjectile();
		}
	}
	
	public void PlayerUpdate()
	{
		if (Input.GetMouseButton(0) && shotTime+cooldown < Time.time)
		{
			GameMaster.Instance.SoundMaster.PlayShoot(transform.position);
			for (int i = 0; i < amountPerShot; i++)
				Shoot();
		}
	}

	private void Shoot()
	{
		var dir = GetMouseDirectionFromPosition(transform.position).normalized;
		dir.y += Random.Range(1-accuracy,-1+accuracy);
		dir.x += Random.Range(1-accuracy,-1+accuracy);
		dir.Normalize();

		projectileData.direction = dir;
		projectileData.startPosition = new Vector2(transform.position.x, transform.position.y) + dir*0.5f;
		projectileData.initialVelocityEffect = player.rb.velocity;

		currentProjectile = GetProjectile();
		currentProjectile.data = projectileData;
		currentProjectile.Activate();

		shotTime = Time.time;

	}

	private Vector2 GetMouseDirectionFromPosition (Vector2 pos)
	{
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
		Vector2 dir = new Vector2(worldPoint.x - pos.x, worldPoint.y - pos.y);

		return dir;
	}

	Projectile CreateProjectile()
	{
		Projectile temp = Instantiate(projectilePrefab, Vector2.zero, Quaternion.identity);
		temp.Deactivate();
		projectiles.Add(temp);

		if (poolParent)
			temp.transform.parent = poolParent;
		else
		{
			poolParent = new GameObject().transform;
			poolParent.gameObject.name = "Projectile Pool";
			temp.transform.parent = poolParent;
		}

		return temp;
	}
	Projectile GetProjectile()
	{
		Projectile temp;

		//Ideally this for-loop will only go once, but in case there aren't enough projectiles it will loop through the whole list.
		for (int i = 0; i < projectiles.Count; i++)
		{
			if (!projectiles[Index].IsActive)
			{
				temp = projectiles[Index];
				++Index; //Go to next index already to avoid unnecessary looping next time.
				return temp;
			}
			else
			{
				++Index; //Continues looping with new index
			}
		}

		//creates new projectile into pool if all others are still active and pool is allowed to grow.
		if (projectiles[index].IsActive && (growPool || projectiles.Count < maxProjectileAmount))
		{
			temp = CreateProjectile();
			Index = projectiles.Count-1;
		}
		else
		{
			//Deactivates an active projectile and puts it into re-use.
			++Index;
			temp = projectiles[index];
			temp.Deactivate();
		}
		
		return temp;
	}

}
