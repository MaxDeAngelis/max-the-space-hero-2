﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[Header("HUD Display Settings")]
	public Canvas hudCanvas;
	public GameObject miniMap;
	public Text gameTime;											// Text object to display the game time
	public Text score;												// Text object of the game score
	public Text level;
	public Slider experience;

	[Header("Level Complete Objects")]
	public Text levelScore;
	public Text levelCompletion;
	public Text levelAccuracy;
	public Text levelTime;

	public static GameManager Instance;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _levelStartTime;
	private int _playerShots = 0;
	private int _playerHits = 0;
	private int _score = 0;
	private List<EnemyController> _enemies = new List<EnemyController>();
	private List<EnemyController> _enemiesKilled = new List<EnemyController>();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of GameManager!");
			Destroy(gameObject);
		}
		Instance = this;

		/* -- INIT VARIABLES -- */
		_levelStartTime = Time.time;

		/* -- INIT DISPLAY -- */
		_updateExperience(0);
		_updateScore(0);
	}

	/// <summary>
	/// Called on level load to set the renderer camera of the HUD
	/// </summary>
	/// <param name="level">The level number</param>
	void OnLevelWasLoaded(int level) {
		// Make sure HUD is using the main camera
		if (hudCanvas) {
			hudCanvas.worldCamera = Camera.main;
		}
			
		// Reset variables
		_levelStartTime = Time.time;
		_playerShots = 0;
		_playerHits = 0;
		_score = 0;
	}
		
	/// <summary>
	/// Called 60times per second fixed, handles all processing
	/// </summary>
	void FixedUpdate() {
		_updateGameTime();
	}
		
	/// <summary>
	/// updated the game timer
	/// </summary>
	private void _updateGameTime() {
		if (gameTime != null) {
			float _newTime = Time.time - _levelStartTime;
			string minutes = Mathf.Floor(_newTime / 60).ToString("00");
			string seconds = (_newTime % 60).ToString("00");

			gameTime.text = minutes + ":" + seconds;
		}
	}
		
	/// <summary>
	/// Calculates the new score to add based on damageList given
	/// </summary>
	/// <param name="scoreToAdd">The amount to add to the score</param>
	/// <param name="damageList">The list of damage done to the enemy used to adjust the score</param>
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

		// After calculating based on modifier then call base version
		_updateScore(scoreToAdd);

		// Get the players location and then display a bonus text
		Transform player = PlayerManager.Instance.getTransform();
		Utilities.Instance.showFadeAwayText(player, "+" + scoreToAdd.ToString(), Color.yellow);
	}
		
	/// <summary>
	/// Called to update the text of the score that is displayed in the hud
	/// </summary>
	/// <param name="scoreToAdd">The amount to add to the score</param>
	private void _updateScore(int scoreToAdd) {
		// If there is a score text object then populate it
		if (score) {
			_score += scoreToAdd;

			// Calculate level score plus overall score for display
			int scoreToDisplay = DataManager.Instance.getCurrentPlayerData().getScore() + _score;

			// Display the combined score
			score.text = scoreToDisplay.ToString();
		}
	}
		
	/// <summary>
	/// Called to update the experience bar in the HUD
	/// </summary>
	/// <param name="experienceToAdd">The experience to add to the xp bar</param>
	private void _updateExperience(int experienceToAdd) {
		if (experience) {
			DataManager.Instance.getCurrentPlayerData().addToExperiance(experienceToAdd);

			experience.value = DataManager.Instance.getCurrentPlayerData().getExperience();
			experience.maxValue = DataManager.Instance.getCurrentPlayerData().getExperienceForNextLevel();

			level.text = DataManager.Instance.getCurrentPlayerData().getLevel().ToString();
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// called to load a specified scene
	/// </summary>
	/// <param name="levelName">the name of the scene to load</param>
	public void loadLevel(string levelName) {
		MenuManager.Instance.hideMenu();

		if (levelName == "Splash") {
			DontDestroy.Instance.Destroy();
		}

		SceneManager.LoadScene(levelName);
	}
		
	/// <summary>
	/// called to quit the application
	/// </summary>
	public void quit() {
		Application.Quit();
	}		

	/// <summary>
	/// Called when the player dies and the game is over
	/// </summary>
	public void gameOver() {
		PlayerManager.Instance.getTransform().gameObject.SetActive(false);

		MenuManager.Instance.showMenu(MENU_TYPE.GameOver);
	}

	/// <summary>
	/// Processes an enemy kill so that you know if the end of the level has been reached
	/// </summary>
	/// <param name="damageList">The list of damage information about the enemy killed</param>
	/// <param name="enemy">The controller of the killed enemy</param></param>
	public void processKill(List<float[]> damageList, EnemyController enemy) {
		// Keep track of all the enemies killed
		_enemiesKilled.Add(enemy);
		_playerHits += damageList.Count;

		_updateScore(enemy.killScore, damageList);
		_updateExperience(enemy.killScore);

		// If the level boss was killed then game over
		if (enemy.rank == ENEMY_RANK.Boss) {
			// Calculate the kill ratio and the accuracy stats for the level
			float killRatio = ((float)_enemiesKilled.Count / (float)_enemies.Count) * 100;
			float accuracy = ((float)_playerHits / (float)_playerShots) * 100;

			// Update text of level complete screen
			levelScore.text = _score.ToString();
			levelCompletion.text = Mathf.Round(killRatio).ToString() + "%";
			levelAccuracy.text = Mathf.Round(accuracy).ToString() + "%";
			levelTime.text = gameTime.text;

			// Play the level complete sound
			SpecialEffectsManager.Instance.playLevelComplete();

			// Display the actual level complete menue
			MenuManager.Instance.showMenu(MENU_TYPE.LevelComplete);

			// Save the level and the player data
			DataManager.Instance.updateLevelData(SceneManager.GetActiveScene().name, killRatio, (Time.time - _levelStartTime), accuracy, _score);
			DataManager.Instance.updateScore(_score);
			DataManager.Instance.save();
		}
	}

	/// <summary>
	/// Called to increment the shot counter to keep track of accuracy
	/// </summary>
	public void processShot() {
		_playerShots++;
	}
		
	/// <summary>
	/// Called when enemies start to regester them as being part of the level
	/// </summary>
	/// <param name="enemy">The enemy controller of the enemy to register</param>
	public void registerEnemy(EnemyController enemy) {
		_enemies.Add(enemy);
	}

	/// <summary>
	/// Set the state of the mini map
	/// </summary>
	/// <param name="state">If set to <c>true</c> the mini map will be set to active. Otherwise it is hidden</param>
	public void setMiniMapState(bool state) {
		if (miniMap) {
			miniMap.SetActive(state);
		}
	}

	/// <summary>
	/// Called to refresh the HUD
	/// </summary>
	public void refreshHUD() {
		_updateExperience(0);
		_updateScore(0);


		EnergyManager.Instance.reset();
		PlayerManager.Instance.getHealthController().reset();
		PlayerManager.Instance.getShield().reset();
	}
}
