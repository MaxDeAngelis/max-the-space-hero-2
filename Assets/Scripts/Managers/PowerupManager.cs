using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static PowerupManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/* ---- MANAGERS ---- */
	private EnergyManager _energyManager;

	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of PowerupManager!");
		}
		Instance = this;

		_energyManager = EnergyManager.Instance;
	}

	/**
	 * @private Called from the process function to apply a health pack bonus
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	IEnumerator _useHealthPowerup(PowerupController powerup, GameObject player) {
		// Get a reference to the players health controller
		HealthController healthController = player.GetComponent<HealthController>();

		// Store off the original max heath to reduce after duration finishes
		float originalMaxHealth = healthController.maximumHealth;

		// Add a buff to the health
		healthController.maximumHealth += powerup.bonus;
		healthController.health += powerup.bonus;

		yield return new WaitForSeconds(powerup.duration);

		// Set back the health once the timer finishes
		healthController.maximumHealth = originalMaxHealth;

		// If the user has too much health reduce back to max health
		if (healthController.health > healthController.maximumHealth) {
			healthController.health = healthController.maximumHealth;
		}

		// Update the display
		healthController.updateHealth();
	}

	/**
	 * @private Called from the process function to apply an energy bonus
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	void _useEnergyPowerup(PowerupController powerup, GameObject player) {
		// If the energy and bonus are higher than the maximum then just cap it else add
		if ((_energyManager.energy + powerup.bonus) > _energyManager.maximumEnergy) {
			_energyManager.energy = _energyManager.maximumEnergy;
		} else {
			_energyManager.energy += powerup.bonus;
		}
	}

	/**
	 * @private Called from the process function to apply a speed bonus
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	IEnumerator _useSpeedPowerup(PowerupController powerup, GameObject player) {
		// Get a handle on the player controller
		PlayerController playerController = player.GetComponent<PlayerController>();

		// Store off the original speed value to be able to revery
		float originalBoostSpeed = playerController.maximumVelocity;
		float originalMovementSpeed = playerController.movementSpeed;

		// Increse both movement values, running and boosting
		playerController.maximumVelocity += powerup.bonus;
		playerController.movementSpeed += (powerup.bonus/2);

		yield return new WaitForSeconds(powerup.duration);

		// Return to the original state
		playerController.maximumVelocity = originalBoostSpeed;
		playerController.movementSpeed = originalMovementSpeed;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Called to process a powerup that was picked up by the player
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	public void process(PowerupController powerup, GameObject player) {
		// Process the powerup that was picked up
		switch(powerup.type) {
			case PowerupController.Type.Health:
				StartCoroutine(_useHealthPowerup(powerup, player));
				break;
			case PowerupController.Type.Energy:
				_useEnergyPowerup(powerup, player);
				break;
			case PowerupController.Type.Speed:
				StartCoroutine(_useSpeedPowerup(powerup, player));
				break;
		}

		// Call the powerup to say it was used
		powerup.use();
	}
}
