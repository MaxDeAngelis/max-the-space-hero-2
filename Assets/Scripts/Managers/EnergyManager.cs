using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour {
	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	public static EnergyManager Instance;
	public Text energyDisplay;						// Text display of the energy value
	public Slider energyBar;						// Bar that represents amount left
	public int regenerationRate = 30;

	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	private float _energy;							// The current energy of the player
	private float _maxEnergy;						// The maximum energy allowed for the player
	private int _energyGenerationFrameCount;		// Rate to regen the energy
	
	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

	/// <summary>
	/// Called when the game object wakes up
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of PowerupManager!");
		}
		Instance = this;

		/* INIT VARIABLES */
		_energy = DataManager.Instance.getCurrentPlayerData().getEnergy();
		_maxEnergy = _energy;
		_energyGenerationFrameCount = regenerationRate;

		/* INIT ENERGY DISPLAY */
		_updateEnergy();
	}

	/// <summary>
	/// Called to update the display of the current enery
	/// </summary>
	void _updateEnergy() {
		if (energyBar) {
			// Calculate the percentage of health left
			float energyPercent = Mathf.Round((_energy/_maxEnergy) * 100);
				
			// Display the calculated string
			energyDisplay.text = _energy + "/" + _maxEnergy;

			// Update energy bar size
			energyBar.value = energyPercent;
		}
	}

	/// <summary>
	/// Called 60times per second fixed, handles all processing
	/// </summary>
	void FixedUpdate() {
		// If you do not have full energy and the counter is up then generate some energy
		if (_energyGenerationFrameCount <= 0 && _energy < _maxEnergy) {
			_energy++;
			_updateEnergy();
			_energyGenerationFrameCount = regenerationRate;
		} else {
			_energyGenerationFrameCount--;
		}
	}


	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

	/// <summary>
	/// Called to add energy to the player
	/// </summary>
	/// <param name="energyToAdd">The amount of energy to add</param>
	public void addEnergy(float energyToAdd) {
		// If the energy and bonus are higher than the maximum then adjust the bonus
		if ((_energy + energyToAdd) >= _maxEnergy) {
			_energy = _maxEnergy;
		} else {
			_energy += energyToAdd;
		}
	}

	/// <summary>
	/// Called to use some energy
	/// </summary>
	/// <param name="usedEnergy">The amount of energy to use</param>
	public void useEnergy(float usedEnergy) {
		// If the current energy is more then the amount taken away then subtract otherwise set to 0
		if (_energy > usedEnergy) {
			_energy -= usedEnergy;
		} else {
			_energy = 0;
		}

		// Update the energy display in the UI
		_updateEnergy();
	}
		
	/// <summary>
	/// Sets the regeneration rate of energy
	/// </summary>
	/// <param name="regenRate">The regen rate to set</param>
	public void setRegenerationRate(int regenRate) {
		regenerationRate = regenRate;
	}

	/// <summary>
	/// Get the remaining energy that the player has
	/// </summary>
	/// <returns>The amount of energy left</returns>
	public float getEnergy() {
		return _energy;
	}

	/// <summary>
	/// Gets the maximum allowed energy
	/// </summary>
	/// <returns>The maximum allowed energy cap</returns>
	public float getMaxEnergy() {
		return _maxEnergy;
	}
}
