using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public float maximumHealth;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float health = 400f;				// The maximum health of the unit
	public Text healthDisplay;				// UI Text component to display the percent of health left
	public Slider healthBar;
	public AudioClip damageSoundEffect; 	// Sound effect for when you take damage
	public bool isPlayer = false;			// Flag for if the unit is the player

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private SpriteRenderer[] _renderers;	// The sprite renderer of the object

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on awake of the game object to init variables
	 **/
	void Awake () {
		_renderers = GetComponentsInChildren<SpriteRenderer>();
		maximumHealth = health;
		updateHealth();
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
		if (weapon != null && weapon.isPlayer != isPlayer) {
			// Set the damage for the weapon
			damage = weapon.damage;

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

		// Make a sound and show damage if hurt
		if (damage > 0) {
			// Show floating text and play sound
			FloatingTextManager.Instance.show(transform, "-" + damage.ToString(), Color.red);
			SoundEffectsManager.Instance.makeSound(damageSoundEffect);

			// Actually take the damage
			StartCoroutine(_takeDamage(_renderers));
		}

		// Deduct the dame that the weapon does from your health
		health -= damage;
		
		// Increment the UI display of health
		updateHealth();
		
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
	 * @private Called to flash sprite renderers of the current game object to indicate damage
	 * 
	 * @param $SpriteRender[]$ sprites - An array of sprite renderers to flash
	 **/
	IEnumerator _takeDamage(SpriteRenderer[] sprites) {
		// Start by turning all renderers red
		for (int i = 0; i < sprites.Length; i++) {       
				sprites[i].color = Color.red;
		}

		// delay for a quarter second
		yield return new WaitForSeconds(0.25f);

		// Turn all renderers back to white
		for (int i = 0; i < sprites.Length; i++) {       
			sprites[i].color = Color.white;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Called to update any display of the health in the UI
	 **/
	public void updateHealth() {
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
}
