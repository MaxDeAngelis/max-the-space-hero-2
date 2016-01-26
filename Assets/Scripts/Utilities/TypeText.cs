﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypeText : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private Text _text;				// Display text object itself
	private string _textString;
	private int _charCount = 0;
	private int _frameDelay = 3;
	private int _frameCount = 3;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	void Start() {
		// Get a handle on the text object for setting the value
		_text = GetComponentInChildren<Text>();
		_textString = _text.text;
		_text.text = "";

		//StartCoroutine(AnimateText());
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void Update() {
		// If there is still text to append then do it
		if (_textString.Length > 0) {
			if (_frameCount >= _frameDelay ) {
				// Strip off the first charecter from the string
				string _singleChar = _textString.Substring(0, 1);
				_textString = _textString.Substring(1);

				// Append the single character to the text object
				_text.text += _singleChar;
				_frameCount = 0;
			} else {
				_frameCount++; 
			}
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
