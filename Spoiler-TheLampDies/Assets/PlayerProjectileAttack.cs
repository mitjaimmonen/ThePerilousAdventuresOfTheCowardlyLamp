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
	public int poolProjectileAmount;
	public bool growPool;
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
		for (int i = 0; i < poolProjectileAmount; i++)
		{
			CreateProjectile();
		}
	}
	
	public void PlayerUpdate()
	{
		if (Input.GetMouseButton(0) && shotTime+cooldown < Time.time)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		var dir = player.crosshair.GetMouseDirectionFromPosition(transform.position).normalized;
		dir.y += Random.Range(1-accuracy,-1+accuracy);
		dir.x += Random.Range(1-accuracy,-1+accuracy);
		dir.Normalize();

		projectileData.direction = dir;
		projectileData.startPosition = transform.position;

		currentProjectile = GetProjectile();
		currentProjectile.data = projectileData;
		currentProjectile.Activate();

		shotTime = Time.time;

	}
	

	Projectile CreateProjectile()
	{
		Projectile temp = Instantiate(projectilePrefab, Vector2.zero, Quaternion.identity);
		temp.Deactivate();
		projectiles.Add(temp);

		return temp;
	}
	Projectile GetProjectile()
	{
		Projectile temp;
		if (projectiles[index].IsActive && (growPool || projectiles.Count < poolProjectileAmount))
		{
			temp = CreateProjectile();
			Index = projectiles.Count-1;
		}
		else
		{
			temp = projectiles[index];
			++Index;
		}
		
		return temp;
	}

}
