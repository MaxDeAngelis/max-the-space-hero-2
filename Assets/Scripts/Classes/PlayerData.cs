﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class PlayerData {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private int _experience;
	private int _level;
	private int _experienceForNextLevel;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public PlayerData() {
		_level = 1;
		_experience = 0;
		_experienceForNextLevel = 15;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			GETTERS													     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public int getLevel() {
		return _level;
	}

	public int getExperience() {
		return _experience;
	}

	public int getExperienceForNextLevel() {
		return _experienceForNextLevel;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     				SETTERS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void setExperience(int experience) {
		_experience = experience;

		if (_experience >= _experienceForNextLevel) {
			_experience -= _experienceForNextLevel;
			_level++;
		}
	}
}
