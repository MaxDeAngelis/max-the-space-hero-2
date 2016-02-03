using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour {
	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	public float health = 400f;				// The current health of the unit
	public float maximumHealth;				// The maximum health of the unit
	public AudioClip damageSoundEffect; 	// Sound effect for when you take damage
	public AudioClip deathSoundEffect; 		// Sound effect for when you take die
	public bool isPlayer = false;			// Flag for if the unit is the player

	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	private SpriteRenderer[] _renderers;	// The sprite renderer of the object

	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/		
	/// <summary>
	/// Called when taking damage, and and flashes all sprite renderers red
	/// </summary>
	/// <param name="sprites">The renderers to flash</param>
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

	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	/// <summary>
	/// Called on start of the game obect
	/// </summary>
	public virtual void Start () {
		_renderers = GetComponentsInChildren<SpriteRenderer>();

		maximumHealth = health;
	}

	/// <summary>
	/// Called from hitboxes to process damage against the unit
	/// </summary>
	/// <param name="damageModifier">The modifier to apply</param>
	/// <param name="triggerCollider">The collider of what is damaging the unit</param>
	public void processDamage(float damageModifier, Collider2D triggerCollider) {
		// Define damage to be taken
		float damage = 0f;

		// Try and get the Weapon Controller off the colider to see if you are being hit with a weapon
		Weapon weapon = triggerCollider.gameObject.GetComponent<Weapon>();
		Projectile projectile = triggerCollider.gameObject.GetComponent<Projectile>();

		/* ---- WEAPON DAMAGE ---- */
		if (weapon != null && weapon.isPlayer != isPlayer) {
			// Set the damage for the weapon
			damage = Mathf.RoundToInt(weapon.damage * damageModifier);

			// For suicide blow up
			if (weapon.type == WEAPON_TYPE.Suicide) {
				weapon.explode();
			}
		}

		/* ---- PROJECTILE DAMAGE ---- */
		if (projectile != null && projectile.isPlayer != isPlayer) {
			// Set the damage for the projectile
			damage = Mathf.RoundToInt(projectile.damage * damageModifier);

			// Fire the hit function for the projectile
			projectile.hit();

			// Call damage to account for the hit
			damaged(damageModifier, damage);
		}

		// Make a sound and show damage if hurt
		if (damage > 0) {
			// Show floating text and play sound
			Utilities.Instance.showFadeAwayText(transform, "-" + damage.ToString(), Color.red);
			SpecialEffectsManager.Instance.playSound(damageSoundEffect);

			// Actually take the damage
			StartCoroutine(_takeDamage(_renderers));
		}

		// Deduct the dame that the weapon does from your health
		health -= damage;

		// Increment the UI display of health
		updateHealth();


		// If your health drops below 0 die
		if (health <= 0) {
			die();
		}
	}

	/*//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		VIRTUAL FUNCTIONS											     ///
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/
	/// <summary>
	/// Called on death
	/// </summary>
	protected virtual void die() {
		// Play and explosion on death
		SpecialEffectsManager.Instance.playExplosion(gameObject.transform.position, deathSoundEffect);

		// Destroy the game object
		Destroy(gameObject);
	}

	/// <summary>
	/// Reset the health componenet and make sure all renderers are white again
	/// </summary>
	public virtual void reset() {
		Start();

		// Turn all renderers back to white
		for (int i = 0; i < _renderers.Length; i++) {       
			_renderers[i].color = Color.white;
		}
	}

	/// <summary>
	/// Extension point to be used when the units health changes
	/// </summary>
	public virtual void updateHealth(){}

	/// <summary>
	/// Extension point to be called when damaged
	/// </summary>
	/// <param name="modifier">The damage modifier</param>
	/// <param name="damage">The amount of damage that was taken</param>
	protected virtual void damaged(float modifier, float damage) {}
}
