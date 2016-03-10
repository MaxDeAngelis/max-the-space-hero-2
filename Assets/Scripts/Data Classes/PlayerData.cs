using UnityEngine;
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
	private int _experienceForNextLevel;
	private int _level;
	private int _health;
	private int _shield;
	private int _score;
	private int _damage;
	private float _energy;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public PlayerData() {
		_level = 1;
		_experience = 0;
		_experienceForNextLevel = 15;
		_health = 50;
		_energy = 250f;
		_shield = 10;
		_score = 0;
		_damage = 10;
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

	public int getHealth() {
		return _health;
	}

	public float getEnergy() {
		return _energy;
	}

	public int getScore() {
		return _score;
	}

	public int getShield() {
		return _shield;
	}

	public int getDamage() {
		return _damage;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     				SETTERS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void setExperience(int experience) {
		_experience = experience;

		if (_experience >= _experienceForNextLevel) {
			_experience = 0;
			_level++;
		}
	}

	public void addToScore(int amount) {
		_score += amount;
	}

	public void addToHealth(int amount) {
		_health += amount;
	}

	public void addToEnergy(int amount) {
		_energy += amount;
	}

	public void addToShield(int amount) {
		_shield += amount;
	}

	public void addToDamage(int amount) {
		_damage += amount;
	}
}
