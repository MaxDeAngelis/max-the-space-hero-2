using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float strength = 100f; 
	public Slider shieldBar;
	public Text shieldDisplay;				// UI Text component to display the percent of shield left

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isShieldDepleted = false;
	private float _maximumShield;

	/* ---- OBJECTS/CONTROLLERS ---- */
	private EdgeCollider2D _collider;
	private Animator _animator;
	private HealthController _health;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		/* INIT COMPONENTS */
		_collider = GetComponent<EdgeCollider2D>();
		_animator = GetComponent<Animator>();
		_health = GetComponentInParent<HealthController>();

		/* INIT VARIABLES */
		_maximumShield = strength;

		/* INIT SHIELD DISPLAY */
		_updateShield();
	}

	/**
	 * @private Collider handler that is triggered when another collider interacts with this game object
	 * 
	 * @param $Collider2D$ otherCollider - The collider that is interacting with this game object
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		ProjectileController projectile = otherCollider.gameObject.GetComponent<ProjectileController>();
		if (projectile != null && !projectile.isPlayer) {
			float damage = projectile.damage;

			if (!_isShieldDepleted && strength > 0 ) {
				// Figure out how much damage to give to absorb and pass along the rest
				if ((strength - damage) < 0) {
					// Reset the projectile damage to the remainder
					projectile.damage = damage - strength;

					// Set damage to what is left in the shield
					damage = strength;
				} else {
					projectile.hit();
				}

				// Take the damage 
				strength -= damage;

				// Update shield display
				_updateShield();

				// Fire the animation
				_animator.SetTrigger("block");
			} else if (strength <= 0) {
				_isShieldDepleted = true;
			}
		}
	}

	/**
	 * @public Called to update any display of the health in the UI
	 **/
	public void _updateShield() {
		// Only update text in UI if it was configured to do so
		if (shieldBar != null) {
			// Calculate the percentage of health left
			float shieldPercent = Mathf.Round((strength/_maximumShield) * 100);
			
			// Display the calculated string
			shieldDisplay.text = strength + "/" + _maximumShield;
			
			// Update health bar value
			shieldBar.value = shieldPercent;
		}
	}
}
