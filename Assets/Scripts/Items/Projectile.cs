using UnityEngine;
using System.Collections;

public enum PROJECTILE_TYPE {Laser, Bomb, Rocket};

public class Projectile : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool isPlayer = false;								// Flag for if the projectile if coming from the player
	public bool isFiredFromGround;								// Flag for if the projectile was fired from the ground
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
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Start () {
		_origin = transform.position;
	}

	/// <summary>
	/// Called 60times per second fixed, handles all processing
	/// </summary>
	void FixedUpdate () {
		_distanceTraveled = Vector2.Distance(transform.position, _origin);

		// If the projectile has gone past its range then destroy it
		if (_distanceTraveled >= range) {
			Destroy(gameObject);
		}
	}
		
	/// <summary>
	/// Collider handler that is triggered when another collider interacts with this game object
	/// </summary>
	/// <param name="otherCollider">The collider that is interacting with this game object</param>
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// If the current projectile is a bomb and it hits ground then blow up
		if (type == PROJECTILE_TYPE.Bomb && otherCollider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
			// Play a small explosion for the bomb exploding
			SpecialEffectsManager.Instance.playSmallExplosion(transform.position, explosion);

			// Fire the hit to get rid of the bomb
			hit();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// This function is called when the projectile hits something
	/// </summary>
	public void hit() {
		Destroy(gameObject);
	}
}
