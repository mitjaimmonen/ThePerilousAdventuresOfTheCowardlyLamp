using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingHandler : MonoBehaviour {

	Vector2 saturationMinMax = new Vector2(-100f, 10f);
	Vector2 vignetteMinMax = new Vector2(0.3f, 0.6f);
	PostProcessVolume postProcessVolume;
	PostProcessProfile profile;
	ColorGrading col;
	Vignette vignette;

	float currentSat;
	float currentVignette;
	

	// Use this for initialization
	void Start () {
		postProcessVolume = GetComponentInChildren<PostProcessVolume>();
		profile = postProcessVolume.profile;

		profile.TryGetSettings( out col);
		profile.TryGetSettings(out vignette);

	}
	public void SetSaturationIntensity(float intensity)
	{
		currentSat = Mathf.Lerp(saturationMinMax.x, saturationMinMax.y, intensity);
	}
	public void SetVignetteIntensity (float intensity)
	{
		currentVignette = Mathf.Lerp(vignetteMinMax.x, vignetteMinMax.y, intensity);
	}

	private void Update()
	{
		col.saturation.value = Mathf.Lerp(col.saturation.value, currentSat, Time.deltaTime*10f);
		vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, currentVignette, Time.deltaTime*10f);
	}
}
