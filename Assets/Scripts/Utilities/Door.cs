using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* ---- OBJECTS/CONTROLLERS ---- */
	private Animator _animator;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on awake of the game object to init variables
	/// </summary>
	void Awake() {
		/* INIT COMPONENTS */
		_animator = GetComponent<Animator>();
	}

	/// <summary>
	/// Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	/// </summary>
	/// <param name="otherCollider">The collider hitting this collider</param>
	void OnTriggerEnter2D(Collider2D otherCollider) {
		if (otherCollider.tag == "Player") {
			SpecialEffectsManager.Instance.playDoorOpen();
			_animator.SetTrigger("open");
		}
	}

	/// <summary>
	/// Handles checking if the player is no longer over a climbable object. Sets gravity back
	/// </summary>
	/// <param name="otherCollider">The collider hitting this collider</param>
	void OnTriggerExit2D(Collider2D otherCollider) {
		if (otherCollider.tag == "Player") {
			SpecialEffectsManager.Instance.playDoorClose();
			_animator.SetTrigger("close");
		}
	}
}
