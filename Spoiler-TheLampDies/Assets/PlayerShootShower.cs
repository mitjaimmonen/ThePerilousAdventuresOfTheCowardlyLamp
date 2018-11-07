using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootShower : MonoBehaviour {

	public float projectileLifetime;
	public float damage;
	[Range(0f,1f)]
	public float accuracy;

	

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
		for (int i = 0; i < poolProjectileAmount; i++)
		{
			CreateProjectile();
		}
	}
	
	public void PlayerUpdate()
	{
		
	}

	Projectile CreateProjectile()
	{
		Projectile temp = Instantiate(projectilePrefab, Vector2.zero, Quaternion.identity);
		temp.gameObject.SetActive(false);
		projectiles.Add(temp);

		return temp;
	}
	void GetProjectile()
	{
		Projectile temp;
		if (projectiles[index].gameObject.activeSelf && growPool)
			temp = CreateProjectile();
		else
			temp = projectiles[index];
	}

}
