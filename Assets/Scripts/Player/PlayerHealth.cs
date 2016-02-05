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
	/// <summary>
	/// Called when the current gameobject dies
	/// </summary>
	protected override void die() {
		SpecialEffectsManager.Instance.playExplosion(transform.position, deathSoundEffect);
		GameManager.Instance.gameOver();
	}

	/// <summary>
	/// Called to update any display of the health in the UI
	/// </summary>
	protected override void updateHealth() {
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

	/// <summary>
	/// Called on start of game object
	/// </summary>
	protected override void Start() {
		isPlayer = true;
		health = DataManager.Instance.getCurrentPlayerData().getHealth();

		updateHealth();

		base.Start();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Add the given amount of health to the player
	/// </summary>
	/// <param name="ammount">Amount to add</param>
	public void add(float amount) {
		// If the addition will be greater than the max health just cap
		if ((health + amount) > maximumHealth) {
			health = maximumHealth;
		} else {
			health += amount;
		}

		// Refresh display
		updateHealth();
	}

	/// <summary>
	/// Reset the Players health
	/// </summary>
	public override void reset() {
		Start();

		base.reset();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		   	FLAGS													     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to see if the players health is full or not
	/// </summary>
	/// <returns><c>true</c>, if the players health is full, <c>false</c> otherwise</returns>
	public bool isFull() {
		return (health >= maximumHealth);
	}
}
