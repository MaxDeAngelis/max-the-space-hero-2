using UnityEngine;
using System.Collections;

public class MapMarker : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private SpriteRenderer _renderer;
	private Color _color;					// Color to use for the text
	private Material _material;				// The material used for the color
	private float _alpha = 1f;				// The opacity of the text object
	private float _duration = 1.5f;			// The duration of the text

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @private Called on start of the game object to init variables
	**/
	void Start() {
		/* INIT COMPONENTS */
		_renderer = GetComponent<SpriteRenderer>();
		_color = _renderer.color;

		// To ensure uniqueness instantiate a new material and set it to be used by the text
		_material = Instantiate(_renderer.material);
		_material.color = _color;  
		_renderer.material = _material;   
	}

	/**
	 * @private Called 60times per second fixed, handles all processing
	 **/
	void FixedUpdate () {
		if (_renderer.enabled) {
			if (_alpha > 0){
				// Calculate what the new alpha will be
				_alpha -= Time.deltaTime / _duration; 

				// Update the alpha of the color then set the text
				_color.a = _alpha;
				_material.color = _color;    
			} else {
				_renderer.enabled = false;

				// Update the alpha of the color then set the text
				_alpha = 1f;
				_color.a = _alpha;
				_material.color = _color;    
			}
		}
	}

	/**
	 * @private Handles checking if the marker is being hit by the radar
	 **/
	void OnTriggerEnter2D(Collider2D otherCollider) {
		// Store off a reference to the current platform you are one
		if (otherCollider.tag == "Radar") {
			_renderer.enabled = true;
		}
	}
}
