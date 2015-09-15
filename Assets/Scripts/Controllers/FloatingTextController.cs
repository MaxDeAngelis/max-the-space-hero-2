using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingTextController : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Text displayText;				// Actual text object

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private float _alpha = 1;				// The opacity of the text object
	private float _scrollRate = 1f;			// The rate at which the text scrolls up
	private float _duration = 1.5f;			// The duration of the text
	private Color _color;					// The color of the text being displayed
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate () {
		if (_alpha > 0){
			// Calculate the new position and then move the text object
			float newHorizontalPosition = transform.position.y + (_scrollRate * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, newHorizontalPosition, transform.position.z);

			// Calculate what the new alpha will be
			_alpha -= Time.deltaTime / _duration; 

			// Retrieve an editable color variable and reset its alpha before setting it back
			_color = displayText.material.color;
			_color.a = _alpha;
			displayText.material.color = _color;        
		} else {
			Destroy(gameObject); // text vanished - destroy itself
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public called to set the internal color of the floating text
	 **/
	public void setColor(Color textColor) {
		displayText.material.color = textColor;
	}
	/**
	 * @public called to set the text of the floating text being displayed
	 **/
	public void setText(string text) {
		displayText.text = text;
	}
}
