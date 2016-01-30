using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : Health {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Text healthDisplay;				// UI Text component to display the percent of health left
	public Slider healthBar;				// Slider to display the remaining health

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	  PROTECTED FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @public Called to update any display of the health in the UI
	**/
	public override void updateHealth() {
		// Only update text in UI if it was configured to do so
		if (healthDisplay != null) {
			// Calculate the percentage of health left
			float healthPercent = Mathf.Round((health/maximumHealth) * 100);

			// Display the calculated string
			healthDisplay.text = health + "/" + maximumHealth;

			// Update health bar value
			healthBar.value = healthPercent;
		}
	}

	/**
	 * @protected Called when the current gameobject dies
	 **/
	protected override void die() {
		GameManager.Instance.gameOver();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Called on start of game object
	 **/
	public override void Start() {
		isPlayer = true;
		health = DataManager.Instance.getCurrentPlayerData().getHealth();


		updateHealth();

		base.Start();
	}
}
