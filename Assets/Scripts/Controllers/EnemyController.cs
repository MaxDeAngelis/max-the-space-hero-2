using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 								     		PUBLIC ENUM											             ///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public enum ENEMY_TYPE {Ground, Flying, Bomber};
public enum PATROL {Horizontal, Vertical};

public class EnemyController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* GENERAL VARIABLES */
	public ENEMY_TYPE type = ENEMY_TYPE.Ground;			// The type of enemy this is
	public float maxSpeed = 5f;							// The maximum speed at which to move
	public float sightRange = 5f;						// The range that the unit can see before engaging the player
	public float patrolDistance = 2f;					// The distance to patrol

	/* SUPPORTING OBJECTS */
	public Transform groundCheck;						// Ground check is used to see if the enemy is over ground
	public Transform gunArm;							// The arm game object holding a weapon used for aiming

	/* FLYING ONLY VARIABLES */
	public PATROL patrolDirection = PATROL.Horizontal;	// The direction the unit should be patrolling
	public float hoverHeight;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* BASIC PRIVATE VARIABLES*/
	private bool _isGrounded = false;					// Flag for if the unit is grounded
	private bool _returnToPatrol = false;
	private float _distanceFromTarget;					// Distance to the player calculated in update
	private float _distanceFromOriginalPosition;		// The distance from the spawn location
	private Vector2 _target;
	private Vector3 _directionModifier;
	private Vector3 _originalPosition = Vector3.zero;					// The units spawn position
	private Vector3 _playerLocation;

	/* SUPPORTING COMPONENTS */
	private Rigidbody2D _rigidbody;						// The ridged body of the unit
	private WeaponController _weapon;					// Weapon controller of the current weapon

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		/* INIT OBJECTS */
		_rigidbody = GetComponent<Rigidbody2D>();
		_weapon = GetComponentInChildren<WeaponController>();

		/* INIT VARIABLES */
		_directionModifier = transform.localScale;
		_originalPosition = transform.position;

		/* INIT DISTANCE VARIABLES */
		_calculateTargetAndOriginDistance();
	}
	
	/**
	 * @private Called once per frame handles calculations used durring fixed update
	 **/
	void Update () {
		if (type == ENEMY_TYPE.Ground) {
			// Line cast to the ground check transform to see if it is over a ground layer
			_isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		}

		/* Recalculate distance variables */
		_calculateTargetAndOriginDistance();
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// If within attack range then try and move towards player
		if (_distanceFromTarget <= sightRange) { 
			_returnToPatrol = true;

			_calculateDirection(_target);

			// If within range then attack 
			if (_distanceFromTarget <= _weapon.range) {
				if (_weapon.type == WEAPON_TYPE.Ranged) {
					_aimWeapon();

					// If there is a gun arm use that for origin
					if (gunArm != null) {
						_weapon.fire(gunArm.transform.position, _weapon.transform.position);
					} else {
						_weapon.fire(_weapon.transform.position, _playerLocation);
					}
				}
			}

			// If the enemy has reached a spot they can attack from then stop moving. Else keep moving
			if ((type == ENEMY_TYPE.Flying || type == ENEMY_TYPE.Bomber) && !_isGrounded && _distanceFromTarget > _weapon.range) {
				_moveToLocation(_target);
			} else {
				_move();
			}
		} else if (_returnToPatrol && _distanceFromOriginalPosition > 0.25f) {
			// Calculate what way to face when returning to original position
			_calculateDirection(_originalPosition);

			// Move back to the original position
			_moveToLocation(_originalPosition);
		} else {
			_returnToPatrol = false;

			// If you are no longer grounded meaning you have found an edge then switch directions
			// this is more of a patrolling mechanic will just keep walking back and forth
			if (type == ENEMY_TYPE.Ground && !_isGrounded) {
				_flipDirection();
			} else if (_distanceFromOriginalPosition > patrolDistance) {
				// Handle only patrolling a distance and if 
				_calculateDirection(_originalPosition);
			} 
			
			// After charecter has be flipped if needed then move 
			_move();
		}
	}

	/**
	 * @private checks and sets distance from target and origin values
	 **/
	void _calculateTargetAndOriginDistance() {
		// Get the player location
		_playerLocation = PlayerManager.Instance.getLocation();

		// Calculate the target
		_target = new Vector2(_playerLocation.x, _playerLocation.y - hoverHeight);

		// Figure out the distance from the target
		_distanceFromTarget = Vector2.Distance(transform.position, _target);
		
		// Figure out haw far ou are from the original location
		_distanceFromOriginalPosition = Vector3.Distance(transform.position, _originalPosition);
	}

	/**
	 * @private handles changing the direction of the transform based on a target position
	 * 
	 * @param Vector3 target - The target position to base the flip on 
	 **/
	void _calculateDirection(Vector3 target) {
		if (patrolDirection == PATROL.Horizontal) {
			// Changes the enemies direction based on what way they are moving
			// 1. Moving RIGHT -> change to moving LEFT
			// 1. Moving LEFT -> change to moving RIGHT
			if (transform.position.x > target.x && _directionModifier.x == 1) {
				_flipDirection();
			} else if (transform.position.x < target.x && _directionModifier.x == -1) {
				_flipDirection();
			}
		} else if (patrolDirection == PATROL.Vertical) {
			// Changes the enemies direction based on what way they are moving
			// 1. Moving UP -> change to moving DOWN
			// 1. Moving DOWN -> change to moving UP
			if (transform.position.y > target.y && _directionModifier.y == 1) {
				_flipDirection();
			} else if (transform.position.y < target.y && _directionModifier.y == -1) {
				_flipDirection();
			}
		}
	}

	/**
	 * @private Aim the arm/weapon at player before firing
	 **/
	void _aimWeapon() {
		// Drop out if paused
		if (!GameManager.Instance.isPaused() && gunArm != null) {
			/* ---- AIM THE ARM TO FIRE ----*/		
			// Get player and arm position
			Vector3 playerPos = _playerLocation;
			playerPos.y += 0.25f;

			// Calculate the upward rotation
			Vector3 upward = gunArm.transform.position - playerPos;
			
			// If not facing right invert x for correct rotation
			upward.x *= transform.localScale.x;
			
			// Set the look rotation of the arm
			gunArm.transform.rotation = Quaternion.LookRotation(Vector3.forward, upward);
		}
	}

	/**
	 * @private move to position given requardless of direction
	 * 
	 * @param Vector3 target - the vector location to move to
	 **/
	void _moveToLocation(Vector3 target) {
		Vector2 delta = target - transform.position;
		_rigidbody.velocity = delta.normalized * maxSpeed;
	}

	/**
	 * @private Move the enemy unit if grounded
	 **/
	void _move() {
		if (type == ENEMY_TYPE.Ground && !_isGrounded || _distanceFromTarget <= _weapon.range) {
			_rigidbody.velocity = new Vector2(0f, 0f);
		} else if (patrolDirection == PATROL.Horizontal) {
			_rigidbody.velocity = new Vector2(_directionModifier.x * maxSpeed, 0f);
		} else if (patrolDirection == PATROL.Vertical) {
			_rigidbody.velocity = new Vector2(0f, _directionModifier.y * maxSpeed);
		}
	}

	/**
	 * @private Flips the transform by reversing its scale
	 **/
	void _flipDirection() {
		// If patrolling horizontally then flip x scale else flip y
		if (patrolDirection == PATROL.Horizontal) {
			_directionModifier.x *= -1;
		} else {
			_directionModifier.y *= -1;
		}

		// If patrolling horizontally then actually flip the transform
		if (patrolDirection == PATROL.Horizontal) {
			transform.localScale = _directionModifier;
		}
	}

	/**
	 * @private draws gizmos when the unit is selected
	 **/
	void OnDrawGizmosSelected () {
		// If the _original position is not set meeing the game is not running use transform pos
		if (_originalPosition == Vector3.zero) {
			_originalPosition = transform.position;
		}

		// Init patrol points
		Gizmos.color = Color.green;
		Vector3 patrolPointOne = _originalPosition;
		Vector3 patrolPointTwo = _originalPosition;

		// Calculate patrol end points
		if (patrolDirection == PATROL.Horizontal) {
			patrolPointOne.x += patrolDistance;
			patrolPointTwo.x -= patrolDistance;
		} else {
			patrolPointOne.y += patrolDistance;
			patrolPointTwo.y -= patrolDistance;
		}

		// DRAW PATROL POINTS AND LINE
		Gizmos.DrawSphere(patrolPointOne, 0.25f);
		Gizmos.DrawSphere(patrolPointTwo, 0.25f);
		Gizmos.DrawLine(patrolPointOne, patrolPointTwo);

		// DRAW SIGHT SPHERE
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, sightRange);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


}
