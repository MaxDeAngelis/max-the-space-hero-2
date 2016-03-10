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
	public void addToExperiance(int amount) {
		_experience += amount;

		if (_experience >= _experienceForNextLevel) {
			// Add to the level and reset the current XP
			_level++;
			_experience = _experience - _experienceForNextLevel;

			// Calculate what the next level will need take
			// Less Steep: 20x * Log10(x2) + 100
			// More Steep: Mathf.Pow((_level/0.1f), 2f);

			float newExperianceNeeded =  Mathf.RoundToInt((20 * _level) * Mathf.Log10(Mathf.Pow(_level, 2)) + 100);
			_experienceForNextLevel = Mathf.RoundToInt(newExperianceNeeded);
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
