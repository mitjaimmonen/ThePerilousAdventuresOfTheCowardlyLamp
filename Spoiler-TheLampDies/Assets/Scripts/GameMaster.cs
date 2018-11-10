using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*****************
INFO
Master controller which has references to other classes and can be called from anywhere.
Decides when updates get called for some classes.

 ******************/

public class GameMaster : MonoBehaviour {


	private static GameMaster _instance;
    public static GameMaster Instance
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

	private GameCanvas gameCanvas;
	public GameCanvas GameCanvas
	{
		get
		{
			if (!gameCanvas)
			{
				var temp = GameObject.FindGameObjectWithTag("MainCanvas");
				if (temp)
					gameCanvas = temp.GetComponent<GameCanvas>();
			}
			return gameCanvas;
		}
	}

	private CameraHandler cameraHandler;
	public CameraHandler CameraHandler
	{
		get
		{
			if (!cameraHandler)
			{
				var temp = GameObject.FindGameObjectWithTag("MainCamera");
				if (temp)
					cameraHandler = temp.GetComponentInParent<CameraHandler>();
			}
			return cameraHandler;
		}
	}

	private PostProcessingHandler postProcessingHandler;
	public PostProcessingHandler PostProcessingHandler
	{
		get 
		{
			if (!postProcessingHandler)
			{
				postProcessingHandler = CameraHandler.GetComponent<PostProcessingHandler>();
			}
			return postProcessingHandler;
		}
	}

	// Use this for initialization
	void Awake () {
		
        if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);

			SceneManager.sceneLoaded += OnSceneLoaded;

			Initialize();
		}
	}

	void Initialize()		//Called on sceneload & awake.
	{
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
        Initialize();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		//Call updates in other scripts if they should be called after each other.
		Player.MasterUpdate();
	}
}
