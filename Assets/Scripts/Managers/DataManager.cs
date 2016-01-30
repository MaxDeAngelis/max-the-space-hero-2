﻿using UnityEngine;
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

		//_updateGameData(new GameData());

		load();
	}
	 
	/**
	 * @private updated the game data file
	 **/
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
	
	/**
	 * @public handles saving the game
	 **/
	public void save() {
		_updateGameData(_currentGameData);
	}
	
	/**
	 * @public handles loading the game
	 **/
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
	}

	/**
	 * @public getter for the current game data object
	 **/
	public GameData getCurrentGameData() {
		return _currentGameData;
	}

	/**
	 * @public getter for the current player data object
	 **/
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

	public void updatePlayerData(int score) {
		_currentGameData.getPlayerData().addToScore(score);
	}
}
