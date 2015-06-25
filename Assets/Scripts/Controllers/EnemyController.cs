﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	/* ---- HIDDEN VARIABLES ---- */
	[HideInInspector] public float horizontalAxis = -1;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float maxSpeed = 5f;							// The maximum speed at which to move
	public float sightRange = 5f;						// The range that the unit can see before engaging the player
	public float patrolDistance = 2f;					// The distance to patrol
	public Transform groundCheck;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isGrounded = false;					// Flag for if the unit is grounded
	private Rigidbody2D _rigidbody;						// The ridged body of the unit
	private GameObject _player;							// The player game object
	private Animator _animator;							// The animator for the current unit
	private float _distanceFromPlayer;					// Distance to the player calculated in update
	private Vector3 _originalPosition;					// The units spawn position
	private float _distanceFromOriginalPosition;		// The distance from the spawn location
	private WeaponController _weapon;					// Weapon controller of the current weapon

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		_rigidbody = GetComponent<Rigidbody2D>();
		_player = GameObject.FindGameObjectWithTag("Player");
		_animator = GetComponent<Animator>();
		_originalPosition = transform.position;
		_weapon = GetComponentInChildren<WeaponController>();
	}
	
	/**
	 * @private Called once per frame handles calculations used durring fixed update
	 **/
	void Update () {
		// Line cast to the ground check transform to see if it is over a ground layer
		_isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		// Figure out the distance from the player
		_distanceFromPlayer = Vector2.Distance(transform.position, _player.transform.position);

		// Figure out haw far ou are from the original location
		_distanceFromOriginalPosition = Vector2.Distance(transform.position, _originalPosition);
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		if (_distanceFromPlayer <= sightRange) { 
			// If within attack range then try and move towards player
			if (transform.position.x > _player.transform.position.x && transform.localScale.x == 1) {
				_flipDirection();
			} else if (transform.position.x < _player.transform.position.x && transform.localScale.x == -1) {
				_flipDirection();
			}

			// If within range then attack 
			if (_distanceFromPlayer <= _weapon.range) {

				if (_weapon.isRanged) {
					_weapon.fire(_player.transform.position);
				} else {
					_animator.SetTrigger("Attack");
				}
			}

			// If the enemy has reached a 
			if (!_isGrounded || _distanceFromPlayer <= _weapon.range) {
				_rigidbody.velocity = new Vector2(0f, 0f);
			} else {
				_move();
			}
		} else {
			// If you are no longer grounded meaning you have found an edge then switch directions
			// this is more of a patrolling mechanic will just keep walking back and forth
			if (!_isGrounded) {
				_flipDirection();
			} else if (_distanceFromOriginalPosition > patrolDistance){
				// Handle only patrolling a distance and if 
				if (transform.position.x > _originalPosition.x && transform.localScale.x == 1) {
					_flipDirection();
				} else if (transform.position.x < _originalPosition.x && transform.localScale.x == -1) {
					_flipDirection();
				}
			} 

			// After charecter has be flipped if needed then move 
			_move();
		}
	}

	void _move() {
		if (_isGrounded) {
			_rigidbody.velocity = new Vector2(horizontalAxis * maxSpeed, _rigidbody.velocity.y);
		}
	}

	/**
	 * @private Flips the transform by reversing its scale
	 **/
	void _flipDirection() {
		horizontalAxis *= -1;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
