using UnityEngine;
using System.Collections;

public class HealthPack : Powerup {

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTION												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void use() {
		// Get a reference to the players health controller
		PlayerHealth playerHealth = PlayerManager.Instance.getHealthController();

		// If the player is not at full health then use pack
		if (!playerHealth.isFull()) {
			// Add the health to the player
			playerHealth.add(bonus);

			// Show the increase in health
			Utilities.Instance.showFadeAwayText(PlayerManager.Instance.getTransform(), "+" + bonus.ToString(), Color.red);

			// Call base to use up the powerup
			base.use();
		}
	}
}
