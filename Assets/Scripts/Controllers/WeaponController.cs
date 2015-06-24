using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	/* ---- PUBLIC VARIABLES ---- */
	public float damage = 5f;						// The amount of damage the current weapon can do
	public float range = 0.75f;						// The range of the weapon before the unit can attack
	public float durability = 1000f;				// The durrability of the weapon, when 0 its broken
	public float durabilityLossPerAttack = 0f;		// The ammount of durrability lost per attack

	public bool isRanged = false;					// Flag for ranged weapons
	public bool isActive = false;					// Flag for when the weapon is active
	public bool isPlayer = false;					// Flag for if the weapon is the player or not

	public float attackSpeed = 0.5f;				// How fast can the weapon attack

	public GameObject projectile;					// For ranged weapons this is the projectile it can fire
	public float projectileSpeed = 1f;				// The speed that a projectile can move

	/* ---- PRIVATE VARIABLES ---- */
	private bool _isFiring = false;					// Flag for when the weapon is being fired
	private int _attackSpeedFrameCounter = 0;		// How many frames should be inbetween each attack

	/**
	 * @public This function is called from the enemy controller to fire a ranged weapon
	 * 
	 * @param $Transform$ target - The transform of the target to fire at
	 **/
	public void fire(Transform target) {
		if (!_isFiring) {
			_attackSpeedFrameCounter = (int)(60 / attackSpeed);
			_isFiring = true;

			// Create the new projectile game object
			GameObject newProjectile = (GameObject)Instantiate(projectile, transform.position, Quaternion.identity);

			// Rotate the new game object towards the target before moving it
			newProjectile.transform.LookAt(target.position);
			newProjectile.transform.Rotate(new Vector3(0,-90,0), Space.Self);

			// "Shoot" the projectile by setting its velocity
			newProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed * (target.position.x - transform.position.x), projectileSpeed * (target.position.y - transform.position.y));

			// Lastly turn on the box collider
			newProjectile.GetComponent<Collider2D>().enabled = true;
		}
	}

	/**
	 * @public This function is called from the health script for when a weapons durrability it 0
	 **/
	public void broken() {
		Destroy(gameObject);
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
}
