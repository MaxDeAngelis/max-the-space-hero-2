using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static Utilities Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	* @private Called on start of the game object to init variables
	**/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of Utilities!");
		}
		Instance = this;

	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to show text that fades away
	/// </summary>
	/// <param name="target">The target transform</param>
	/// <param name="text">Text to display</param>
	/// <param name="textColor">Text color</param>
	public void showFadeAwayText(Transform target, string text, Color textColor) {
		// Get a handle on the target renderer to beter adjust the text location
		Renderer targetRenderer = target.GetComponent<SpriteRenderer>();

		// Instantiate the text and get a handle on the controller
		GameObject fadingTextObject = Instantiate(Resources.Load("FadeAwayText") as GameObject);
		FadeAwayText fadeAwayText = fadingTextObject.GetComponent<FadeAwayText>();

		// If there is a renderer then try and better position the text towards the top middle
		if (targetRenderer != null) {
			//Recalculate only the Y position
			float newVerticalPosition = targetRenderer.bounds.center.y + targetRenderer.bounds.extents.y;

			// Construct a new vector with the new vertical position
			fadingTextObject.transform.position = new Vector3(target.position.x, newVerticalPosition, target.position.z);
		} else {
			fadingTextObject.transform.position = target.position;
		}

		// Set the color and text string
		fadeAwayText.setColor(textColor);
		fadeAwayText.setText(text);
	}
}
