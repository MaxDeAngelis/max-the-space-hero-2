﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float maxSpeed = 5f;
	public float boostForce = 3f;
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
		if (Input.GetButtonDown("Jump") && _isAnchored && !_climbableController) {
			_isAnchored = false;
			_rigidbody.gravityScale = 0;
			_collider.isTrigger = true;
			_rigidbody.AddForce(new Vector2(0f, 5f));
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
			float horizontalVelocity = Input.GetAxis("Horizontal") * maxSpeed;

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
			_rigidbody.AddForce(new Vector2(Input.GetAxis("Horizontal") * boostForce, Input.GetAxis("Vertical") * boostForce));
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
	 * @private Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		if (_isAnchored && otherCollider.gameObject.GetComponent<ClimbController>() != null) {
			_rigidbody.gravityScale = 0;
			_climbableController = otherCollider.gameObject.GetComponent<ClimbController>();
		}
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
}
