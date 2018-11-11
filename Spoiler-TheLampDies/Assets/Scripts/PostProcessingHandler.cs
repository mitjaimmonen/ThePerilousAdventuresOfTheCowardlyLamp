using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingHandler : MonoBehaviour {

	Vector2 saturationMinMax = new Vector2(-100f, 10f);
	Vector2 vignetteMinMax = new Vector2(0.3f, 0.6f);
	Vector2 focusDistMinMax = new Vector2(1f, 10f);
	PostProcessVolume postProcessVolume;
	PostProcessProfile profile;
	ColorGrading col;
	Vignette vignette;
	DepthOfField dof;

	float currentFocusDistance;
	float currentSat;
	float currentVignette;
	
	

	// Use this for initialization
	void Start () {
		postProcessVolume = GetComponentInChildren<PostProcessVolume>();
		profile = postProcessVolume.profile;

		profile.TryGetSettings( out col);
		profile.TryGetSettings( out vignette);
		profile.TryGetSettings( out dof);

		//Max focus is default focus.
		focusDistMinMax.y = dof.focusDistance;
		currentFocusDistance = focusDistMinMax.y;

	}
	public void SetSaturationIntensity(float intensity)
	{
		//Lerp finds value between min and max with intensity ("time")
		Debug.Log(saturationMinMax);
		Debug.Log(intensity + ", " + currentSat);
		currentSat = Mathf.Lerp(saturationMinMax.x, saturationMinMax.y, Mathf.Clamp01(intensity));
	}
	public void SetVignetteIntensity (float intensity)
	{
		//Lerp finds value between min and max with intensity ("time")
		
		currentVignette = Mathf.Lerp(vignetteMinMax.x, vignetteMinMax.y, intensity = Mathf.Clamp01(intensity));
	}

	public void GamePaused(bool state)
	{
		if (state)
		{
			//If paused, focus distance is minimum to add blur.
			currentFocusDistance = focusDistMinMax.x;
		}
		else
		{
			//Unpaused game, focus distance back to default.
			currentFocusDistance = focusDistMinMax.y;
		}
	}

	private void Update()
	{
		float t = Time.deltaTime * 10f;

		//Lerp all values over time
		col.saturation.value = Mathf.Lerp(col.saturation.value, currentSat, t);
		vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, currentVignette, t);
		dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, currentFocusDistance, t);
		Debug.Log("currentdof: " + currentFocusDistance + " currentsat: " + currentSat + " currentvig: " + currentVignette);
	}
}
