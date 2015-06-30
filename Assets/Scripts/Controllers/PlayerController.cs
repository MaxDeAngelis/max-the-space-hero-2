using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
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
	private bool _facingRight = true;			// Flag for if the player is facing the right 
	private float _originalGravityScale;		// Starting gravity 
	private Vector3 _boundryIntersectPosition;	// The position the player was in as he intersects with a boundry

	/* ---- OBJECTS/CONTROLLERS ---- */
	private Rigidbody2D _rigidbody;
	private BoxCollider2D _collider;
	private ClimbController _climbable;
	private WeaponController _weapon;
	private LandingController _landing;
	private EnergyController _energy;

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
		_landing = GetComponentInChildren<LandingController>();
		_energy = GetComponent<EnergyController>();
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
			if (_climbable) {
				if (Input.GetAxis("Vertical") > 0) {
					verticalVelocity = _climbable.upSpeed;
				} else if (Input.GetAxis("Vertical") < 0) {
					verticalVelocity = _climbable.downSpeed * -1;
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

			// If direction key is down and there is enough energy then boost
			if (_energy.energy >= boostCost && (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))) {
				// Calculate artifical drag force
				float drag = boostForce / maximumVelocity;

				// Get the direction based on keys down
				Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

				// Calculate the new velocity using the fage drag
				Vector2 newVelocity = (direction * boostForce) - (_rigidbody.velocity * drag);

				// Apply the velocity over time
				_rigidbody.velocity += newVelocity * Time.deltaTime ;

				// If you are moving using energy
				if (Mathf.Abs(newVelocity.x) > 0.25f || Mathf.Abs(newVelocity.y) > 0.25f) {
					_energy.useEnergy(boostCost);
				}
			}







			/* OPTION 1 - Separate control of force. Shitty! Move much faster on diagnals
			if (_rigidbody.velocity.sqrMagnitude > maximumVelocity) {
				_rigidbody.velocity = _rigidbody.velocity.normalized * maximumVelocity;
			} else {
				if (Input.GetAxis("Horizontal") != 0) {

					_rigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * boostForce, 0f));

					if (_rigidbody.velocity.x > maximumVelocity){
					//	_rigidbody.velocity = new Vector2(maximumVelocity, _rigidbody.velocity.y);
					}
				}

				if (Input.GetAxis("Vertical") != 0) {
					_rigidbody.AddForce(new Vector2(0f, Input.GetAxis("Vertical") * boostForce));

					if (_rigidbody.velocity.y > maximumVelocity) {
					//	_rigidbody.velocity = new Vector2(_rigidbody.velocity.x, maximumVelocity);
					}
				}
			}*/


			/* OPTION 2 - Original approach with a coupld cap ideas not idea because you cant turn at high speed
			 * also does not seem to be a way to find if at max speed
			Debug.Log(_rigidbody.velocity.sqrMagnitude);
			if (_rigidbody.velocity.sqrMagnitude < maximumVelocity && _energy.energy >= boostCost) {
				// If your magnitude is too high you are moving too fast so start throttling down
				//Debug.Log("Yes");
				// If you are pressing a key use some energy
				if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
					_energy.useEnergy(boostCost);
				}

				// Apply thrust
				// TODO: The bellow calculations were an attempt to figure out how to make thrust proportanal to 
				// the time when the player tries to move. Since Input.GetAxis starts really small and build 
				// I thought I could subtract from a whole number to get a larger fraction then user that as a
				// multiplier
				float horizontalBoostPower = 0f;
				if (Input.GetAxis("Horizontal") != 0) {
					horizontalBoostPower = (2 - Mathf.Abs(Input.GetAxis("Horizontal"))) * boostForce;
					if (Input.GetAxis("Horizontal") < 0) {
						horizontalBoostPower *= -1; 
					}
				}

				float verticalBoostPower = 0f;
				if (Input.GetAxis("Vertical") != 0) {
					verticalBoostPower = (2 - Mathf.Abs(Input.GetAxis("Vertical"))) * boostForce;
					if (Input.GetAxis("Vertical") < 0) {
						verticalBoostPower *= -1; 
					}
				}

				_rigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * boostForce, Input.GetAxis("Vertical") * boostForce));
			} else if (_rigidbody.velocity.sqrMagnitude > maximumVelocity) {
				//Debug.Log("Not");
				_rigidbody.velocity *= 0.99f;
			}*/
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
