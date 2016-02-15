using UnityEngine;
using System.Collections;

public class SpecialEffectsManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static SpecialEffectsManager Instance;

	[Header("Particle Effects")]
	public GameObject explosion;			// Explosion reference game object
	public GameObject explosionSmall;		// Small explosion reference game objec
	public GameObject weaponCharging;
	public GameObject weaponCharged;
	public GameObject weaponFired;
	public GameObject alienEject;			// Alien eject reference

	[Header("Sounds")]
	public AudioClip buttonHoverSound;
	public AudioClip buttonClickSound;
	public AudioClip typingSound;
	public AudioClip doorOpen;
	public AudioClip doorClose;
	public AudioClip levelComplete;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private GameObject _currentChargingAnimation;
	private AudioSource _audioSource;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of SoundEffectsManager!");
		}
		Instance = this;

		OnLevelWasLoaded(0);
	}

	/// <summary>
	/// Called on level load to set the renderer camera of the HUD
	/// </summary>
	/// <param name="level">The level number</param>
	void OnLevelWasLoaded(int level) {
		_audioSource = Camera.main.GetComponent<AudioSource>();
	}

	/// <summary>
	/// Called to instantiate a new game object and then destroy it
	/// </summary>
	/// <param name="location">The location to create the new object</param>
	/// <param name="sound">The sound effect to play</param>
	/// <param name="objectToCreate">The object to instantiate</param>
	/// <param name="duration">The amount of time to keep the object alive</param>
	private GameObject _instantiate(Vector3 location, AudioClip sound, GameObject objectToCreate, float duration) {
		playSound(sound);
		
		GameObject newExplosion = Instantiate(objectToCreate, location, Quaternion.identity) as GameObject;

		if (duration > 0f) {
			Destroy(newExplosion, duration);
		}

		return newExplosion;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Plays sound for button hover
	/// </summary>
	public void playButtonHover() {
		playSound(buttonHoverSound);
	}

	/// <summary>
	/// Plays sound for button click
	/// </summary>
	public void playButtonClick() {
		playSound(buttonClickSound);
	}

	/// <summary>
	/// Plays a typing sound
	/// </summary>
	public void playTypingSound() {
		playSound(typingSound);
	}

	/// <summary>
	/// Plays the door open sound
	/// </summary>
	public void playDoorOpen() {
		playSound(doorOpen);
	}

	/// <summary>
	/// Plays the door close sound
	/// </summary>
	public void playDoorClose() {
		playSound(doorClose);
	}
		
	/// <summary>
	/// Plays the level complete sound
	/// </summary>
	public void playLevelComplete() {
		playSound(levelComplete);
	}

	/// <summary>
	/// Function responsable for making the sound effect given
	/// </summary>
	/// <param name="originalClip">The sound effect to make or null</param>
	public void playSound(AudioClip originalClip) {
		// As it is not 3D audio clip, position doesn't matter.
		if (originalClip != null) {
			_audioSource.PlayOneShot(originalClip);
		}
	}
		
	/// <summary>
	/// Called to play a normal sized explosion
	/// </summary>
	/// <param name="location">The location to create the new object</param>
	/// <param name="sound">The sound effect to play</param>
	public void playExplosion(Vector3 location, AudioClip sound) {
		_instantiate(location, sound, explosion, 1f);
	}

	/**
	 * @public Called to play a small sized explosion
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 **/
	public void playSmallExplosion(Vector3 location, AudioClip sound) {
		_instantiate(location, sound, explosionSmall, 1f);
	}

	/**
	 * @public Called to play an alien eject
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 **/
	public void playAlienEject(Vector3 location, AudioClip sound) {
		_instantiate(location, sound, alienEject, 1f);
	}

	/**
	 * @public Called to play weapon charged effect
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 **/
	public void playWeaponCharged(Vector3 location, AudioClip sound) {
		stopWeaponCharging();
		GameObject charged = _instantiate(location, sound, weaponCharged, 1f);
		charged.transform.parent = PlayerManager.Instance.getWeapon().transform;
	}

	/**
	 * @public Called to play weapon charging effect
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 **/
	public void playWeaponCharging(Vector3 location, AudioClip sound) {
		_currentChargingAnimation = _instantiate(location, sound, weaponCharging, 0f);
		_currentChargingAnimation.GetComponent<FollowGameObject>().gameObjectToFollow = PlayerManager.Instance.getWeapon().gameObject;
	}

	/**
	 * @public Called to stop the weapon charging
	 **/
	public void stopWeaponCharging() {
		Destroy(_currentChargingAnimation);
	}
}
