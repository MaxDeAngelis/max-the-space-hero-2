using UnityEngine;

public class MenuManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[Header("Menu Objects")]
	public GameObject pauseMenu;									// Pause Menu canvas game object
	public GameObject gameOverMenu;									// Game Over canvas game object
	public GameObject levelCompleteMenu;



	public static MenuManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private CURSOR_TYPE _originalCursor;			// Stores the originally used cursor
	private bool _pause = false;					// Flag for when the game is paused
	private GameObject _activeMenu;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of MenuManager!");
			Destroy(gameObject);
		}
		Instance = this;

		/* -- INIT VARIABLES -- */

		/* -- INIT DISPLAY -- */
	}
		
	/// <summary>
	/// called once per frame. Used to capture key events for later
	/// </summary>
	void Update() {
		if (Input.GetButtonDown("Cancel") && !isPaused()) { 
			showMenu(pauseMenu);
		} else if (Input.GetButtonDown("Cancel") && isPaused()) { 
			hideMenu();
		}
	}
		
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to show a given menu. Handles pausing the game and setting correct flags
	/// </summary>
	/// <param name="menu">The menu object to show</param>
	public void showMenu(GameObject menu) {
		pause();

		_activeMenu = menu;
		_activeMenu.SetActive(true);
	}

	/// <summary>
	/// Called to hid what ever menu is currently showing
	/// </summary>
	public void hideMenu() {
		resume();

		if (_activeMenu) {
			_activeMenu.SetActive(false);
			_activeMenu = null;
		}
	}

	/// <summary>
	/// called to pause the game 
	/// </summary>
	public void pause() {
		_pause = true;
		//setCursor(CURSOR_TYPE.Default);
		Time.timeScale = 0;
	}

	/// <summary>
	/// Called to un pause the game
	/// </summary>
	public void resume() {
		_pause = false;
		//resetCursor();
		Time.timeScale = 1;
	}

	/// <summary>
	/// Called to see if the game is paused
	/// </summary>
	/// <returns><c>true</c>, if paused was ised, <c>false</c> otherwise.</returns>
	public bool isPaused() {
		return _pause;
	}
}
