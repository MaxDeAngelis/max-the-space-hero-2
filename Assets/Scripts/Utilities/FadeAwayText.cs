using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeAwayText : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private Text _displayText;				// Display text object itself
	private Color _textColor;				// Color to use for the text
	private Material _material;				// The material used for the color
	private float _alpha = 1;				// The opacity of the text object
	private float _scrollRate = 1f;			// The rate at which the text scrolls up
	private float _duration = 1.5f;			// The duration of the text
	//private Color _color;					// The color of the text being displayed
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	void Awake() {
		// Get a handle on the text object for setting the value
		_displayText = GetComponentInChildren<Text>();

		// To ensure uniqueness instantiate a new material and set it to be used by the text
		_material = Instantiate(_displayText.material);
		_material.color = _textColor;  
		_displayText.material = _material;   
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void Update () {
		if (_alpha > 0){
			// Calculate the new position and then move the text object
			float newHorizontalPosition = transform.position.y + (_scrollRate * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, newHorizontalPosition, transform.position.z);

			// Calculate what the new alpha will be
			_alpha -= Time.unscaledDeltaTime / _duration; 

			// Update the alpha of the color then set the text
			_textColor.a = _alpha;
			_material.color = _textColor;    
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
		_textColor = textColor;
	}
	/**
	 * @public called to set the text of the floating text being displayed
	 **/
	public void setText(string text) {
		_displayText.text = text;
	}
}
