using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float movementSpeed = 5f;
	public float boostForce = 10f;
	public float maximumVelocity = 18f;
	public Transform forwardGroundCheck;
	public Transform backwardGroundCheck;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isForwardGrounded = false;
	private bool _isBackwardGrounded = false;
	private bool _isAnchored = true;
	private bool _facingRight = true;
	private Rigidbody2D _rigidbody;
	private BoxCollider2D _collider;
	private ClimbController _climbableController;
	private float _originalGravityScale;
	private WeaponController _weapon;
	private LandingController _landingController;
	private EnergyController _energyController;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		_rigidbody = GetComponent<Rigidbody2D>();
		_collider = GetComponent<BoxCollider2D>();
		_weapon = GetComponentInChildren<WeaponController>();
		_landingController = GetComponentInChildren<LandingController>();
		_energyController = GetComponent<EnergyController>();
		_originalGravityScale = _rigidbody.gravityScale;
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// Line cast to the ground check transform to see if it is over a ground layer to prevent double jump
		_isForwardGrounded = Physics2D.Linecast(transform.position, forwardGroundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		_isBackwardGrounded = Physics2D.Linecast(transform.position, backwardGroundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		/* ---- HANDLE FIRING GUN ---- */
		if (Input.GetButtonDown("Fire1")) {
			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			_weapon.fire(pos);
		}

		/* ---- HANDLE ANCHORING ---- */
		if (Input.GetButtonDown("Jump") && _isAnchored && !_climbableController && (_isForwardGrounded || _isBackwardGrounded)) {
			_isAnchored = false;
			_rigidbody.gravityScale = 0;
			_collider.isTrigger = true;

			StartCoroutine(_takeoff());
		} else if (Input.GetButtonDown("Jump") && !_isAnchored && _landingController.isAbleToLand) {
			_isAnchored = true;
			_collider.isTrigger = false;
			_rigidbody.gravityScale = _originalGravityScale;
			_rigidbody.velocity = new Vector2(0f, 0f);
		}

		/* ---- AIM THE ARM TO FIRE ----*/
		// Get a reference the the game object of the arm
		GameObject arm = _weapon.transform.parent.gameObject;

		// Get mouse position and arm position
		Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		Vector3 armPos = Camera.main.WorldToViewportPoint (arm.transform.position);

		// Get arm and mouse position relative to the game object
		Vector2 relativeArmPos = new Vector2(armPos.x - 0.5f, armPos.y - 0.5f);
		Vector2 relativeMousePos = new Vector2 (mousePos.x - 0.5f, mousePos.y - 0.5f) - relativeArmPos;

		// Calculate the angle
		float angle = Vector2.Angle (Vector2.down, relativeMousePos);

		// Flip the player if aiming in the opposite direction
		if ((relativeMousePos.x < 0 && _facingRight) || (relativeMousePos.x > 0 && !_facingRight)) {
			_flipPlayer();
		}

		// Calculate the Quaternion and rotate the arm
		Quaternion quat = Quaternion.identity;
		quat.eulerAngles = new Vector3(0, 0, angle);
		arm.transform.rotation = quat;

		/* ---- HANDLE MOVEMENT ---- */
		if (_isAnchored) {
			// Set the default vertical/horizontal velocity to what it currently is
			float verticalVelocity = _rigidbody.velocity.y;
			float horizontalVelocity = Input.GetAxis("Horizontal") * movementSpeed;

			/* ---- HANDLE IF OVER A CLIMBABLE OBJECT ---- */
			if (_climbableController) {
				if (Input.GetAxis("Vertical") > 0) {
					verticalVelocity = _climbableController.upSpeed;
				} else if (Input.GetAxis("Vertical") < 0) {
					verticalVelocity = _climbableController.downSpeed * -1;
				} else {
					verticalVelocity = 0f;
				}
			}

			/* ---- CHECK IF USER IS GOING TO FALL ---- */ 
			if (_facingRight && horizontalVelocity > 0 && !_isForwardGrounded) { 
				// FACING RIGHT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (!_facingRight && horizontalVelocity > 0 && !_isBackwardGrounded) {
				// FACING LEFT MOVING RIGHT
				horizontalVelocity = 0f;
			} else if (_facingRight && horizontalVelocity < 0 && !_isBackwardGrounded) {
				// FACING RIGHT MOVING LEFT
				horizontalVelocity = 0f;
			} else if (!_facingRight && horizontalVelocity < 0 && !_isForwardGrounded) {
				// FACING LEFT MOVING RIGHT
				horizontalVelocity = 0f;
			}

			/* ---- HANDLE MOVING HORIZONTALLY AND VERTICALLY ---- */
			_rigidbody.velocity = new Vector2(horizontalVelocity, verticalVelocity);
		} else {
			// If your magnitude is too high you are moving too fast so start throttling down
			if (_rigidbody.velocity.sqrMagnitude < maximumVelocity) {
				// If there is energy left then you can boost
				if (_energyController.energy >= 1f) {
					// If you are pressing a key use some energy
					if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
						_energyController.useEnergy(1f);
					}

					// Apply thrust
					_rigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * boostForce, Input.GetAxis("Vertical") * boostForce));
				}
			} else {
				_rigidbody.velocity *= 0.99f;
			}
		}
	}

	/**
	 * @private Flips the transform by reversing its scale
	 **/
	void _flipPlayer() {
		_facingRight = !_facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	/**
	 * @private checks to see if the player is bumping into the boundries and if so it stops the player
	 **/
	void _checkIfHittingBoundry(Collider2D otherCollider) {
		Vector3 colliderCenter = otherCollider.bounds.center;
		Vector3 colliderExtents =  otherCollider.bounds.extents;
		float playerHalfWidth = _collider.bounds.extents.x;
		float playerHalfHeight = _collider.bounds.extents.y;

		if (otherCollider.gameObject.tag == "LEFT_BOUNDRY" && Input.GetAxis("Horizontal") <= 0){
			// Lock the players horizontal position and velocity
			transform.position = new Vector3((colliderCenter.x + colliderExtents.x + playerHalfWidth), transform.position.y);
			_rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
		} else if (otherCollider.gameObject.tag == "RIGHT_BOUNDRY" && Input.GetAxis("Horizontal") >= 0) {
			// Lock the players horizontal position and velocity
			transform.position = new Vector3((colliderCenter.x - colliderExtents.x - playerHalfWidth), transform.position.y);
			_rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
		} else if (otherCollider.gameObject.tag == "TOP_BOUNDRY" && Input.GetAxis("Vertical") >= 0) {
			// Lock the players vertical position and velocity
			transform.position = new Vector3(transform.position.x, (colliderCenter.y - colliderExtents.y - playerHalfHeight));
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
		} else if (otherCollider.gameObject.tag == "BOTTOM_BOUNDRY" && Input.GetAxis("Vertical") <= 0) {
			// Lock the players vertical position and velocity
			transform.position = new Vector3(transform.position.x, (colliderCenter.y + colliderExtents.y + playerHalfHeight));
			_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
		}
	}
	/**
	 * @private Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		_checkIfHittingBoundry(otherCollider);

		if (_isAnchored && otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = 0;
			_climbableController = otherCollider.gameObject.GetComponent<ClimbController>();
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
			_climbableController = null;
		}
	}

	/**
	 * @private Called when taking off from an anchored position
	 **/
	IEnumerator _takeoff() {
		// Add inital force to get off ground
		_rigidbody.AddForce(new Vector2(0f, 5 * maximumVelocity));

		// If the user does not have a direction key down then stop velocity
		if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f) {
			// delay for a quarter second
			yield return new WaitForSeconds(0.25f);

			// After wait stop from moving
			_rigidbody.velocity = new Vector2(0f, 0f);
		}
	}
}
