using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class SoundMaster : MonoBehaviour {

	[FMODUnity.EventRef] public string backgroundMusic;
	[FMODUnity.EventRef] public string shootSE;
	[FMODUnity.EventRef] public string projectileBounceSE;
	[FMODUnity.EventRef] public string projectileDestroySE;
	[FMODUnity.EventRef] public string shardDestroySE;
	[FMODUnity.EventRef] public string shardHitSE;
	[FMODUnity.EventRef] public string laserHitSE;
	[FMODUnity.EventRef] public string playerDestroySE;
	[FMODUnity.EventRef] public string playerHitSE;
	[FMODUnity.EventRef] public string collectSE;
	[FMODUnity.EventRef] public string checkpointSE;
	
	private FMOD.Studio.EventInstance musicEI;
	private GameObject target;
	private Player player;
	private float health = 1f;
	private float healthLerp = 1f;
	private float oneShotTimer;
	private float ambienceCheckTimer;


	void Start()
	{
		musicEI = FMODUnity.RuntimeManager.CreateInstance(backgroundMusic);
		musicEI.start();

	}

	void SceneLoaded()
	{
		FMOD.Studio.PLAYBACK_STATE playbackState;
		musicEI.getPlaybackState(out playbackState);

		if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
			musicEI.start();

	}

	void Update()
	{
		musicEI.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position));

		if (GameMaster.Instance.GameState == GameState.game)
		{
			if (!player)
				player = GameMaster.Instance.Player;
			
			//Get health value between 0-1
			health = Mathf.Clamp01(player.CurrentHealth/player.maxHealth);
			//Lerp value for smooth effect.
			healthLerp = Mathf.Lerp(healthLerp, health, Time.deltaTime*3.5f);
			//Apply parameter to fmod. (Makes lowpass & pitch change effect when damage taken)
			musicEI.setParameterValue("PlayerHealth", healthLerp);
		}
		else
		{
			//In menu keep parameter at 1
			health = 1f;
			healthLerp = Mathf.Lerp(healthLerp, health, Time.deltaTime*2f);
			musicEI.setParameterValue("PlayerHealth", healthLerp);
		}


		//Every 5 seconds check that ambience is playing.
		if (ambienceCheckTimer + 5f < Time.time)
		{
			FMOD.Studio.PLAYBACK_STATE playbackState;
			musicEI.getPlaybackState(out playbackState);

			if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
				musicEI.start();
		}

	}



	public void PlayShoot(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(shootSE, pos);
	}
	public void PlayProjectileBounce(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(projectileBounceSE, pos);
	}
	public void PlayProjectileDestroy(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(projectileDestroySE, pos);
	}

	public void PlayShardDestroy(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(shardDestroySE, pos);
	}
	public void PlayShardHit(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(shardHitSE, pos);
	}
	public void PlayLaserHit(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(laserHitSE, pos);
	}

	public void PlayPlayerDestroy(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(playerDestroySE, pos);
	}
	public void PlayPlayerHit(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(playerHitSE, pos);
	}
	public void PlayCollect(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(collectSE, pos);
	}
	public void PlayCheckpoint(Vector2 pos)
	{
		if (oneShotTimer + 0.05f < Time.time)
			FMODUnity.RuntimeManager.PlayOneShot(checkpointSE, pos);
	}
}
