﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public float maximumEnergy;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float energy = 400f;				// The maximum health of the unit
	public Text energyDisplay;				// Text display of the energy value
	public Slider energyBar;				// Bar that represents amount left

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private int _energyGeneration = 15;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on awake of the game object to init variables
	 **/
	void Awake () {
		maximumEnergy = energy;
		_updateEnergy();
	}

	/**
	 * @private Called to update the display of the current enery
	 **/
	void _updateEnergy() {
		// Calculate the percentage of health left
		float energyPercent = Mathf.Round((energy/maximumEnergy) * 100);
			
		// Display the calculated string
		energyDisplay.text = energy + "/" + maximumEnergy;

		// Update energy bar size
		energyBar.value = energyPercent;
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// If you do not have full energy and the counter is up then generate some energy
		if (_energyGeneration <= 0 && energy < maximumEnergy) {
			energy++;
			_updateEnergy();
			_energyGeneration = 15;
		} else {
			_energyGeneration--;
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
}
