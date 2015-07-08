using UnityEngine;
using System.Collections;

public class GunArmController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Transform head;
	public Transform bicep;
	public Transform forearm;
	public Transform hand;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private WeaponController _weapon;
	private PlayerController _player;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start() {
		_weapon = GetComponentInChildren<WeaponController>();
		_player = GetComponent<PlayerController>();
	}
	
	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate() {
		/* ---- AIM THE ARM TO FIRE ----*/		
		// Get mouse position and arm position
		Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		Vector3 armPos = Camera.main.WorldToViewportPoint (hand.transform.position);
		
		// Get arm and mouse position relative to the game object
		Vector2 relativeArmPos = new Vector2(armPos.x - 0.5f, armPos.y - 0.5f);
		Vector2 relativeMousePos = new Vector2 (mousePos.x - 0.5f, mousePos.y - 0.5f) - relativeArmPos;
		float angle = Vector2.Angle (Vector2.down, relativeMousePos);

		// Flip the player if aiming in the opposite direction
		if ((relativeMousePos.x < 0 && _player.facingRight) || (relativeMousePos.x > 0 && !_player.facingRight)) {
			_player.flipPlayer();
		}

		// Calculate the Quaternion and rotate the arm
		Quaternion quat = Quaternion.identity;
		quat.eulerAngles = new Vector3(0, 0, angle);

		// Rotate the arm pieces
		bicep.transform.rotation = quat;
		forearm.transform.rotation = quat;
		hand.transform.rotation = quat;


		/* ---- AIM THE HEAD TO FOLLOW GUN ----*/	
		// Get the head position
		Vector3 headPos = Camera.main.WorldToViewportPoint (head.transform.position);

		// Get head and mouse position relative to the game object
		Vector2 relativeHeadPos = new Vector2(headPos.x - 0.5f, headPos.y - 0.5f);
		relativeMousePos = new Vector2 (mousePos.x - 0.5f, mousePos.y - 0.5f) + relativeHeadPos;
		angle = Vector2.Angle(Vector2.down, relativeMousePos) - 85f;

		// Manually clamp the head rotation to make it more natural
		if (relativeMousePos.y < 0 && angle < 340f) {
			angle = 340f;
		} else if (relativeMousePos.y > 0 && angle > 60f) {
			angle = 60f;
		}

		// Calculate the Quaternion and rotate the head
		quat = Quaternion.identity;
		quat.eulerAngles = new Vector3(0, 0, angle);

		// Actually rotate the head
		head.transform.rotation = quat;
	}
}
