﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* ---- MOVEMENT RELATED ---- */
	public float movementSpeed = 5f;			// Grounded movement speed
	public float boostForce = 10f;				// Jetpack boost force
	public float maximumVelocity = 18f;			// Maximum jetpack velocity

	/* ---- ENERGY RLATED ---- */
	public float boostCost = 1f;				// The energy cost of using your jetpack
	public float takeOffCost = 25f;				// The energy cost of taking off
	public int flyingEnergyRegenRate = 30;		// The rate of which energy is regenerated when flying
	public int anchoredEnergyRegenRate = 6;		// The rate of which energy is regenerated when landed

	/* ---- SUPPORTING TRANSFORMS ---- */
	public Transform topLandingCheck;			// Transform used to check if top is in range to land
	public Transform bottomLandingCheck;		// Transform used to check if bottom is in range to land
	public Transform forwardGroundCheck;		// Transform to check if front of player is on ground
	public Transform backwardGroundCheck;		// Transform to check if back of player is on ground
	public Transform gunArm;					// Transform of the arm holding the gun

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isForwardGrounded = false;	// Flag for when the front of the player is grounded
	private bool _isBackwardGrounded = false;	// Flag for whant the back of the player is grounded
	private bool _isAnchored = true;			// Flag for when player is attached to the ground
	private bool _isClimbing = false;			// Flag for when player is actually climbing
	private bool _isFacingRight = true;			// Flag for if the player is facing the righ
	private bool _isAbleToLand = false;
	private bool _isTakingOff = false;
	private float _originalGravityScale;		// Starting gravity 
	private Vector3 _boundryIntersectPosition;	// The position the player was in as he intersects with a boundry
	private string[] _groundLayers = new string[2] {"Ground", "Climbable"}; // List of layers to consider ground

	/* VARIABLES FOR WAITING ANIMATION CHECK */
	private int _framesBeforeWait = 60 * 10;	// Amount of frames to count before considered waiting
	private int _frameWaitCounter = 0;			// Frame counter for wait animation
	private Vector3 _previousVelocity;			// Previous frames velocity for max
	private Vector2 _previousMousePosition;		// Previous frames mouse position
	private Quaternion _defaultArmRotation;     // The default arm rotation at start

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Rigidbody2D _rigidbody;
	private BoxCollider2D _collider;
	private ClimbController _climbable;
	private WeaponController _weapon;
	private Animator _animator;
	private SpriteRenderer _renderer;

	/* ---- MANAGERS ---- */
	private PowerupManager _powerupManager;
	private EnergyManager _energyManager;

	/* ---- KEY TRACKER ---- */
	//private enum KEYS {None, Jump, Shoot};
	//private KEYS _keyDown;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		/* INIT COMPONENTS */
		_rigidbody = GetComponent<Rigidbody2D>();
		_collider = GetComponent<BoxCollider2D>();
		_weapon = GetComponentInChildren<WeaponController>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();

		/* INIT MANAGERS */
		_powerupManager = PowerupManager.Instance;
		_energyManager = EnergyManager.Instance;

		/* INIT VARIABLES */
		_originalGravityScale = _rigidbody.gravityScale;
		_previousVelocity = _rigidbody.velocity;
		_previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		_defaultArmRotation = gunArm.transform.rotation;
	}

	/**
	 * @private called once per frame. Used to capture key events for later
	 **/
	void Update() {
		// Aim your weapon towards the mouse
		_aim();

		// Check if it is possable to land
		_checkIfAbleToLand();
		
		// Line cast to the ground check transform to see if it is over a ground layer
		_checkIfGrounded();

		/* ---- HANDLE FIRING GUN ---- */
		if (Input.GetButtonDown("Fire1") && !_isClimbing) {
			Vector3 target = Input.mousePosition;
			target.z = transform.position.z - Camera.main.transform.position.z;
			target = Camera.main.ScreenToWorldPoint(target);

			//if (!_renderer.bounds.Contains(target)) {
				_weapon.fire(target);
			//}
		}
		
		
		/* ---- HANDLE ANCHORING ---- */
		if (Input.GetButtonDown("Jump") && _isAnchored && !_climbable && (_isForwardGrounded || _isBackwardGrounded)) {
			_isAnchored = false;
			_rigidbody.gravityScale = 0;
			_collider.isTrigger = true;
			
			StartCoroutine(_takeoff());
		} else if (Input.GetButtonDown("Jump") && !_isAnchored && _isAbleToLand) {
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

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// Check if Max is waiting
		_checkIfWaiting();

		/* ---- HANDLE MOVEMENT ---- */
		if (_isAnchored) {
			// Set the default vertical/horizontal velocity to what it currently is
			float verticalVelocity = _rigidbody.velocity.y;
			float horizontalVelocity = Input.GetAxis("Horizontal") * movementSpeed;

			/* ---- HANDLE IF OVER A CLIMBABLE OBJECT ---- */
			if (_climbable) {
				// Default the vertical velocity to the key press
				verticalVelocity = Input.GetAxis("Vertical");

				if (verticalVelocity != 0f) {


					if (!_isClimbing) {
						_isClimbing = true;

						// Start climbling since you are moving on a climbable object
						_animator.SetBool("climbing", true);

						// Center Max on the climbable object
						transform.position = new Vector3(_climbable.transform.position.x, transform.position.y, transform.position.z);
					}
					// Set the special speed based on climb controller defaults
					if (verticalVelocity > 0) {
						verticalVelocity = _climbable.upSpeed;
					} else {
						verticalVelocity = _climbable.downSpeed * -1;
					}
				}
			} else {
				// Start climbling since you are moving on a climbable object
				_animator.SetBool("climbing", false);
			}
				
			/* ---- CHECK IF USER IS GOING TO FALL ---- */ 
			if (_isFacingRight && horizontalVelocity > 0 && !_isForwardGrounded) { 
				// FACING RIGHT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (!_isFacingRight && horizontalVelocity > 0 && !_isBackwardGrounded) {
				// FACING LEFT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (_isFacingRight && horizontalVelocity < 0 && !_isBackwardGrounded) {
				// FACING RIGHT MOVING LEFT
				horizontalVelocity = 0f;
			} else if (!_isFacingRight && horizontalVelocity < 0 && !_isForwardGrounded) {
				// FACING LEFT MOVING RIGHT
				horizontalVelocity = 0f;
			}

			// Set speed floats for  animations
			_animator.SetFloat("horizontalSpeed", Mathf.Abs(horizontalVelocity));
			_animator.SetFloat("verticalSpeed", Mathf.Abs(verticalVelocity));

			/* ---- HANDLE MOVING HORIZONTALLY AND VERTICALLY ---- */
			_rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
		} else {
			// Nullify horizontalSpeed if you loose anchor
			_animator.SetFloat("horizontalSpeed", 0f);

			// If direction key is down and there is enough energy then boost
			if (_energyManager.energy >= boostCost && (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))) {
				// Start boosting
				_animator.SetBool("boosting", true);

				// Calculate artifical drag force
				float drag = boostForce / maximumVelocity;


				// Get a reference to the input of vertical and horizontal force
				float horizontalForce = Input.GetAxis("Horizontal");
				float verticalForce = Input.GetAxis("Vertical");

				// Modify new force by adding or subtracting 2 this giver a bigger number the smaller the force is
				// for example when first pressed force might equal 0.05 after mod it equals 1.95
				// this results in a faster accelaration upfront
				// Modify horizontal force
				if (horizontalForce > 0f) {
					horizontalForce = 2 - horizontalForce;
				} else if (horizontalForce < 0f) {
					horizontalForce = -2 - horizontalForce;
				}
				// Modify vertical force
				if (verticalForce > 0f) {
					verticalForce = 2 - verticalForce;
				} else if (verticalForce < 0f) {
					verticalForce = -2 - verticalForce;
				}

				// Get the direction based on keys down
				Vector2 direction = new Vector2(horizontalForce, verticalForce);

				// Calculate the new velocity using the fage drag
				Vector2 newVelocity = (direction * boostForce) - (_rigidbody.velocity * drag);

				// Apply the velocity over time
				_rigidbody.velocity += newVelocity * Time.deltaTime ;

				// If you are moving using energy
				if (Mathf.Abs(newVelocity.x) > 0.25f || Mathf.Abs(newVelocity.y) > 0.25f) {
					_energyManager.useEnergy(boostCost);
				}
			} else if (!_isTakingOff) {
				// Stop boosting
				_animator.SetBool("boosting", false);
			}
		}
	}

	/**
	 * @private checks if max is waiting and if so triggers the wait animation
	 **/
	void _checkIfWaiting() {
		// If the wait limit has been reached then set off the wait animation
		if (_frameWaitCounter > _framesBeforeWait) {
			// Rotate the arm back to the original location
			gunArm.transform.rotation = _defaultArmRotation;
			_animator.SetBool("waiting", true);
		}
		
		// Checks if the mouse is still in the same position and that max has not moved since last frame if not then he is active
		if (Vector3.Distance(_previousVelocity, _rigidbody.velocity) != 0f || Vector2.Distance(_previousMousePosition, Camera.main.ScreenToViewportPoint(Input.mousePosition)) != 0f) {
			_previousVelocity = _rigidbody.velocity;
			_previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			_animator.SetBool("waiting", false);
			_frameWaitCounter = 0;
		} else {
			_frameWaitCounter += 1;
		}

	}

	void _checkIfAbleToLand() {
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
	}

	/**
	 * @private rotate the arm towards the mouse cursor
	 **/
	void _aim() {
		// Drop out if paused
		if (!GameManager.Instance.isPaused()) {
			/* ---- AIM THE ARM TO FIRE ----*/		
			// Get mouse position and arm position
			Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			Vector3 armPos = Camera.main.WorldToViewportPoint(gunArm.transform.position);
			
			// Get arm and mouse position relative to the game object
			Vector2 relativeArmPos = new Vector2(armPos.x - 0.5f, armPos.y - 0.5f);
			Vector2 relativeMousePos = new Vector2 (mousePos.x - 0.5f, mousePos.y - 0.5f) - relativeArmPos;
			float angle = Vector2.Angle (Vector2.down, relativeMousePos);
			
			// Flip the player if aiming in the opposite direction
			if ((relativeMousePos.x < 0 && _isFacingRight) || (relativeMousePos.x > 0 && !_isFacingRight)) {
				_flipPlayer();
			}
			
			// Calculate the Quaternion and rotate the arm
			Quaternion quat = Quaternion.identity;
			quat.eulerAngles = new Vector3(0, 0, angle);
			
			// Rotate the arm pieces
			gunArm.transform.rotation = quat;
		}
	}

	/**
	 * @private checks to see if the player is bumping into the boundries and if so it stops the player
	 **/
	void _checkIfHittingBoundry(Collider2D otherCollider) {
		// Four boundry scenarios
		// 1. Hitting left boundry and still moving left
		// 2. Hitting right boundry and still moving right
		// 3. Hitting top boundry and still moving up
		// 4. Hitting bottom boundry and still moving down
		if (otherCollider.gameObject.tag == "LEFT_BOUNDRY" && Input.GetAxis("Horizontal") <= 0){
			// Lock the players horizontal position and velocity
			transform.position = new Vector3(_boundryIntersectPosition.x, transform.position.y);
			_rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
		} else if (otherCollider.gameObject.tag == "RIGHT_BOUNDRY" && Input.GetAxis("Horizontal") >= 0) {
			// Lock the players horizontal position and velocity
			transform.position = new Vector3(_boundryIntersectPosition.x, transform.position.y);
			_rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
		} else if (otherCollider.gameObject.tag == "TOP_BOUNDRY" && Input.GetAxis("Vertical") >= 0) {
			// Lock the players vertical position and velocity
			transform.position = new Vector3(transform.position.x, _boundryIntersectPosition.y);
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
		} else if (otherCollider.gameObject.tag == "BOTTOM_BOUNDRY" && Input.GetAxis("Vertical") <= 0) {
			// Lock the players vertical position and velocity
			transform.position = new Vector3(transform.position.x, _boundryIntersectPosition.y);
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
		}
	}

	/**
	 * @private checks if the player is grounded on any of the supported types of grounds
	 **/
	void _checkIfGrounded() {
		// Default to false before the loop
		_isForwardGrounded = false;
		_isBackwardGrounded = false;
		// Loop over all supported ground layers and check if you are grounded
		foreach(string layerName in _groundLayers) {
			if (!_isForwardGrounded) {
				_isForwardGrounded = Physics2D.Linecast(transform.position, forwardGroundCheck.position, 1 << LayerMask.NameToLayer(layerName));
			}

			if (!_isBackwardGrounded) {
				_isBackwardGrounded = Physics2D.Linecast(transform.position, backwardGroundCheck.position, 1 << LayerMask.NameToLayer(layerName));
			}
		}
	}

	/**
	 * @private Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// Store off the intersection point where the player first hit the collider
		_boundryIntersectPosition = transform.position;

		// Check if hitting a boundry and clamp
		_checkIfHittingBoundry(otherCollider);

		// Ladder logic to allow climbing
		if (_isAnchored && otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = 0;
			_climbable = otherCollider.gameObject.GetComponent<ClimbController>();
		}

		// Process if you hit a powerup, call manager to gain boost
		if (_isAnchored && otherCollider.gameObject.tag == "Powerup") {
			_powerupManager.process(otherCollider.gameObject.GetComponent<PowerupController>(), gameObject);
		}
	}

	/**
	 * @private Handles checking what the collider is hitting
	 **/
	void OnTriggerStay2D(Collider2D otherCollider) {
		_checkIfHittingBoundry(otherCollider);
	}

	/**
	 * @private Handles checking if the player is no longer over a climbable object. Sets gravity back
	 **/
	void OnTriggerExit2D(Collider2D otherCollider) {
		if (_isAnchored && otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = _originalGravityScale;
			_climbable = null;
			_isClimbing = false;
		}
	}

	/**
	 * @private Called when taking off from an anchored position
	 **/
	IEnumerator _takeoff() {
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

		// If the user does not have a direction key down then stop velocity
		if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f) {
			// delay for a quarter second
			yield return new WaitForSeconds(0.75f);

			// After wait stop from moving
			_rigidbody.velocity = new Vector2(0f, 0f);
		}

		// Stop boosting
		_animator.SetBool("boosting", true);

		// Now that the take off is done flip flag
		_isTakingOff = false;
	}

	/**
	 * @private Flips the transform by reversing its scale
	 **/
	private void _flipPlayer() {
		_isFacingRight = !_isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public called to see if flying
	 **/
	public bool isFlying() {
		return ((!_isAnchored && !_isTakingOff) || _isClimbing);
	}
}
