﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public bool facingRight = true;			// Flag for if the player is facing the right 

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float movementSpeed = 5f;			// Grounded movement speed
	public float boostForce = 10f;				// Jetpack boost force
	public float boostCost = 1f;				// The energy cost of using your jetpack
	public float maximumVelocity = 18f;			// Maximum jetpack velocity
	public Transform forwardGroundCheck;		// GameObject to check if front of player is on ground
	public Transform backwardGroundCheck;		// GameObject to check if back of player is on ground

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isForwardGrounded = false;	// Flag for when the front of the player is grounded
	private bool _isBackwardGrounded = false;	// Flag for whant the back of the player is grounded
	private bool _isAnchored = true;			// Flag for when player is attached to the ground
	private bool _isClimbing = false;			// Flag for when player is actually climbing
	private float _originalGravityScale;		// Starting gravity 
	private Vector3 _boundryIntersectPosition;	// The position the player was in as he intersects with a boundry
	private string[] _groundLayers = new string[2] {"Ground", "Climbable"}; // List of layers to consider ground

	/* VARIABLES FOR WAITING ANIMATION CHECK */
	private int _framesBeforeWait = 60 * 10;	// Amount of frames to count before considered waiting
	private int _frameWaitCounter = 0;			// Frame counter for wait animation
	private Vector3 _previousVelocity;			// Previous frames velocity for max
	private Vector2 _previousMousePosition;		// Previous frames mouse position

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Rigidbody2D _rigidbody;
	private BoxCollider2D _collider;
	private ClimbController _climbable;
	private WeaponController _weapon;
	private LandingController _landing;
	private EnergyController _energy;
	private Animator _animator;

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
		_landing = GetComponentInChildren<LandingController>();
		_energy = GetComponent<EnergyController>();
		_animator = GetComponent<Animator>();

		/* INIT VARIABLES */
		_originalGravityScale = _rigidbody.gravityScale;
		_previousVelocity = _rigidbody.velocity;
		_previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// Check if Max is waiting
		_checkIfWaiting();

		// Always set landing flag for animation
		_animator.SetBool("ableToLand", _landing.isAbleToLand);

		// Line cast to the ground check transform to see if it is over a ground layer
		_checkIfGrounded();

		/* ---- HANDLE FIRING GUN ---- */
		if (Input.GetButtonDown("Fire1") && !_isClimbing) {
			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			_weapon.fire(pos);
		}

		/* ---- HANDLE ANCHORING ---- */
		if (Input.GetButtonDown("Jump") && _isAnchored && !_climbable && (_isForwardGrounded || _isBackwardGrounded)) {
			_isAnchored = false;
			_rigidbody.gravityScale = 0;
			_collider.isTrigger = true;

			StartCoroutine(_takeoff());
		} else if (Input.GetButtonDown("Jump") && !_isAnchored && _landing.isAbleToLand) {
			_isAnchored = true;
			_collider.isTrigger = false;
			_rigidbody.gravityScale = _originalGravityScale;
			_rigidbody.velocity = new Vector2(0f, 0f);

			// Stop flying since you are landing
			_animator.SetBool("flying", false);
		}

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
			if (facingRight && horizontalVelocity > 0 && !_isForwardGrounded) { 
				// FACING RIGHT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (!facingRight && horizontalVelocity > 0 && !_isBackwardGrounded) {
				// FACING LEFT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (facingRight && horizontalVelocity < 0 && !_isBackwardGrounded) {
				// FACING RIGHT MOVING LEFT
				horizontalVelocity = 0f;
			} else if (!facingRight && horizontalVelocity < 0 && !_isForwardGrounded) {
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
			if (_energy.energy >= boostCost && (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))) {
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
					_energy.useEnergy(boostCost);
				}
			} else {
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
			PowerupManager.Instance.process(otherCollider.gameObject.GetComponent<PowerupController>(), gameObject);
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
		// Add inital force to get off ground
		_rigidbody.AddForce(new Vector2(0f, 5 * maximumVelocity));

		// Start flying since you have taken off
		_animator.SetBool("flying", true);

		// If the user does not have a direction key down then stop velocity
		if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f) {
			// delay for a quarter second
			yield return new WaitForSeconds(0.25f);

			// After wait stop from moving
			_rigidbody.velocity = new Vector2(0f, 0f);
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Flips the transform by reversing its scale
	 **/
	public void flipPlayer() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
