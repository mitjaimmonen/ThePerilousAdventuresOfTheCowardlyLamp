using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


	private static GameController _instance;
    public static GameController Instance
    {
        get
        {
            return _instance;
        }
	}


	private Player player;
	public Player Player
	{
		get
		{
			if (!player)
			{
				var temp = GameObject.FindGameObjectWithTag("Player");
				if (temp)
					player = temp.GetComponent<Player>();
			}
			return player;
		}
	}

	// Use this for initialization
	void Awake () {
		
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1)
        {
            Destroy(this.gameObject);
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

		Initialize();
	}

	void Initialize()		//Called on sceneload & awake.
	{
		
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
        Initialize();
        
}
	
	// Update is called once per frame
	void Update () {
		
	}
}
