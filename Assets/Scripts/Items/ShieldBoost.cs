using UnityEngine;
using System.Collections;

public class ShieldBoost : Boost {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PROTECTED FUNCTION											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Initiates the energy boost
	/// </summary>
	protected override void initiate() {
		// Get a reference to the current shield
		Shield playerShield = PlayerManager.Instance.getShield();

		// Update the maximum shield strength and boost the current strength
		playerShield.setMaxShield(playerShield.getStrength() + bonus);
		playerShield.addStrength(bonus);

		// Call base version for core functionality
		base.initiate();
	}

	/// <summary>
	/// Extenstion point to be used for finishing the boost
	/// </summary>
	protected override void finish() {
		// Get a reference to the current shield
		Shield playerShield = PlayerManager.Instance.getShield();

		// Reset the maximum shield strength
		playerShield.setMaxShield(playerShield.getStrength() - bonus);

		// Call base version for core functionality
		base.finish();
	}
}
