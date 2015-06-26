using UnityEngine;
using System.Collections;

public class LandingController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		HIDDEN VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[HideInInspector] public bool isAbleToLand = true;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Handles checking if the player is over a climbable object. Changes the gravity to allow climbing
	 **/
	void OnTriggerStay2D(Collider2D otherCollider) {
		if (otherCollider.gameObject.tag == "Ground") {
			// Get a reference to the current collider for the Landing check object
			Collider2D currentCollider = gameObject.GetComponent<BoxCollider2D>();

			// Figure out the position of top of the ground you are over
			float topOfGround = otherCollider.bounds.center.y + otherCollider.bounds.extents.y;

			// Find the top border of the landing check collider
			float topOfLandingCheck = currentCollider.bounds.center.y + currentCollider.bounds.extents.y;

			// Find the bottom of the landing check collider
			float bottomOfLandingCheck = currentCollider.bounds.center.y - currentCollider.bounds.extents.y;

			// As long as the top of the landing check is over the top of the ground and the bottom is below
			// then Max can land
			if(topOfLandingCheck >= topOfGround && bottomOfLandingCheck <= topOfGround) {
				isAbleToLand = true;
			} else {
				isAbleToLand = false;
			}
		}
	}
}
