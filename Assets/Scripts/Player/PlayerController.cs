using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* ---- MOVEMENT RELATED ---- */
	public float movementSpeed = 5f;			// Grounded movement speed

	/* ---- SUPPORTING TRANSFORMS ---- */
	public Transform forwardGroundCheck;		// Transform to check if front of player is on ground
	public Transform backwardGroundCheck;		// Transform to check if back of player is on ground
	public Transform gunArm;					// Transform of the arm holding the gun

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isForwardGrounded = false;	// Flag for when the front of the player is grounded
	private bool _isBackwardGrounded = false;	// Flag for whant the back of the player is grounded
	private bool _isClimbing = false;			// Flag for when player is actually climbing
	private float _originalGravityScale;		// Starting gravity 
	private string[] _groundLayers = new string[2] {"Ground", "Climbable"}; // List of layers to consider ground
	private GameObject _currentPlatform;

	/* VARIABLES FOR WAITING ANIMATION CHECK */
	private int _framesBeforeWait = 60 * 10;	// Amount of frames to count before considered waiting
	private int _frameWaitCounter = 0;			// Frame counter for wait animation
	private Vector3 _previousVelocity;			// Previous frames velocity for max
	private Vector2 _previousMousePosition;		// Previous frames mouse position
	private Quaternion _defaultArmRotation;     // The default arm rotation at start

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Rigidbody2D _rigidbody;
	private Climbable _climbable;
	private Animator _animator;

	/* ---- MANAGERS ---- */
	private PowerupManager _powerupManager;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		/* INIT COMPONENTS */
		_rigidbody = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();

		/* INIT MANAGERS */
		_powerupManager = PowerupManager.Instance;

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
		// Line cast to the ground check transform to see if it is over a ground layer
		_checkIfGrounded();
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		/* ---- HANDLE MOVEMENT ---- */
		if (PlayerManager.Instance.isAnchored()) {
			// Check if Max is waiting
			_checkIfWaiting();

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

				/* ---- CHECK IF USER IS GOING TO FALL ---- */ 
				if (_checkIfFalling(horizontalVelocity)) { 
					// FACING RIGHT MOVING RIGHT
					horizontalVelocity = 0f;
				}
			}

			// Set speed floats for  animations
			_animator.SetFloat("horizontalSpeed", Mathf.Abs(_rigidbody.velocity.x));
			_animator.SetFloat("verticalSpeed", Mathf.Abs(_rigidbody.velocity.y));

			/* ---- HANDLE MOVING HORIZONTALLY AND VERTICALLY ---- */
			_rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
		}
	}

	/**
	 * @private Checks if the player is falling off of a platform 
	 * 
	 * @param $Float$ horizontalVelocity - The current horizontal velocity
	 **/
	bool _checkIfFalling(float horizontalVelocity) {
		bool _isFacingRight = (transform.localScale.x > 0);
		/* ---- CHECK IF USER IS GOING TO FALL ---- */ 
		if (_isFacingRight && horizontalVelocity > 0 && !_isForwardGrounded) { 
			// FACING RIGHT MOVING RIGHT
			return true;
		} else if (!_isFacingRight && horizontalVelocity > 0 && !_isBackwardGrounded) {
			// FACING LEFT MOVING RIGHT
			return true;
		} else if (_isFacingRight && horizontalVelocity < 0 && !_isBackwardGrounded) {
			// FACING RIGHT MOVING LEFT
			return true;
		} else if (!_isFacingRight && horizontalVelocity < 0 && !_isForwardGrounded) {
			// FACING LEFT MOVING RIGHT
			return true;
		}

		return false;
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
		// Store off a reference to the current platform you are one
		if (isGrounded() && otherCollider.tag == "Ground") {
			_currentPlatform = otherCollider.gameObject;
		}

		// Ladder logic to allow climbing
		if (PlayerManager.Instance.isAnchored() && otherCollider.gameObject.GetComponent<Climbable>() != null) {
			_rigidbody.gravityScale = 0;
			_climbable = otherCollider.gameObject.GetComponent<Climbable>();
		}

		// Process if you hit a powerup, call manager to gain boost
		if (PlayerManager.Instance.isAnchored() && otherCollider.gameObject.tag == "Powerup") {
			_powerupManager.process(otherCollider.gameObject.GetComponent<PowerupController>(), gameObject);
		}
	}

	/**
	 * @private Handles checking if the player is no longer over a climbable object. Sets gravity back
	 **/
	void OnTriggerExit2D(Collider2D otherCollider) {
		if (PlayerManager.Instance.isAnchored() && otherCollider.gameObject.GetComponent<Climbable>() != null) {
			_rigidbody.gravityScale = _originalGravityScale;
			_climbable = null;
			_isClimbing = false;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public called to see if player if grounded
	 **/
	public bool isGrounded() {
		return !PlayerManager.Instance.isFlying();
	}

	/**
	 * @public called to see if player if climbing
	 **/
	public bool isClimbing() {
		return _isClimbing;
	}

	/**
	 * @public returns the platform that the player is standing on
	 **/
	public GameObject getCurrentPlatform() {
		return _currentPlatform;
	}
}
