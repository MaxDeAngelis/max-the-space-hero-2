using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static PlayerManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private GameObject _player;
	private ParticleSystem _particle;
	private PlayerHealth _health;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of PlayerManager!");
		}
		Instance = this;

		/* INIT VARIABLES */
		_player = GameObject.FindGameObjectWithTag("Player");
		_particle = _player.GetComponent<ParticleSystem>();
		_health = _player.GetComponent<PlayerHealth>();
	}

	/// <summary>
	/// Called on level load to place the player on the spawn
	/// </summary>
	/// <param name="level">The level number</param>
	void OnLevelWasLoaded(int level) {
		GameObject _spawn = GameObject.FindGameObjectWithTag("Respawn");

		if (_spawn) {
			_player.transform.position = _spawn.transform.position;
		}
	}

	/// <summary>
	/// Called from use to trigger an effect to show the powerup was used
	/// </summary>
	/// <param name="duration">Duration of particle effect</param>
	/// <param name="color">Color of the particle effect</param>
	IEnumerator _playParticleEffect(float duration, Color color) {
		// Make sure the alpha is set to 1, for some reason it seems to be 0 alot of the time
		color.a = 1f;

		// Play the particle effect for use if there is one
		_particle.startColor = color;
		_particle.Play();

		// delay for a quarter second
		yield return new WaitForSeconds(duration);
		
		// Destroy after affect is done
		_particle.Stop();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Plays the particle system attached to the player for the given durration with the given color
	/// </summary>
	/// <param name="duration">Duration of particle effect</param>
	/// <param name="color">Color of the particle effect</param>
	public void playParticleEffect(float duration, Color color) {
		StartCoroutine(_playParticleEffect(duration, color));
	}

	/// <summary>
	/// Reset the player's components
	/// </summary>
	public void reset() {
		getShield().reset();
		getJetpack().reset();
		getHealthController().reset();
		getController().reset();

		// Reset the energy manager
		EnergyManager.Instance.reset();

		// Finally set active
		getTransform().gameObject.SetActive(true);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	    	  FLAGS 	  							     	    	     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Returns true if the player is anchored
	/// </summary>
	/// <returns><c>true</c>, if player is anchored, <c>false</c> otherwise.</returns>
	public bool isAnchored() {
		return getJetpack().isAnchored();
	}

	/// <summary>
	/// Returns true if the player is flying or not
	/// </summary>
	/// <returns><c>true</c>, if player is flying, <c>false</c> otherwise.</returns>
	public bool isFlying() {
		return getJetpack().isFlying();
	}

	/// <summary>
	/// Returns true if the player is grounded
	/// </summary>
	/// <returns><c>true</c>, if player is grounded, <c>false</c> otherwise.</returns>
	public bool isGrounded() {
		return getController().isGrounded();
	}

	/// <summary>
	/// Returns true if the player is climbing something
	/// </summary>
	/// <returns><c>true</c>, if player is climbing, <c>false</c> otherwise.</returns>
	public bool isClimbing() {
		return getController().isClimbing();
	}
		
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	    	  GETTERS 	  							     	    	     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Return the platform that the player is on
	/// </summary>
	/// <returns>The current platform.</returns>
	public BoxCollider2D getCurrentPlatform() {
		return getController().getCurrentPlatform().GetComponent<BoxCollider2D>();
	}

	/// <summary>
	/// Return the players location
	/// </summary>
	/// <returns>The location.</returns>
	public Vector3 getLocation() {
		return _player.transform.position;
	}

	/// <summary>
	/// Return the players transform
	/// </summary>
	/// <returns>The transform.</returns>
	public Transform getTransform() {
		return _player.transform;
	}

	/// <summary>
	/// Return the players controller
	/// </summary>
	/// <returns>The controller.</returns>
	public PlayerController getController() {
		return _player.GetComponent<PlayerController>();
	}

	/// <summary>
	/// Return the players health controller
	/// </summary>
	/// <returns>The health controller.</returns>
	public PlayerHealth getHealthController() {
		return _health;
	}

	/// <summary>
	/// Return the players weapon
	/// </summary>
	/// <returns>The weapon.</returns>
	public PlayerWeapon getWeapon() {
		return _player.GetComponentInChildren<PlayerWeapon>();
	}

	/// <summary>
	/// Return the players jetpack
	/// </summary>
	/// <returns>The jet pack.</returns>
	public Jetpack getJetpack() {
		return _player.GetComponentInChildren<Jetpack>();
	}

	/// <summary>
	/// Return the players shield
	/// </summary>
	/// <returns>The shield.</returns>
	public Shield getShield() {
		return _player.GetComponentInChildren<Shield>();
	}
}
