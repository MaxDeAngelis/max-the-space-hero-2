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
	
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance) {
			DestroyImmediate(gameObject);
		} else {
			DontDestroyOnLoad(gameObject);
			Instance = this;

			GAME_DATA_LOCATION = Application.persistentDataPath + "/gameData.dat";

			//_updateGameData(new GameData());

			load();
		}
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
		return (GameData)_currentGameData;
	}

	/**
	 * @public called to update the current level data
	 **/
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

		// Once global object is updated save to game file
		save();
	}
}
