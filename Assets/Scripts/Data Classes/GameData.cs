using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class GameData {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private int _version;
	private List<LevelData> _levels;
	private PlayerData _player;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public GameData() {
		_version = 1; // Imcrementing this will force a rebuild in game and all game data will be lost

		_levels = new List<LevelData>();
		_levels.Add(new LevelData("Level_1", "Moon Surface"));
		_levels.Add(new LevelData("Level_2", "Space Base"));
		_levels.Add(new LevelData("Level_3", "Surface"));
		_levels.Add(new LevelData("Level_4", "Cargo Bay"));
		_levels.Add(new LevelData("Level_5", "Escape"));

		_player = new PlayerData();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			GETTERS													     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public int version() {
		return _version;
	}

	public void storeLevelData(LevelData levelData) {
		_levels.Add(levelData);
	}

	public List<LevelData> getLevels() {
		return _levels;
	}

	public PlayerData getPlayerData() {
		return _player;
	}
}
