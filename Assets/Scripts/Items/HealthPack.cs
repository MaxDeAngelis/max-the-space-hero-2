using UnityEngine;
using System.Collections;

public class HealthPack : Powerup {

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTION												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to use the health pack. Handles only conditionally calling it and showing bonus text
	/// </summary>
	public override void use() {
		// Get a reference to the players health controller
		PlayerHealth playerHealth = PlayerManager.Instance.getHealthController();

		// If the player is not at full health then use pack
		if (!playerHealth.isFull()) {
			// Add the health to the player
			playerHealth.add(bonus);

			// Call base to use up the powerup
			base.use();
		}
	}
}
