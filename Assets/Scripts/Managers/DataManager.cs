using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataManager : MonoBehaviour {	
	public static DataManager Instance;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTANTS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private string GAME_DATA_LOCATION;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private GameData _currentGameData;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/// <summary>
	/// Called when the game object wakes up
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of DataManager!");
		}

		Instance = this;

		GAME_DATA_LOCATION = Application.persistentDataPath + "/gameData.dat";

		load();
	}
	 
	/// <summary>
	/// Updated the game data file
	/// </summary>
	/// <param name="data">Data.</param>
	private void _updateGameData(GameData data) {
		// Get a handle on the formatter and the file itself
		BinaryFormatter _formatter = new BinaryFormatter();
		FileStream _file = File.Open(GAME_DATA_LOCATION, FileMode.Open);
		
		// Serialize and store then close the file
		_formatter.Serialize(_file, data);
		_file.Close();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Save the actual game data to save file
	/// </summary>
	public void save() {
		_updateGameData(_currentGameData);
	}
	
	/// <summary>
	/// Load the game from game file if it exists or create a new game object
	/// </summary>
	public void load() {
		// If the file exists
		if (File.Exists(GAME_DATA_LOCATION)) {
			// Get a handle on the file
			BinaryFormatter _formatter = new BinaryFormatter();
			FileStream _file = File.Open(GAME_DATA_LOCATION, FileMode.Open);
			
			// Read the data then set back in game land
			_currentGameData = (GameData)_formatter.Deserialize(_file);
			
			// Close the file
			_file.Close();
		} else {
			// Get a handle on the formatter and the file itself
			BinaryFormatter _formatter = new BinaryFormatter();
			FileStream _file = File.Create(GAME_DATA_LOCATION);

			// Create and store off the current game data
			_currentGameData = new GameData();

			// Serialize and store then close the file
			_formatter.Serialize(_file, _currentGameData);
			_file.Close();
		}

		// Backwards compatability check
		GameData newData = new GameData();
		if (_currentGameData.version() != newData.version()) {
			Debug.LogError("Game Data out of date! It has been automatically upgraded");
			_updateGameData(new GameData());
		}
	}

	/// <summary>
	/// Called to get the entire saved game data object
	/// </summary>
	/// <returns>The current game data object</returns>
	public GameData getCurrentGameData() {
		return _currentGameData;
	}

	/// <summary>
	/// Called to return only the player portion of the game data object
	/// </summary>
	/// <returns>The current player data</returns>
	public PlayerData getCurrentPlayerData() {
		return _currentGameData.getPlayerData();
	}

	/// <summary>
	/// Called to save off the current level data
	/// </summary>
	/// <returns>The level data.</returns>
	/// <param name="levelName">Level name.</param>
	/// <param name="ratio">Ratio.</param>
	/// <param name="time">Time.</param>
	/// <param name="accuracy">Accuracy.</param>
	/// <param name="score">Score.</param>
	public void updateLevelData(string levelName, float ratio, float time, float accuracy, int score) {
		// Loop until you find the current level and update the completion
		foreach(LevelData currentLevel in _currentGameData.getLevels()) {
			if (currentLevel.getName() == levelName) {
				currentLevel.setCompletion(ratio);
				currentLevel.setTime(time);
				currentLevel.setAccuracy(accuracy);
				currentLevel.setScore(score);
				break;
			}
		}
	}

	/// <summary>
	/// Called to update the player data with the new score
	/// </summary>
	/// <param name="score">The new score to add</param>
	public void updateScore(int score) {
		_currentGameData.getPlayerData().addToScore(score);
	}

	/// <summary>
	/// Called to update the player data with the new health
	/// </summary>
	/// <param name="score">The new score to add</param>
	public void updateHealth(int amount) {
		_currentGameData.getPlayerData().addToHealth(amount);
	}

	/// <summary>
	/// Called to update the player data with additional energy
	/// </summary>
	/// <param name="amount">The amount to add to the existing energy</param>
	public void updateEnergy(int amount) {
		_currentGameData.getPlayerData().addToEnergy(amount);
	}

	/// <summary>
	/// Called to update the player date with additional shield strength
	/// </summary>
	/// <param name="amount">The amount to add to the existing shield</param>
	public void updateShield(int amount) {
		_currentGameData.getPlayerData().addToShield(amount);
	}

	/// <summary>
	/// Called to update the player data with additional damage for the players weapon
	/// </summary>
	/// <param name="amount">The amount to add to the weapon damage</param>
	public void updateDamage(int amount) {
		_currentGameData.getPlayerData().addToDamage(amount);
	}
}
