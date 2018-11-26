using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*****************
INFO
Master controller which has references to other classes and can be called from anywhere.
Decides when updates get called for some classes.

 ******************/

public enum GameState
{
	menu,
	game
}

public class GameMaster : MonoBehaviour {

	#region Singleton

	private static GameMaster _instance;
    public static GameMaster Instance
    {
        get
        {			
            return _instance;
        }
	}
	
	#endregion


	#region  Getters & Setters

	public GameState GameState
	{
		get
		{
			if (CurrentSceneData)
				return CurrentSceneData.gameState;
			else
				return GameState.game;
		}
	}

	private int levelNumber;
	public int LevelNumber
	{
		get {return levelNumber; }
		set { levelNumber = value; }
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
				{
					gameCanvas = temp.GetComponent<GameCanvas>();
				}
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
				var temp = Camera.main;
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

	private SceneData currentSceneData;
	public SceneData CurrentSceneData
	{
		get
		{
			if (!currentSceneData)
			{
				var temp = GameObject.FindGameObjectWithTag("SceneData");
				if (temp)
					currentSceneData = temp.GetComponent<SceneData>();
			}
			return currentSceneData;
		}
	}

	private SoundMaster soundMaster;
	public SoundMaster SoundMaster
	{
		get 
		{
			if (!soundMaster)
			{
				soundMaster = GetComponent<SoundMaster>();
			}
			return soundMaster;
		}
	}


	private bool isPaused;
	public bool IsPaused
	{
		get { return isPaused; }
		set 
		{
			isPaused = value;
			if (PostProcessingHandler)
				PostProcessingHandler.GamePaused(isPaused);
			if (GameCanvas)
			{
				GameCanvas.PauseMenuCanvas.GamePaused(isPaused, IsFinished ? PauseType.won : PauseType.paused);
				GameCanvas.HudCanvas.GamePaused(isPaused);
			}

			if (isPaused)
				Time.timeScale = 0;
			else
				Time.timeScale = 1f;
		}
	}

	private bool isFinished;
	public bool IsFinished
	{
		get { return isFinished; }
		set
		{
			isFinished = value;
			if (isFinished)
				IsPaused = value; //Game pauses if finished.
		}
	}

	#endregion
	
	// Use this for initialization
	private void Awake () {
		
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

	private void Initialize()		//Called on sceneload & awake.
	{
		//Initialization according to scene.
		if (GameState == GameState.game)
		{
			LevelNumber = CurrentSceneData.levelNumber;
			IsFinished = false;
			IsPaused = false;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		
        Initialize();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (GameState == GameState.game)
		{
			HandleInputs();
	
			if (!IsPaused)
			{
				//Call updates in other scripts if they should be called after each other.
				Player.MasterUpdate();
			}
		}
	}

	private void HandleInputs()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!IsFinished)
				IsPaused = !IsPaused;
		}
	}

	public string GetCurrentSceneName()
	{
		return SceneManager.GetActiveScene().name;
	}

	public bool HasScene(string sceneName)
	{
		return Application.CanStreamedLevelBeLoaded(sceneName);
	}


	public void LoadLevel(string sceneName)
	{
		if (HasScene(sceneName))
		{
			SceneManager.LoadScene(sceneName);
		}
	}

	public void NextLevel()
	{
		++LevelNumber;
        if (HasScene("Level" + LevelNumber))
		    SceneManager.LoadScene("Level" + LevelNumber);
        else
            SceneManager.LoadScene("MainMenu");
	}

	public void EndLevel()
	{
		IsFinished = true;
		IsPaused = true;

		PlayerPrefsManager.SetLevel(Mathf.Max(PlayerPrefs.GetInt("Level"), LevelNumber+1));
		gameCanvas.HudCanvas.GamePaused(IsPaused);
		gameCanvas.PauseMenuCanvas.GamePaused(IsFinished, PauseType.won);
		
	}
}
