using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

	This script keeps track of all preferences.
	Using separate script instead of accessing PlayerPrefs randomly, keeps it easy to handle.

 */

public static class PlayerPrefsManager {

	public static void SetLevel(int levelIndex)
	{
		PlayerPrefs.SetInt("Level", levelIndex);
	}

	public static int GetLevel()
	{
		if (!PlayerPrefs.HasKey("Level"))
			PlayerPrefs.SetInt("Level", 0);
		
		return PlayerPrefs.GetInt("Level");
}
}
