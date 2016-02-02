using UnityEngine;
using System.Collections;

public class PlayerWeapon : Laser {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* SECONDARY FIRE VARIABLES */
	public float secondaryDamage = 5f;
	public float secondaryChargeTime;
	public float secondaryEnergyCost;
	public GameObject secondaryProjectile;
	public AudioClip secondarySoundEffect;
	public AudioClip secondaryChargedSoundEffect;

	public Transform gunArm;							// Transform of the arm holding the gun

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isEnabled; 

	/* VARIABLES FOR SECONDARY FIRE */
	private bool _isFireDown = false;
	private bool _isSecondaryCharged = false;
	private float _startChargeTime;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called once per frame. Used to capture key events for later
	/// </summary>
	private void Update() {
		_aim();

		/* ---- HANDLE FIRING GUN ---- */
		// If the weapon is enabled, game is not paused, and not climbing then process the weapon controls
		if (_isEnabled && !MenuManager.Instance.isPaused() && !PlayerManager.Instance.isClimbing()) {
			// -- MOUSEDOWN -- When first pressed set flag in case of charging
			if (Input.GetButtonDown("Fire1")) {
				//Debug.Log("1");
				_isFireDown = true;
				_startChargeTime = Time.time;
				SpecialEffectsManager.Instance.playWeaponCharging(transform.position, null);
			}
			// -- MOUSEUP -- On mouse up check if charged and if you have energy to use a secondary
			if (Input.GetButtonUp("Fire1") && _isFireDown) {
				_isFireDown = false;
				SpecialEffectsManager.Instance.stopWeaponCharging();

				// If seconday is charged and there is still enough energy then use it
				if (_isSecondaryCharged && EnergyManager.Instance.getEnergy() >= secondaryEnergyCost) {
					_isSecondaryCharged = false;
					fireSecondary(gunArm.transform.position, transform.position, PlayerManager.Instance.isGrounded());
				} else {
					fire(gunArm.transform.position, transform.position, PlayerManager.Instance.isGrounded());
				}
			}
			// -- CHARGED -- While fire is down check if charged, play sound, and set flag
			if (!_isSecondaryCharged && _isFireDown) {
				// If the time is reached and there is enough energy then play a sound
				if ((Time.time - _startChargeTime) > secondaryChargeTime && EnergyManager.Instance.getEnergy() >= secondaryEnergyCost) {
					_isSecondaryCharged = true;
					SpecialEffectsManager.Instance.playWeaponCharged(transform.position, secondaryChargedSoundEffect);
				}
			}
		}
	}
		
	/// <summary>
	/// Flips the transform by reversing its scale
	/// </summary>
	public void _flipPlayer() {
		Vector3 theScale = PlayerManager.Instance.getTransform().localScale;
		theScale.x *= -1;
		PlayerManager.Instance.getTransform().localScale = theScale;
	}

	/// <summary>
	/// Rotate the arm towards the mouse cursor
	/// </summary>
	void _aim() {
		bool _isFacingRight = (PlayerManager.Instance.getTransform().localScale.x > 0);
		// Drop out if paused
		if (!MenuManager.Instance.isPaused()) {
			/* ---- AIM THE ARM TO FIRE ----*/		
			// Get a handle on the player pos and the mouse
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 playerPos = PlayerManager.Instance.getLocation();

			// If the mouse is on the oposite side then flip and skip
			if ((mousePos.x < playerPos.x && _isFacingRight) || (mousePos.x > playerPos.x && !_isFacingRight)) {
				_flipPlayer();
			} else {
				// Calculate the upward rotation
				Vector3 upward = gunArm.transform.position - mousePos;

				// TODO: Make localScale If not facing right invert x for correct rotation
				if (!_isFacingRight) {
					upward.x *= -1f;
				}

				// Set the look rotation of the arm
				gunArm.transform.rotation = Quaternion.LookRotation(Vector3.forward, upward);
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of game object
	/// </summary>
	public override void Start() {
		isPlayer = true;
		base.Start();
	}
		
	/// <summary>
	/// This function is called from the enemy controller to fire a ranged weapon
	/// </summary>
	/// <param name="origin">The position of the origin of the gun</param>
	/// <param name="target">The position of the target to fire at</param>
	/// <param name="firedFromGround">If set to <c>true</c> weapon was fired from ground.</param>
	public override void fire(Vector3 origin, Vector3 target, bool firedFromGround) {
		GameManager.Instance.processShot();

		base.fire(origin, target, firedFromGround);
	}
		
	/// <summary>
	/// Called from player controller to fire a secondary shot
	/// </summary>
	/// <param name="origin">The position of the origin of the gun</param>
	/// <param name="target">The position of the target to fire at</param>
	/// <param name="firedFromGround">If set to <c>true</c> weapon was fired from ground</param>
	public void fireSecondary(Vector3 origin, Vector3 target, bool firedFromGround) {
		EnergyManager.Instance.useEnergy(secondaryEnergyCost);

		attack(origin, target, secondaryDamage, secondaryProjectile, secondarySoundEffect, firedFromGround);
	}

	/// <summary>
	/// Sets the state of the players weapon
	/// </summary>
	/// <param name="state">If set to <c>true</c> _isEnabled will be set true and jetpack will work</param>
	public void setState(bool state) {
		_isEnabled = state;
	}
}