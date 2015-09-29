using UnityEngine;
using System.Collections;

public class SpecialEffectsManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static SpecialEffectsManager Instance;

	public GameObject explosion;

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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Function responsable for making the sound effect given
	 * 
	 * @param $AudioClip$ originalClip - The sound effect to make or null
	 **/
	public void makeSound(AudioClip originalClip) {
		// As it is not 3D audio clip, position doesn't matter.
		if (originalClip != null) {
			AudioSource.PlayClipAtPoint(originalClip, transform.position);
		}
	}

	public void makeExplosion(Vector3 location, AudioClip sound) {
		makeSound(sound);

		GameObject newExplosion = Instantiate(explosion, location, Quaternion.identity) as GameObject;

		Destroy(newExplosion, 1f);
	}
}
