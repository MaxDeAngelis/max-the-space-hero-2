using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	public Slider healthBar;				// Slider to display the remaining health
	public AudioClip damageSoundEffect; 	// Sound effect for when you take damage
	public AudioClip deathSoundEffect; 		// Sound effect for when you take die
	public bool isPlayer = false;			// Flag for if the unit is the player
	public bool isAlienAbleToEject = false;	// Flag for if the alien can eject

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private SpriteRenderer[] _renderers;	// The sprite renderer of the object
	private List<float[]> _damageList = new List<float[]>();

	/* ---- OBJECTS/CONTROLLERS ---- */
	private EnemyController _enemy;
	private PlayerController _player;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on awake of the game object to init variables
	 **/
	void Start () {
		_renderers = GetComponentsInChildren<SpriteRenderer>();

		if (isPlayer) {
			_player = PlayerManager.Instance.getController();
		} else {
			_enemy = GetComponent<EnemyController>();
		}

		maximumHealth = health;
		updateHealth();
	}

	/**
	 * @private Called when the current gameobject dies
	 **/
	void _die() {
		// If the player dies then game over
		if (isPlayer) {
			GameManager.Instance.gameOver();
		} else {
			GameManager.Instance.processKill(_damageList, _enemy);
		}

		// Play and explosion on death
		SpecialEffectsManager.Instance.playExplosion(gameObject.transform.position, deathSoundEffect);

		// If the enemy can eject then eject
		if (isAlienAbleToEject) {
			SpecialEffectsManager.Instance.playAlienEject(gameObject.transform.position, null);
		}

		// Destroy the game object
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

			// Add data to damage array
			if (!isPlayer) {
				float[] damageItem = new float[]{damageModifier, damage};
				_damageList.Add(damageItem);
			}
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
			_die();
		}
	}

	bool _shouldProjectileHurt(ProjectileController controller) {
		bool returnValue = false;

		if (isPlayer) {

		} else {
			if (controller.isPlayer) {

			}
		}

		return returnValue;
	}
}
