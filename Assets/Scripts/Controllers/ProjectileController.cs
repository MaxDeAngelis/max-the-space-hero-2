using UnityEngine;
using System.Collections;

public enum PROJECTILE_TYPE {Laser, Bomb, Rocket};

public class ProjectileController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool isPlayer = false;								// Flag for if the projectile if coming from the player
	public PROJECTILE_TYPE type = PROJECTILE_TYPE.Laser;		// The type of the projectile
	public float range;											// The range before the projectile destroys
	public float damage;										// The damage
	public float speed = 5f;									// The speed of the projectile
	public AudioClip explosion;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _distanceTraveled;
	private Vector3 _origin;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS   										     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start () {
		_origin = transform.position;
	}
	
	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate () {
		_distanceTraveled = Vector2.Distance(transform.position, _origin);

		// If the projectile has gone past its range then destroy it
		if (_distanceTraveled >= range) {
			Destroy(gameObject);
		}
	}

	/**
	 * @private Collider handler that is triggered when another collider interacts with this game object
	 * 
	 * @param $Collider2D$ otherCollider - The collider that is interacting with this game object
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// If the current projectile is a bomb and it hits ground then blow up
		if (type == PROJECTILE_TYPE.Bomb && otherCollider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
			SpecialEffectsManager.Instance.makeExplosion(transform.position, explosion);

			hit();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public This function is called when the projectile hits something
	 **/
	public void hit() {
		Destroy(gameObject);
	}
}
