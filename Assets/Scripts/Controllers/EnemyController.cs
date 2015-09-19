using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 								     		PUBLIC ENUM											             ///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public enum ENEMY_TYPE {Ground, Flying};

public class EnemyController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public ENEMY_TYPE type = ENEMY_TYPE.Ground;
	public float horizontalAxis = -1;
	public float maxSpeed = 5f;							// The maximum speed at which to move
	public float sightRange = 5f;						// The range that the unit can see before engaging the player
	public float patrolDistance = 2f;					// The distance to patrol
	public float hoverDistance = 2f;
	public Transform groundCheck;
	public Transform gunArm;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private enum TYPES {Ground, Flying};
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
		/* INIT OBJECTS */
		_rigidbody = GetComponent<Rigidbody2D>();
		_player = GameObject.FindGameObjectWithTag("Player");
		_animator = GetComponent<Animator>();
		_weapon = GetComponentInChildren<WeaponController>();

		/* INIT VARIABLES */
		_originalPosition = transform.position;
		_distanceFromPlayer = Vector2.Distance(transform.position, _player.transform.position);
		_distanceFromOriginalPosition = Vector2.Distance(transform.position, _originalPosition);
	}
	
	/**
	 * @private Called once per frame handles calculations used durring fixed update
	 **/
	void Update () {
		if (type == ENEMY_TYPE.Ground) {
			// Line cast to the ground check transform to see if it is over a ground layer
			_isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		}

		// Figure out the distance from the player
		_distanceFromPlayer = Vector2.Distance(transform.position, _player.transform.position);

		// Figure out haw far ou are from the original location
		_distanceFromOriginalPosition = Vector2.Distance(transform.position, _originalPosition);
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// TODO: Temp fix for dropping players onto the platform. If falling do nothing
		if (_rigidbody.velocity.y != 0) {
			return;
		}

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
					_aimWeapon();
					_weapon.fire(_player.transform.position);
				} else {
					_animator.SetTrigger("Attack");
				}
			}

			// If the enemy has reached a 
			if ((type == ENEMY_TYPE.Ground && !_isGrounded) || _distanceFromPlayer <= _weapon.range) {
				_rigidbody.velocity = new Vector2(0f, 0f);
			} else {
				_move();
			}
		} else {
			// If you are no longer grounded meaning you have found an edge then switch directions
			// this is more of a patrolling mechanic will just keep walking back and forth
			if (type == ENEMY_TYPE.Ground && !_isGrounded) {
				_flipDirection();
			} else if (_distanceFromOriginalPosition > patrolDistance) {
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



	/**
	 * @private Aim the arm/weapon at player before firing
	 **/
	void _aimWeapon() {
		if (gunArm != null) {
			/* ---- AIM THE ARM TO FIRE ----*/		
			// Get player and arm position
			Vector3 playerPos = _player.transform.position;
			Vector3 armPos = gunArm.position;
			
			// Get arm and player position relative to the game object
			Vector2 relativeArmPos = new Vector2(armPos.x - 0.5f, armPos.y - 0.5f);
			Vector2 relativePlayerPos = new Vector2 (playerPos.x - 0.5f, playerPos.y - 0.5f) - relativeArmPos;
			float angle = Vector2.Angle (Vector2.down, relativePlayerPos);
			
			// Calculate the Quaternion and rotate the arm
			Quaternion quat = Quaternion.identity;
			quat.eulerAngles = new Vector3(0, 0, angle);
			
			// Rotate the arm pieces
			gunArm.transform.rotation = quat;
		}
	}

	/**
	 * @private Move the enemy unit if grounded
	 **/
	void _move() {
		if (type == ENEMY_TYPE.Ground && _isGrounded) {
			_rigidbody.velocity = new Vector2(horizontalAxis * maxSpeed, _rigidbody.velocity.y);
		} else if (type == ENEMY_TYPE.Flying) {
			_rigidbody.velocity = new Vector2(horizontalAxis * maxSpeed, _rigidbody.velocity.y);
		} else {
			_rigidbody.velocity = new Vector2(0f, 0f);
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
