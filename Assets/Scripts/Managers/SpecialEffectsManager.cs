using UnityEngine;
using System.Collections;

public class SpecialEffectsManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static SpecialEffectsManager Instance;

	public GameObject explosion;			// Explosion reference game object
	public GameObject explosionSmall;		// Small explosion reference game objec
	public GameObject weaponCharging;
	public GameObject weaponCharged;
	public GameObject weaponFired;
	public GameObject alienEject;			// Alien eject reference

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private GameObject _currentChargingAnimation;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of SoundEffectsManager!");
		}
		Instance = this;
	}

	/**
	 * @private Called to instantiate a new game object and then destroy it
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 * @param GameObject objectToCreate - the object to instantiate
	 * @param float duration - the amount of time to keep the object alive
	 **/
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
	/**
	 * @public Function responsable for making the sound effect given
	 * 
	 * @param $AudioClip$ originalClip - The sound effect to make or null
	 **/
	public void playSound(AudioClip originalClip) {
		// As it is not 3D audio clip, position doesn't matter.
		if (originalClip != null) {
			AudioSource.PlayClipAtPoint(originalClip, transform.position);
		}
	}

	/**
	 * @public Called to play a normal sized explosion
	 * 
	 * @param Vector3 location - the location to create the new object
	 * @param AudioClip sound - the sound effect to play
	 **/
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

	public void playWeaponCharged(Vector3 location, AudioClip sound) {
		stopWeaponCharging();
		GameObject charged = _instantiate(location, sound, weaponCharged, weaponCharged.GetComponent<ParticleSystem>().duration);
		charged.transform.parent = PlayerManager.Instance.getWeapon().transform;
	}

	public void playWeaponCharging(Vector3 location, AudioClip sound) {
		_currentChargingAnimation = _instantiate(location, sound, weaponCharging, 0f);
		_currentChargingAnimation.transform.parent = PlayerManager.Instance.getWeapon().transform;
	}

	public void stopWeaponCharging() {
		Destroy(_currentChargingAnimation);
	}
}
