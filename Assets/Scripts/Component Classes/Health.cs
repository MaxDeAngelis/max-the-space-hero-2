using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float health = 400f;				// The maximum health of the unit
	public float maximumHealth;
	public AudioClip damageSoundEffect; 	// Sound effect for when you take damage
	public AudioClip deathSoundEffect; 		// Sound effect for when you take die
	public bool isPlayer = false;			// Flag for if the unit is the player

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private SpriteRenderer[] _renderers;	// The sprite renderer of the object

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
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
	* @private Called on awake of the game object to init variables
	**/
	public virtual void Start () {
		_renderers = GetComponentsInChildren<SpriteRenderer>();

		maximumHealth = health;
	}

	/**
	 * @public Called from the hitbox to process damage that was taken
	 * 
	 * @param $Float$ damageModifier - The modifier defined on the hitbox
	 * @param $Collider$ triggerCollider - The collider that caused the damage
	 **/
	public void processDamage(float damageModifier, Collider2D triggerCollider) {
		// Define damage to be taken
		float damage = 0f;

		// Try and get the Weapon Controller off the colider to see if you are being hit with a weapon
		WeaponController weapon = triggerCollider.gameObject.GetComponent<WeaponController>();
		ProjectileController projectile = triggerCollider.gameObject.GetComponent<ProjectileController>();

		/* ---- WEAPON DAMAGE ---- */
		if (weapon != null && weapon.isPlayer != isPlayer) {
			// Set the damage for the weapon
			damage = Mathf.RoundToInt(weapon.damage * damageModifier);

			// Damage the actual weapon
			weapon.durability -= weapon.durabilityLossPerAttack;
			if(weapon.durability <= 0) {
				weapon.broken();
			}

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
			FloatingTextManager.Instance.show(transform, "-" + damage.ToString(), Color.red);
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		VIRTUAL FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @protected Called on death
	 **/
	protected virtual void die() {
		// Play and explosion on death
		SpecialEffectsManager.Instance.playExplosion(gameObject.transform.position, deathSoundEffect);

		// Destroy the game object
		Destroy(gameObject);
	}

	/**
	* @public Called after damage is taken, so after health is adjusted
	**/
	public virtual void updateHealth(){}

	/**
	 * @protected Called when ever projectile damage is taken
	 **/
	protected virtual void damaged(float modifier, float damage) {}
}
