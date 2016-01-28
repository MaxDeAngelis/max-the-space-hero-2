using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 								     		PUBLIC ENUM											             ///
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public enum WEAPON_TYPE {Ranged, Melee, Suicide};			// Enum for options in the type drop down

public class Weapon : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* GENERAL VARIABLE */
	public WEAPON_TYPE type = WEAPON_TYPE.Ranged;	// The type of weapon
	public bool isPlayer = false;					// Flag for if the weapon is the player or not
	public float damage = 5f;						// The amount of damage the current weapon can do
	public float attackSpeed = 0.5f;				// How fast can the weapon attack
	public AudioClip attackSoundEffect;				// Sound effect to make on attack

	/* RANGED VARIABLES */
	public GameObject projectile;					// For ranged weapons this is the projectile it can fire
	public float range = 0.75f;						// The range of the weapon before the unit can attack

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isFiring = false;					// Flag for when the weapon is being fired
	private int _attackSpeedFrameCounter = 0;		// How many frames should be inbetween each attack
	private Animator _animator;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		// If you are firing and attack counter is 0 or less then you are no longer fireing otherwise increment count
		if (_isFiring && _attackSpeedFrameCounter <= 0) {
			_isFiring = false;
		} else {
			_attackSpeedFrameCounter--;
		}
	}

	/**
	 * @protected Called to actually do the work of attacking
	 * 
	 * @param $Vector3$ origin - The origin of the weapon
	 * @param $Vector3$ target - The target of the attack
	 * @param $Float$ dmg - The amount of damage to do
	 * @param $GameObject$ shot - The game object of the projectile to create
	 * @param $AudioClip$ sound - The sound to make 
	 * @param $Boolean$ firedFromGround - Flag for if the weapon is being used from the ground
	 **/
	protected void attack(Vector3 origin, Vector3 target, float dmg, GameObject shot, AudioClip sound, bool firedFromGround) {
		if (!_isFiring) {
			if (_animator) {
				_animator.SetTrigger("shoot");
			}

			_attackSpeedFrameCounter = (int)(60 / attackSpeed);
			_isFiring = true;

			// Create the new projectile game object
			Vector3 newLocation = transform.position;
			newLocation.z += 0.1f;
			GameObject newProjectile = (GameObject)Instantiate(shot, newLocation, Quaternion.identity);

			//Get a handle on the projectile controller
			Projectile projectileController = newProjectile.GetComponent<Projectile>();
			projectileController.isPlayer = isPlayer;
			projectileController.damage = dmg;
			projectileController.isFiredFromGround = firedFromGround;
			projectileController.range = range;

			// Set the new velocity based on what the weapon is shooting
			// "Shoot" the projectile by setting its velocity
			Vector2 delta = target - origin;
			Vector2 projectileVelocity = delta.normalized;
		
			// Adjust the projectile itself and its velocity if needed
			projectileVelocity = adjustProjectile(origin, target, newProjectile, projectileVelocity);

			//Set the velocity of the projectile
			newProjectile.GetComponent<Rigidbody2D>().velocity = projectileVelocity * projectileController.speed;

			// Lastly turn on the box collider
			newProjectile.GetComponent<BoxCollider2D>().enabled = true;

			// Make sound effect
			SpecialEffectsManager.Instance.playSound(sound);
		}
	}

	/**
	 * @private draws gizmos when the unit is selected
	 **/
	void OnDrawGizmosSelected () {
		// DRAW WEAPON RANGE
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @private Called on start of the game object to init variables
	**/
	public virtual void Start() {
		_animator = GetComponentInParent<Animator>();
	}
		
	/**
	 * @public handles exploding the weapon in the case of a suicide bomber
	 **/
	public void explode() {
		SpecialEffectsManager.Instance.playExplosion(transform.position, attackSoundEffect);

		Destroy(gameObject);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		VIRTUAL FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @protected Called from attack to adjust the projectiles rotation. Overriden in different weapon classes
	 *
	 * @param $Vector3$ origin - The position of the origin of the gun
	 * @param $Vector3$ target - The position of the target to fire at
	 * @param $GameObject$ projectile - The projectile game object to adjust its location
	 * @param $Vector2$ velocity - The projectile velocity
	 **/
	protected virtual Vector2 adjustProjectile(Vector3 origin, Vector3 target, GameObject projectile, Vector2 velocity) {
		return velocity;
	}

	/**
	* @public This function is called from the enemy controller to fire a ranged weapon
	* 
	* @param $Vector3$ origin - The position of the origin of the gun
	* @param $Vector3$ target - The position of the target to fire at
	* @param $Boolean$ firedFromGround - Is the weapon fired from the ground
	**/
	public virtual void fire(Vector3 origin, Vector3 target, bool firedFromGround) {
		attack(origin, target, damage, projectile, attackSoundEffect, firedFromGround);
	}
}

