using UnityEngine;
using System.Collections;
using System;

public class Jetpack : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* ---- MOVEMENT RELATED ---- */
	public float boostForce;				// Jetpack boost force
	public float maximumVelocity;			// Maximum jetpack velocity

	/* ---- ENERGY RLATED ---- */
	public float boostCost;				// The energy cost of using your jetpack
	public float takeOffCost;				// The energy cost of taking off
	public int flyingEnergyRegenRate;		// The rate of which energy is regenerated when flying
	public int anchoredEnergyRegenRate;		// The rate of which energy is regenerated when landed

	/* ---- SUPPORTING TRANSFORMS ---- */
	public Transform topLandingCheck;			// Transform used to check if top is in range to land
	public Transform bottomLandingCheck;		// Transform used to check if bottom is in range to land

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isEnabled;
	private bool _isAnchored = true;			// Flag for when player is attached to the ground
	private bool _isTakingOff = false;
	private float _originalGravityScale;		// Starting gravity 

	private enum DIRECTION {Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft, Error};
	private DIRECTION _previousDirection = DIRECTION.Up;

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Rigidbody2D _rigidbody;
	private BoxCollider2D _collider;
	private Animator _animator;

	/* ---- MANAGERS ---- */
	private EnergyManager _energyManager;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on awake of game object
	/// </summary>
	private void Awake() {
		/* INIT COMPONENTS */
		_rigidbody = GetComponent<Rigidbody2D>();
		_collider = GetComponent<BoxCollider2D>();
		_animator = GetComponent<Animator>();

		/* INIT VARIABLES */
		_originalGravityScale = _rigidbody.gravityScale;

		/* INIT MANAGERS */
		_energyManager = EnergyManager.Instance;
	}

	/// <summary>
	/// Called once per frame
	/// </summary>
	private void Update () {
		// If enabled the process controls
		if (_isEnabled) {
			/* ---- HANDLE ANCHORING ---- */
			if (Input.GetButtonDown("Jump") && !isFlying()) {
				_isAnchored = false;
				_rigidbody.gravityScale = 0;
				_collider.isTrigger = true;

				StartCoroutine(_takeoff());
			} else if (_isAbleToLand() && isFlying() && Input.GetButtonDown("Jump")) {
				// Set the regeneration rate since you are no longer flying
				_energyManager.setRegenerationRate(anchoredEnergyRegenRate);

				// Reset all defaults
				_isAnchored = true;
				_collider.isTrigger = false;
				_rigidbody.gravityScale = _originalGravityScale;
				_rigidbody.velocity = new Vector2(0f, 0f);

				// Stop flying since you are landing
				_animator.SetBool("flying", false);
			}
		}
	}

	/// <summary>
	/// Called 60times per second fixed, handles all processing
	/// </summary>
	private void FixedUpdate() {
		if (isFlying()) {
			// Nullify horizontalSpeed if you loose anchor
			_animator.SetFloat("horizontalSpeed", 0f);

			float currentMagnitude = (float)Math.Round(_rigidbody.velocity.sqrMagnitude);

			// Get a reference to the input of vertical and horizontal force
			float horizontalForce = Input.GetAxis("Horizontal");
			float verticalForce = Input.GetAxis("Vertical");

			if (currentMagnitude > maximumVelocity) {
				// Calculate the difference in velocity from now to cap
				float brakeSpeed = maximumVelocity - _rigidbody.velocity.sqrMagnitude;

				// Figure out the amount needed to slow current velocity to excepted speed
				Vector2 normalisedVelocity = _rigidbody.velocity.normalized;
				Vector2 brakeVelocity = normalisedVelocity * brakeSpeed;

				// Apply the ammount needed to break
				_rigidbody.AddForce(brakeVelocity);
			} else if (_isBoostAllowed(currentMagnitude, horizontalForce, verticalForce)) {
				// Start boosting
				_animator.SetBool("boosting", true);

				// Get the direction based on keys down
				Vector2 direction = new Vector2(horizontalForce, verticalForce);

				// Calculate the new velocity using the fage drag
				Vector2 newVelocity = direction * boostForce;

				// Add the force to boost and use energy
				_rigidbody.AddForce(newVelocity);
				_energyManager.useEnergy(boostCost);
			} else if (!_isTakingOff) {
				// Stop boosting
				_animator.SetBool("boosting", false);
			}
		}
	}

	/// <summary>
	/// Called to return the direction of a given x,y location
	/// </summary>
	/// <returns>The DIRECTION enum value for whice quadrent its in</returns>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	private DIRECTION _getDirection(float x, float y) {
		if (x == 0 && y > 0) {
			return DIRECTION.Up;
		} else if (x > 0 && y > 0) {
			return DIRECTION.UpRight;
		}if (x > 0 && y == 0) {
			return DIRECTION.Right;
		} else if (x > 0 && y < 0) {
			return DIRECTION.DownRight;
		} else if (x == 0 && y < 0) {
			return DIRECTION.Down;
		} else if (x < 0 && y < 0) {
			return DIRECTION.DownLeft;
		} else if (x < 0 && y == 0) {
			return DIRECTION.Left;
		} else if (x < 0 && y > 0) {
			return DIRECTION.UpLeft;
		}

		return DIRECTION.Error;
	}

	/// <summary>
	/// Called to see if boosting is allowed at the moment
	/// </summary>
	/// <returns><c>true</c>, if boosting is allowed, <c>false</c> otherwise.</returns>
	/// <param name="currentMagnitude">Current magnitude</param>
	/// <param name="horizontalForce">Horizontal force</param>
	/// <param name="verticalForce">Vertical force</param>
	private bool _isBoostAllowed(float currentMagnitude, float horizontalForce, float verticalForce) {
		// First check if directional keys are down and you have energy
		// No need to even check additional logic if first level fails
		if ((Input.GetButton("Vertical") || Input.GetButton("Horizontal")) &&  _energyManager.getEnergy() >= boostCost) {
			// 1: If capped velocity and trying to head in the same direction the player is moving do not allow
			// 2: If the velocity is capped but you are trying to move another direction then allow it
			// 3: If not capped continue moving and also set previous direction
			if (currentMagnitude == maximumVelocity && _getDirection(horizontalForce, verticalForce) == _getDirection(_rigidbody.velocity.x, _rigidbody.velocity.y)) {
				return false;
			} else if (currentMagnitude == maximumVelocity && _previousDirection != _getDirection(horizontalForce, verticalForce)) {
				return true;
			} else if (currentMagnitude < maximumVelocity) {
				_previousDirection = _getDirection(horizontalForce, verticalForce);
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Called to check if you are able to land
	/// </summary>
	/// <returns><c>true</c>, if if able to land was checked, <c>false</c> otherwise.</returns>
	private bool _isAbleToLand() {
		bool _isAbleToLand = false;
		bool isTopHitting = Physics2D.Linecast(transform.position, topLandingCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		bool isBottomHitting = Physics2D.Linecast(transform.position, bottomLandingCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		// If the top is not hitting ground and the bottom is then you can land
		if (!isTopHitting && isBottomHitting) {
			_isAbleToLand = true;
		} else {
			_isAbleToLand = false;
		}

		// Always set landing flag for animation
		_animator.SetBool("ableToLand", _isAbleToLand);

		return _isAbleToLand;
	}

	/// <summary>
	/// Called when taking off from an anchored position
	/// </summary>
	private IEnumerator _takeoff() {
		// Set Takeoff flag to allow animation to fire
		_isTakingOff = true;

		// Use up a chunk of energy since you are taking off
		_energyManager.useEnergy(takeOffCost);

		// Set the regeneration rate since you are now flying
		_energyManager.setRegenerationRate(flyingEnergyRegenRate);

		// Start flying since you have taken off
		_animator.SetBool("flying", true);

		// Start boosting
		_animator.SetBool("boosting", true);

		// Add inital force to get off ground
		_rigidbody.AddForce(new Vector2(0f, 5 * maximumVelocity));

		// delay for a quarter second
		yield return new WaitForSeconds(0.75f);

		// If the user does not have a direction key down then stop velocity
		if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f) {
			// After wait stop from moving
			_rigidbody.velocity = new Vector2(0f, 0f);
		}

		// Now that the take off is done flip flag
		_isTakingOff = false;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to reset the state of the jetpack
	/// </summary>
	public void reset() {
		// Reset the defaults
		_isAnchored = true;
		_isTakingOff = false;

		// Reset physics
		_collider.isTrigger = false;
		_rigidbody.gravityScale = _originalGravityScale;

		// Reset the animator flags
		_animator.SetBool("flying", false);
		_animator.SetBool("boosting", false);
		_animator.SetBool("ableToLand", false);
	}

	/// <summary>
	/// Sets the state of the jetpack
	/// </summary>
	/// <param name="state">If set to <c>true</c> _isEnabled will be set true and jetpack will work</param>
	public void setState(bool state) {
		_isEnabled = state;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	    	  FLAGS 	  							     	    	     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to see if flying
	/// </summary>
	/// <returns><c>true</c>, if flying, <c>false</c> otherwise.</returns>
	public bool isFlying() {
		return ((!_isAnchored && !_isTakingOff) || _isTakingOff);
	}

	/// <summary>
	/// Called to see if anchored
	/// </summary>
	/// <returns><c>true</c>, if anchored, <c>false</c> otherwise.</returns>
	public bool isAnchored() {
		return _isAnchored;
	}
}
