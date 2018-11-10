using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : MonoBehaviour {

	[Tooltip("Laser plays this PS constantly if hitting something.")]
	public ParticleSystem laserContactPSPrefab;
	[Tooltip("Laser plays this PS if hitting something damageable")]
	public ParticleSystem laserHitPSPrefab;
	public float distance;
	public float damage;
	public float damageInterval;
	public float rotationSpeed;
	public float timeOn;
	public float timeOff;


	private List<LineRenderer> lasers = new List<LineRenderer>();
	private List<ParticleSystem> contactPS = new List<ParticleSystem>();
	private List<ParticleSystem> hitPS = new List<ParticleSystem>();
	private float hitTime;
	private float stateTime;
	private bool isOn;

	// Use this for initialization
	void Awake () 
	{
		isOn = true;
		//Get all lasers.
        foreach ( var line in GetComponentsInChildren<LineRenderer>())
		{
			lasers.Add (line);

			//Make sure of some default values.
			lasers[lasers.Count-1].useWorldSpace = false;
			lasers[lasers.Count-1].SetPosition (0, Vector3.zero);
		}

		//Create hitPS & contactPS for all lasers. Assign them to same index in their own list.
		for (int i = 0; i < lasers.Count; i++)
		{
			if (laserContactPSPrefab)
			{
				//Contact particles for constant hits
				ParticleSystem temp = Instantiate(laserContactPSPrefab, lasers[i].transform.position, lasers[i].transform.rotation);
				temp.transform.parent = lasers[i].transform;
				contactPS.Add(temp);
			}

			if (laserHitPSPrefab)
			{
				//Hit particles for damageable hits
				ParticleSystem temp = Instantiate(laserHitPSPrefab, lasers[i].transform.position, lasers[i].transform.rotation);
				temp.transform.parent = lasers[i].transform;
				hitPS.Add(temp);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		for (int i = 0; i < lasers.Count; i++)
		{
			if (isOn)
				lasers[i].gameObject.SetActive(true);
			else
			{
				lasers[i].gameObject.SetActive(false);
				continue;
			}

			//Lasers are positioned to shoot upwards, raycast is up as well.
			RaycastHit2D hit2D = Physics2D.Raycast(lasers[i].transform.position, lasers[i].transform.up, distance);

			//Set second linerenderer Y position to hitpoint or max distance.
			//Only Y position because line is in local space.
			if (hit2D.collider)
			{
				lasers[i].SetPosition(1, new Vector3(0, Vector3.Distance(hit2D.point, lasers[i].transform.position), 0));
				if (laserContactPSPrefab)
				{
					//Plays laser hit particles in hit position.
					contactPS[i].transform.position = hit2D.point;
					if (!contactPS[i].isPlaying)
						contactPS[i].Play();
				}
			}
			else
			{
				lasers[i].SetPosition(1, new Vector3(0, distance, 0));
				if (laserContactPSPrefab)
				{
					//Stop hit PS because no hits.
					contactPS[i].Stop();
				}
			}


			if (hitTime + damageInterval < Time.time && hit2D.collider)
			{
				//Player & other damageable collisions.
				IDamageable Idmg = hit2D.collider.GetComponent<IDamageable>();
				if (Idmg != null)
				{
					hitTime = Time.time;
					Idmg.GetHit(damage, hit2D.point);

					if (laserHitPSPrefab)
					{
						//Play hit particles
						hitPS[i].transform.position = hit2D.point;
						hitPS[i].Play();
					}
				}
			}
		}

		if (timeOff != 0 && timeOn != 0)
		{
			if (isOn)
			{
				if (stateTime + timeOn < Time.time)
				{
					isOn = false;
					stateTime = Time.time;
				}
			}
			else
			{
				if (stateTime + timeOff < Time.time)
				{
					isOn = true;
					stateTime = Time.time;
				}
			}
		}

		if (rotationSpeed != 0)
			transform.Rotate(new Vector3(0,0,rotationSpeed*Time.deltaTime), Space.Self);
	}
}
