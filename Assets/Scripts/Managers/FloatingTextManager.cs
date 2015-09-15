using UnityEngine;
using System.Collections;

public class FloatingTextManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static FloatingTextManager Instance;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of FloatingTextManager!");
		}
		Instance = this;

	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	 * @public called to display floating text that auto scrolls up then vanishes
	 **/
	public void show(Transform target, string text, Color textColor) {
		// Get a handle on the target renderer to beter adjust the text location
		Renderer targetRenderer = target.GetComponent<SpriteRenderer>();

		// Instantiate the text and get a handle on the controller
		GameObject floatingTextObject = Instantiate(Resources.Load("FloatingText") as GameObject);
		FloatingTextController floatingTextController = floatingTextObject.GetComponent<FloatingTextController>();

		// If there is a renderer then try and better position the text towards the top middle
		if (targetRenderer != null) {
			//Recalculate only the Y position
			float newVerticalPosition = targetRenderer.bounds.center.y + targetRenderer.bounds.extents.y;

			// Construct a new vector with the new vertical position
			floatingTextObject.transform.position = new Vector3(target.position.x, newVerticalPosition, target.position.z);
		} else {
			floatingTextObject.transform.position = target.position;
		}

		// Set the color and text string
		floatingTextController.setColor(textColor);
		floatingTextController.setText(text);
	}
}
