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
	IEnumerator _useHealthPowerup(Powerup powerup, GameObject player) {
		// Call the powerup to say it was used
		powerup.use();

		// Get a reference to the players health controller
		PlayerHealth healthController = player.GetComponent<PlayerHealth>();

		// Store off the original max heath to reduce after duration finishes
		float originalMaxHealth = healthController.maximumHealth;

		// Add a buff to the health
		healthController.maximumHealth += powerup.bonus;
		healthController.health += powerup.bonus;

		// Update the display
		healthController.updateHealth();

		// Show the increase in health
		FadeAwayTextManager.Instance.show(player.transform, "+" + powerup.bonus.ToString(), Color.red);

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
	void _useEnergyPowerup(Powerup powerup, GameObject player) {
		// Only use the powerup if you need it
		if (_energyManager.energy < _energyManager.maximumEnergy) {
			// If the energy and bonus are higher than the maximum then adjust the bonus
			if ((_energyManager.energy + powerup.bonus) > _energyManager.maximumEnergy) {
				powerup.bonus = _energyManager.maximumEnergy - _energyManager.energy;
			}

			// Apply the bonus
			_energyManager.energy += powerup.bonus;

			// Show the increase in energy
			FadeAwayTextManager.Instance.show(player.transform, "+" + powerup.bonus.ToString(), Color.blue);

			// Call the powerup to say it was used
			powerup.use();
		}
	}


	/**
	 * @private Called from the process function to apply a speed bonus
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	IEnumerator _useSpeedPowerup(Powerup powerup, GameObject player) {
		/* TODO: Should not use maximum velocity from the player */
		// Call the powerup to say it was used
		powerup.use();

		// Get a handle on the player controller
		PlayerController playerController = player.GetComponent<PlayerController>();

		// Store off the original speed value to be able to revery
		//float originalBoostSpeed = playerController.maximumVelocity;
		float originalMovementSpeed = playerController.movementSpeed;

		// Increse both movement values, running and boosting
		//playerController.maximumVelocity += powerup.bonus;
		playerController.movementSpeed += (powerup.bonus/2);

		yield return new WaitForSeconds(powerup.duration);

		// Return to the original state
		//playerController.maximumVelocity = originalBoostSpeed;
		playerController.movementSpeed = originalMovementSpeed;

	}

	/**
	 * @private Called from the process function to apply a speed bonus
	 * 
	 * @param $PowerupController$ powerup - The controller of the powerup to apply
	 * @param $GameObject$ player - The game object of the player
	 **/
	IEnumerator _useShieldPowerup(Powerup powerup, GameObject player) {
		yield return new WaitForSeconds(powerup.duration);
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
	public void process(Powerup powerup, GameObject player) {
		// Process the powerup that was picked up
		switch(powerup.type) {
			case POWERUP_TYPE.Health:
				StartCoroutine(_useHealthPowerup(powerup, player));
				break;
			case POWERUP_TYPE.Energy:
				_useEnergyPowerup(powerup, player);
				break;
			case POWERUP_TYPE.Shield:
				StartCoroutine(_useShieldPowerup(powerup, player));
				break;
			case POWERUP_TYPE.Speed:
				StartCoroutine(_useSpeedPowerup(powerup, player));
				break;
		}
	}
}
