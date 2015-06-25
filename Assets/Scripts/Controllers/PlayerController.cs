using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public bool facingRight = true;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float maxSpeed = 5f;
	public float jumpForce = 250f;
	public Transform groundCheck;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isGrounded = false;
	private Rigidbody2D _rigidbody;
	private ClimbController _climbableController;
	private float _originalGravityScale;
	private WeaponController _weapon;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		_rigidbody = GetComponent<Rigidbody2D>();
		_weapon = GetComponentInChildren<WeaponController>();
		_originalGravityScale = _rigidbody.gravityScale;
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// Line cast to the ground check transform to see if it is over a ground layer to prevent double jump
		_isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		
		// Set the default vertical velocity to what it currently is
		float _verticalVelocity = _rigidbody.velocity.y;

		/* ---- HANDLE FIRING GUN ---- */
		if (Input.GetButtonDown("Fire1")) {
			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			_weapon.fire(pos);
		}

		/* ---- HANDLE IF OVER A CLIMBABLE OBJECT ---- */
		if (_climbableController) {
			if (Input.GetAxis("Vertical") > 0) {
				_verticalVelocity = _climbableController.upSpeed;
			} else if (Input.GetAxis("Vertical") < 0) {
				_verticalVelocity = _climbableController.downSpeed * -1;
			} else {
				_verticalVelocity = 0f;
			}
		}

		/* ---- HANDLE MOVING HORIZONTALLY AND VERTICALLY ---- */
		// See what the horizontal axis is to know if moving left or right
		float horizontalAxis = Input.GetAxis("Horizontal");
		_rigidbody.velocity = new Vector2(horizontalAxis * maxSpeed, _verticalVelocity);

		// Handle flipping the transform depending on the direction you are heading
		// TODO: Need to figure out how to handle flipping for gun and movement. Right now they are kinda conflicting
		// as the player aims oposite of movement it is flipping twice per frame
		if (horizontalAxis > 0 && !facingRight) {
			_flipPlayer();
		} else if (horizontalAxis < 0 && facingRight) {
			_flipPlayer();
		}

		// Lastly if the jump flag is set in this frame then apply jump force and trigger flag
		if (Input.GetButtonDown("Jump") && _isGrounded) {
			_rigidbody.AddForce(new Vector2(0f, jumpForce));
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
		if ((relativeMousePos.x < 0 && facingRight) || (relativeMousePos.x > 0 && !facingRight)) {
			_flipPlayer();
		}

		// Calculate the Quaternion and rotate the arm
		Quaternion quat = Quaternion.identity;
		quat.eulerAngles = new Vector3(0, 0, angle);
		arm.transform.rotation = quat;
	}

	/**
	 * @private Flips the transform by reversing its scale
	 **/
	void _flipPlayer() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	/**
	 * @private Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		if (otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = 0;
			_climbableController = otherCollider.gameObject.GetComponent<ClimbController>();
		}
	}

	/**
	 * @private Handles checking if the player is no longer over a climbable object. Sets gravity back
	 **/
	void OnTriggerExit2D(Collider2D otherCollider) {
		if (otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = _originalGravityScale;
			_climbableController = null;
		}
	}
}
