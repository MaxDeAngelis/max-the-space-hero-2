using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float strength = 100f; 			// The strength of the shield
	public Slider shieldBar;				// The slider bar that displays the shield energy left
	public Text shieldDisplay;				// UI Text component to display the percent of shield left

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _maximumShield;			// The maximum strength of the shield
	private int _refreshDelay = 2 * 60; 	// Delay before shield starts to refresh
	private int _refreshRate = 15;			// The amount of frames befor a point is added to the shield
	private int _refreshDelayCount;			// Counter for delay of shield generation
	private int _refreshRateCount;			// Counter for rate of shield regeneration
	private Color _originalColor;			// The original color of the shield on start

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Animator _animator;
	private PlayerController _player;
	private SpriteRenderer _renderer;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		/* INIT COMPONENTS */
		_animator = GetComponent<Animator>();
		_player = PlayerManager.Instance.getController();
		_renderer = GetComponent<SpriteRenderer>();

		/* INIT VARIABLES */
		_maximumShield = strength;
		_originalColor = _renderer.color;

		/* INIT SHIELD DISPLAY */
		_updateShield();
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// Set flying boolean in animator
		_animator.SetBool("flying", _player.isFlying());

		// Only recharge the shield if it is missing some energy
		if (strength < _maximumShield) {
			// Process the delay of refreshing the shield
			if (_refreshDelayCount >= _refreshDelay) {
				// If the refresh rate expires add strength to the shield
				if (_refreshRateCount >= _refreshRate) {
					_refreshRateCount = 0;
					strength++;
					_updateShield();
				} else {
					_refreshRateCount++;
				}
			} else {
				_refreshDelayCount++;
			}
		}
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

			// Reset timers everytime you are hit
			_refreshDelayCount = 0;
			_refreshRateCount = 0;

			// If the shield has power then use it
			if (strength > 0 ) {
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

				// Set the color before animation
				_renderer.color = _originalColor;

				// Fire the animation
				_animator.SetTrigger("block");
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
