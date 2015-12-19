using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum CURSOR_TYPE { Default, Crosshairs, Pointer};
public class GameManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public CURSOR_TYPE startingCursor = CURSOR_TYPE.Default;		// The default cursor to display
	public Text gameTime;											// Text object to display the game time
	public Text score;												// Text object of the game score
	public Text level;
	public Slider experience;
	public Texture2D crosshairCursor;								// Crosshair cursor
	public Texture2D pointerCursor;									// Pointer cursor
	public Texture2D defaultCursor;									// Default cursor
	public GameObject pauseMenu;									// Pause Menu canvas game object
	public GameObject gameOverMenu;									// Game Over canvas game object
	public GameObject levelCompleteMenu;

	public static GameManager Instance;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private CURSOR_TYPE _originalCursor;			// Stores the originally used cursor
	private bool _pause = false;					// Flag for when the game is paused
	private int _playerShots = 0;
	private int _playerHits = 0;
	private int _score = 0;
	private List<EnemyController> _enemies = new List<EnemyController>();
	private List<EnemyController> _enemiesKilled = new List<EnemyController>();

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
			Destroy(gameObject);
		}
		Instance = this;

		setCursor(startingCursor);

		_originalCursor = startingCursor;

		_updateExperience(0);
	}

	/**
	 * @private called once per frame. Used to capture key events for later
	 **/
	void Update() {
		if (Input.GetButtonDown("Cancel") && !isPaused()) { 
			pause();
		} else if (Input.GetButtonDown("Cancel") && isPaused()) { 
			resume();
		}
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		_updateGameTime();
	}

	/**
	 * @private updated the game timer
	 **/
	private void _updateGameTime() {
		if (gameTime != null) {
			string minutes = Mathf.Floor(Time.time / 60).ToString("00");
			string seconds = (Time.time % 60).ToString("00");

			gameTime.text = minutes + ":" + seconds;
		}
	}


	private void _updateScore(int scoreToAdd, List<float[]> damageList) {
		float totalModifier = 0f;

		// Loop over the damage list to see what the modifiers were
		foreach(float[] hit in damageList) {
			totalModifier += hit[0];
		}

		// Average out the modifiers
		totalModifier =  totalModifier / damageList.Count;

		// Multiple the score to add by the multiplier
		scoreToAdd = Mathf.RoundToInt((float)scoreToAdd * totalModifier);
		_score += scoreToAdd;
		score.text = _score.ToString();

		// Get the players location and then display a bonus text
		Transform player = PlayerManager.Instance.getTransform();
		FloatingTextManager.Instance.show(player, "+" + scoreToAdd.ToString(), Color.yellow);
	}

	private void _updateExperience(int experienceToAdd) {
		if (experience) {
			PlayerData _player = DataManager.Instance.getCurrentPlayerData();

			int _experience = _player.getExperience();
			int _newExperience = _experience += experienceToAdd;
			_player.setExperience(_newExperience);

			experience.maxValue = _player.getExperienceForNextLevel();
			experience.value = _newExperience;

			level.text = _player.getLevel().ToString();
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
		_pause = true;
		setCursor(CURSOR_TYPE.Default);
		Time.timeScale = 0;
		pauseMenu.SetActive(true);
	}

	/**
	 * @public called to un pause the game
	 **/
	public void resume() {
		_pause = false;
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

	/**
	 * @public called to see if the game is paused
	 **/
	public bool isPaused() {
		return _pause;
	}
	
	/**
	 * @public processes an enemy kill so that you know if the end of the level has been reached
	 **/
	public void processKill(List<float[]> damageList, EnemyController enemy) {
		// Keep track of all the enemies killed
		_enemiesKilled.Add(enemy);
		_playerHits += damageList.Count;

		_updateScore(enemy.killScore, damageList);
		_updateExperience(enemy.killScore);

		// If the level boss was killed then game over
		if (enemy.rank == ENEMY_RANK.Boss) {
			float killRatio = ((float)_enemiesKilled.Count / (float)_enemies.Count) * 100;
			float accuracy = ((float)_playerHits / (float)_playerShots) * 100;

			_pause = true;
			setCursor(CURSOR_TYPE.Default);
			Time.timeScale = 0;
			levelCompleteMenu.SetActive(true);

			DataManager.Instance.updateLevelData(Application.loadedLevelName, killRatio, Time.time, accuracy, _score);
		}
	}

	public void processShot() {
		_playerShots++;
	}

	/**
	 * @private called when enemies start to regester them as being part of the level
	 **/
	public void registerEnemy(EnemyController enemy) {
		_enemies.Add(enemy);
	}
}
