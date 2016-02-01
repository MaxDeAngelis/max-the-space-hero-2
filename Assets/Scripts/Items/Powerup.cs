using UnityEngine;
using System.Collections;

public enum POWERUP_TYPE { Health, Energy, Shield, Speed };

public class Powerup : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float bonus;				// Bonus to be applied
	public float duration;			// Duration of the powerup
	public Color colorEffect;				// The color of the particle effect
	public POWERUP_TYPE type;		// Type of powerup
	public AudioClip pickup;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTION											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on awake of the game object to init variables
	/// </summary>
	void Awake () {

	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTION												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to use the powerup
	/// </summary>
	public void use() {
		// Destroy the powerup
		Destroy(gameObject);

		// Start particle effect for use
		PlayerManager.Instance.playParticleEffect(2f, colorEffect);

		SpecialEffectsManager.Instance.playSound(pickup);
	}
}
