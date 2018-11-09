using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour {

	private HudCanvas hudCanvas;
	private PauseMenuCanvas pauseMenuCanvas;

	public HudCanvas HudCanvas
	{
		get 
		{ 
			if (!hudCanvas)
			{
				hudCanvas = GetComponentInChildren<HudCanvas>();
			}
			return hudCanvas;
		}
	}
	public PauseMenuCanvas PauseMenuCanvas
	{
		get 
		{ 
			if (!pauseMenuCanvas)
			{
				pauseMenuCanvas = GetComponentInChildren<PauseMenuCanvas>();
			}
			return pauseMenuCanvas;
		}
	}
}
