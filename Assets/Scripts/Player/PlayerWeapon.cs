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
	/* VARIABLES FOR SECONDARY FIRE */
	private bool _isFireDown = false;
	private bool _isSecondaryCharged = false;
	private float _startChargeTime;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @private Flips the transform by reversing its scale
	**/
	public void _flipPlayer() {
		Vector3 theScale = PlayerManager.Instance.getTransform().localScale;
		theScale.x *= -1;
		PlayerManager.Instance.getTransform().localScale = theScale;
	}

	/**
	 * @private rotate the arm towards the mouse cursor
	 **/
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
	/**
	* @public Called on start of game object
	**/
	public override void Start() {
		isPlayer = true;
		base.Start();
	}

	/**
	 * @private called once per frame. Used to capture key events for later
	 **/
	public void Update() {
		_aim();

		/* ---- HANDLE FIRING GUN ---- */
		// 1. When first pressed set flag in case of charging
		// 2. On mouse up check if charged and if you have energy to use a secondary
		// 3. While fire is down check if charged, play sound, and set flag
		if (Input.GetButtonDown("Fire1") && !PlayerManager.Instance.isClimbing()) {
			_isFireDown = true;
			_startChargeTime = Time.time;
			SpecialEffectsManager.Instance.playWeaponCharging(transform.position, null);
		} else if (Input.GetButtonUp("Fire1") && _isFireDown && !PlayerManager.Instance.isClimbing()) {
			_isFireDown = false;
			SpecialEffectsManager.Instance.stopWeaponCharging();

			// If seconday is charged and there is still enough energy then use it
			if (_isSecondaryCharged && EnergyManager.Instance.getEnergy() >= secondaryEnergyCost) {
				_isSecondaryCharged = false;
				fireSecondary(gunArm.transform.position, transform.position, PlayerManager.Instance.isGrounded());
			} else {
				fire(gunArm.transform.position, transform.position, PlayerManager.Instance.isGrounded());
			}
		} else if (!_isSecondaryCharged && _isFireDown && !PlayerManager.Instance.isClimbing()) {
			// If the time is reached and there is enough energy then play a sound
			if ((Time.time - _startChargeTime) > secondaryChargeTime && EnergyManager.Instance.getEnergy() >= secondaryEnergyCost) {
				_isSecondaryCharged = true;
				SpecialEffectsManager.Instance.playWeaponCharged(transform.position, secondaryChargedSoundEffect);
			}
		}
	}

	/**
	* @public This function is called from the enemy controller to fire a ranged weapon
	* 
	* @param $Vector3$ origin - The position of the origin of the gun
	* @param $Vector3$ target - The position of the target to fire at
	* @param $Boolean$ firedFromGround - Is the weapon fired from the ground
	**/
	public override void fire(Vector3 origin, Vector3 target, bool firedFromGround) {
		GameManager.Instance.processShot();

		base.fire(origin, target, firedFromGround);
	}

	/**
	 * @public Called from player controller to fire a secondary shot
	 *
	 * @param $Vector3$ origin - The position of the origin of the gun
	 * @param $Vector3$ target - The position of the target to fire at
	 * @param $Boolean$ firedFromGround - Is the weapon fired from the ground
	 **/
	public void fireSecondary(Vector3 origin, Vector3 target, bool firedFromGround) {
		EnergyManager.Instance.useEnergy(secondaryEnergyCost);

		attack(origin, target, secondaryDamage, secondaryProjectile, secondarySoundEffect, firedFromGround);
	}
}