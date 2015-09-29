using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float damage = 5f;						// The amount of damage the current weapon can do
	public float range = 0.75f;						// The range of the weapon before the unit can attack
	public float attackSpeed = 0.5f;				// How fast can the weapon attack
	public float durability = 1000f;				// The durrability of the weapon, when 0 its broken
	public float durabilityLossPerAttack = 0f;		// The ammount of durrability lost per attack
	public AudioClip attackSoundEffect;				// Sound effect to make on attack

	public bool isPlayer = false;					// Flag for if the weapon is the player or not
	public bool isRanged = false;					// Flag for ranged weapons

	public GameObject projectile;					// For ranged weapons this is the projectile it can fire

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
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		_animator = GetComponentInParent<Animator>();
	}
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public This function is called from the enemy controller to fire a ranged weapon
	 * 
	 * @param $Vector3$ target - The position of the target to fire at
	 **/
	public void fire(Vector3 target) {
		if (!_isFiring) {
			if (_animator) {
				_animator.SetTrigger("shoot");
			}

			_attackSpeedFrameCounter = (int)(60 / attackSpeed);
			_isFiring = true;

			// Create the new projectile game object
			Vector3 newLocation = transform.position;
			newLocation.z += 0.1f;
			GameObject newProjectile = (GameObject)Instantiate(projectile, newLocation, Quaternion.identity);

			//Get a handle on the projectile controller
			ProjectileController projectileController = newProjectile.GetComponent<ProjectileController>();
			projectileController.isPlayer = isPlayer;
			projectileController.damage = damage;


			// Set the new velocity based on what the weapon is shooting
			// "Shoot" the projectile by setting its velocity
			Vector2 delta = target - transform.position;
			Vector2 projectileVelocity = delta.normalized;
			
			if (projectileController.type == PROJECTILE_TYPE.Laser) {
				// Set the projectile range based on weapon
				projectileController.range = range;

				// Rotate the new game object towards the target before moving it
				newProjectile.transform.LookAt(target);
				newProjectile.transform.Rotate(new Vector3(0,-90,0), Space.Self);
			} else if (projectileController.type == PROJECTILE_TYPE.Bomb) {
				projectileController.range = 100f;
				projectileVelocity = new Vector2(0f, -1f);
			} else if (projectileController.type == PROJECTILE_TYPE.Rocket) {
				newProjectile.transform.LookAt(new Vector3(target.x, newProjectile.transform.position.y, target.z));
				newProjectile.transform.Rotate(new Vector3(0,-90,0), Space.Self);
				projectileVelocity.y = 0f;
			}

			//Set the velocity of the projectile
			newProjectile.GetComponent<Rigidbody2D>().velocity = projectileVelocity * projectileController.speed;

			// Lastly turn on the box collider
			newProjectile.GetComponent<Collider2D>().enabled = true;

			// Make sound effect
			SpecialEffectsManager.Instance.makeSound(attackSoundEffect);
		}
	}

	/**
	 * @public This function is called from the health script for when a weapons durrability it 0
	 **/
	public void broken() {
		Destroy(gameObject);
	}
}

