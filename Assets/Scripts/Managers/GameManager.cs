using UnityEngine;
using System.Collections;

public enum CURSOR_TYPE { Default, Crosshairs, Pointer};

public class GameManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public CURSOR_TYPE startingCursor = CURSOR_TYPE.Default;
	public Texture2D crosshairCursor;
	public Texture2D pointerCursor;
	public Texture2D defaultCursor;
	public GameObject pauseMenu;
	public GameObject gameOverMenu;

	public static GameManager Instance;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private CURSOR_TYPE _originalCursor;
	private bool _pause = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of GameManager!");
		}
		Instance = this;

		setCursor(startingCursor);
	}

	/**
	 * @private called once per frame. Used to capture key events for later
	 **/
	void Update() {
		if (Input.GetButtonDown("Cancel")) { 
			_pause = true;
		}
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		if (_pause) {
			_pause = false;
			pause();
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	 * @public called to load a specified scene
	 * 
	 * @param String levelName - the name of the scene to load
	 **/
	public void loadLevel(string levelName) {
		Application.LoadLevel(levelName);
	}

	/**
	 * @public called to quit the application
	 **/
	public void quit() {
		Application.Quit();
	}

	/**
	 * @public called to change the cursor that is being used
	 * 
	 * @param CURSOR_TYPE type - the type of cursor to use
	 **/
	public void setCursor(CURSOR_TYPE type) {
		Texture2D _cursorTexture = null;
		Vector2 _cursorOffset = Vector2.zero;

		// Figure out what cursor to use
		switch(type) {
			case CURSOR_TYPE.Crosshairs:
				_cursorTexture = crosshairCursor;
				_cursorOffset = new Vector2(13f, 13f);
				break;
			case CURSOR_TYPE.Pointer:
				_cursorTexture = pointerCursor;
				_cursorOffset = new Vector2(12f, 3f);
				break;
			case CURSOR_TYPE.Default:
				_cursorTexture = defaultCursor;
				break;
		}

		// Set the cursor texture
		Cursor.SetCursor(_cursorTexture, _cursorOffset, CursorMode.Auto);
	}

	/**
	 * @public called to reset the cursor back to the original
	 **/
	public void resetCursor() {
		setCursor(_originalCursor);
	}

	/**
	 * @public called to pause the game 
	 **/
	public void pause() {
		setCursor(CURSOR_TYPE.Default);
		Time.timeScale = 0;
		pauseMenu.SetActive(true);
	}

	/**
	 * @public called to un pause the game
	 **/
	public void resume() {
		resetCursor();
		Time.timeScale = 1;
		pauseMenu.SetActive(false);
	}

	/**
	 * @public called when the player dies and the game is over
	 **/
	public void gameOver() {
		setCursor(CURSOR_TYPE.Default);
		gameOverMenu.SetActive(true);
	}
}
