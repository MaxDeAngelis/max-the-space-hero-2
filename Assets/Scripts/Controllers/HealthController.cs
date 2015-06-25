using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float health = 400f;			// The maximum health of the unit
	public Text healthDisplay;			// UI Text component to display the percent of health left
	public bool isPlayer = false;		// Flag for if the unit is the player

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _originalHealth;		// Original maximum health of the unit

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on awake of the game object to init variables
	 **/
	void Awake () {
		_originalHealth = health;
		_updateHealth();
	}

	/**
	 * @private Collider handler that is triggered when another collider interacts with this game object
	 * 
	 * @param $Collider2D$ otherCollider - The collider that is interacting with this game object
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// Define damage to be taken
		float damage = 0f;

		// Try and get the Weapon Controller off the colider to see if you are being hit with a weapon
		WeaponController weapon = otherCollider.gameObject.GetComponent<WeaponController>();
		ProjectileController projectile = otherCollider.gameObject.GetComponent<ProjectileController>();

		/* ---- WEAPON DAMAGE ---- */
		if (weapon != null && weapon.isActive == true && weapon.isPlayer != isPlayer) {
			// Set the damage for the weapon
			damage = weapon.damage;

			// When the weapon makes contact then set to inactive so you only get one hit per swing
			weapon.isActive = false;

			// Damage the actual weapon
			weapon.durability -= weapon.durabilityLossPerAttack;
			if(weapon.durability <= 0) {
				weapon.broken();
			}
		}

		/* ---- PROJECTILE DAMAGE ---- */
		if (projectile != null && projectile.isPlayer != isPlayer) {
			// Set the damage for the projectile
			damage = projectile.damage;

			// Fire the hit function for the projectile
			projectile.hit();
		}

		// Deduct the dame that the weapon does from your health
		health -= damage;
		
		// Increment the UI display of health
		_updateHealth();
		
		// If your health drops below 0 die
		if (health <= 0) {
			_die();
		}
	}

	/**
	 * @private Called when the current gameobject dies
	 **/
	void _die() {
		Destroy(gameObject);
	}

	/**
	 * @private Called to update any display of the health in the UI
	 **/
	void _updateHealth() {
		// Only update text in UI if it was configured to do so
		if (healthDisplay != null) {
			// Calculate the percentage of health left
			float healthPercent = Mathf.Round((health/_originalHealth) * 100);

			// Display the calculated string
			healthDisplay.text = healthPercent.ToString() + "%";
		}
	}
}
