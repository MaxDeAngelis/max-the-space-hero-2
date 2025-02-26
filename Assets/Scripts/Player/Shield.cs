﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shield : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Slider shieldBar;				// The slider bar that displays the shield energy left
	public Text shieldDisplay;				// UI Text component to display the percent of shield left

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _strength; 				// The _strength of the shield
	private float _maximumShield;			// The maximum _strength of the shield
	private int _refreshDelay = 2 * 60; 	// Delay before shield starts to refresh
	private int _refreshRate = 15;			// The amount of frames befor a point is added to the shield
	private int _refreshDelayCount;			// Counter for delay of shield generation
	private int _refreshRateCount;			// Counter for rate of shield regeneration
	private Color _originalColor;			// The original color of the shield on start

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Animator _animator;
	private SpriteRenderer _renderer;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on awake of the game object to init variables
	/// </summary>
	private void Awake() {
		/* INIT COMPONENTS */
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();

		/* INIT VARIABLES */
		_strength = DataManager.Instance.getCurrentPlayerData().getShield();
		_maximumShield = _strength;
		_originalColor = _renderer.color;

		/* INIT SHIELD DISPLAY */
		_updateShield();
	}

	/// <summary>
	/// Called 60times per second fixed, handles all processing
	/// </summary>
	private void FixedUpdate() {
		// Set flying boolean in animator
		_animator.SetBool("flying", PlayerManager.Instance.isFlying());

		// Only recharge the shield if it is missing some energy
		if (_strength < _maximumShield) {
			// Process the delay of refreshing the shield
			if (_refreshDelayCount >= _refreshDelay) {
				// If the refresh rate expires add _strength to the shield
				if (_refreshRateCount >= _refreshRate) {
					_refreshRateCount = 0;
					_strength++;
					_updateShield();
				} else {
					_refreshRateCount++;
				}
			} else {
				_refreshDelayCount++;
			}
		}
	}

	/// <summary>
	/// Collider handler that is triggered when another collider interacts with this game object
	/// </summary>
	/// <param name="otherCollider">The collider causing the trigger</param>
	private void OnTriggerEnter2D(Collider2D otherCollider) {
		Projectile projectile = otherCollider.gameObject.GetComponent<Projectile>();
		if (projectile != null && !projectile.isPlayer) {
			float damage = projectile.damage;

			// Reset timers everytime you are hit
			_refreshDelayCount = 0;
			_refreshRateCount = 0;

			// If the shield has power then use it
			if (_strength > 0 ) {
				// Figure out how much damage to give to absorb and pass along the rest
				if ((_strength - damage) < 0) {
					// Reset the projectile damage to the remainder
					projectile.damage = damage - _strength;

					// Set damage to what is left in the shield
					damage = _strength;
				} else {
					projectile.hit();
				}

				// Take the damage 
				_strength -= damage;

				// Update shield display
				_updateShield();

				// Set the color before animation
				_renderer.color = _originalColor;

				// Fire the animation
				_animator.SetTrigger("block");
			}
		}
	}

	/// <summary>
	/// Called to update any display of the health in the UI
	/// </summary>
	private void _updateShield() {
		// Only update text in UI if it was configured to do so
		if (shieldBar) {
			// Calculate the percentage of health left
			float shieldPercent = Mathf.Round((_strength/_maximumShield) * 100);
			
			// Display the calculated string
			shieldDisplay.text = _strength + "/" + _maximumShield;
			
			// Update health bar value
			shieldBar.value = shieldPercent;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Reset the players shield
	/// </summary>
	public void reset() {
		// Reset shield power and color
		_strength = DataManager.Instance.getCurrentPlayerData().getShield();
		_maximumShield = _strength;
		_renderer.color = _originalColor;
		_renderer.enabled = false;

		/* INIT SHIELD DISPLAY */
		_updateShield();
	}
	/// <summary>
	/// Called to add more strength to the shield
	/// </summary>
	/// <param name="strengthToAdd">Strength to add</param>
	public void addStrength(float strengthToAdd) {
		// If the new strength is going to be more than the max then cap it
		if ((_strength + strengthToAdd) > _maximumShield) {
			_strength = _maximumShield;
		} else {
			_strength += strengthToAdd;
		}

		// Update the UI of the shield to display new values
		_updateShield();
	}

	/// <summary>
	/// Returns the current strength of the shield
	/// </summary>
	/// <returns>The strength of the shield</returns>
	public float getStrength() {
		return _strength;
	}

	/// <summary>
	/// Called to set the maximum allowed strength of the shield
	/// </summary>
	/// <param name="newMaxShield">The new maximum shield strength</param>
	public void setMaxShield(float newMaxShield) {
		_maximumShield = newMaxShield;

		if (_strength > _maximumShield) {
			_strength = _maximumShield;
		}

		// Update the UI of the shield to display new values
		_updateShield();
	}
}
