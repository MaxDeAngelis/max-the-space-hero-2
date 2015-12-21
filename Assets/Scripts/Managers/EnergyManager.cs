using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public float maximumEnergy;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static EnergyManager Instance;
	public float energy = 400f;				// The maximum health of the unit
	public Text energyDisplay;				// Text display of the energy value
	public Slider energyBar;				// Bar that represents amount left
	public int regenerationRate = 30;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private int _energyGenerationFrameCount;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of PowerupManager!");
		}
		Instance = this;

		maximumEnergy = energy;
		_energyGenerationFrameCount = regenerationRate;
		_updateEnergy();
	}

	/**
	 * @private Called to update the display of the current enery
	 **/
	void _updateEnergy() {
		if (energyBar) {
			// Calculate the percentage of health left
			float energyPercent = Mathf.Round((energy/maximumEnergy) * 100);
				
			// Display the calculated string
			energyDisplay.text = energy + "/" + maximumEnergy;

			// Update energy bar size
			energyBar.value = energyPercent;
		}
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// If you do not have full energy and the counter is up then generate some energy
		if (_energyGenerationFrameCount <= 0 && energy < maximumEnergy) {
			energy++;
			_updateEnergy();
			_energyGenerationFrameCount = regenerationRate;
		} else {
			_energyGenerationFrameCount--;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Called to use some energy
	 **/
	public void useEnergy(float usedEnergy) {
		// If the current energy is more then the amount taken away then subtract otherwise set to 0
		if (energy > usedEnergy) {
			energy -= usedEnergy;
		} else {
			energy = 0;
		}

		// Update the energy display in the UI
		_updateEnergy();
	}

	/**
	 * @public sets the regeneration rate of energy
	 * 
	 * @param {Integer} regenRate - The amount of frames befor next point is regenerated
	 **/
	public void setRegenerationRate(int regenRate) {
		regenerationRate = regenRate;
	}
}
