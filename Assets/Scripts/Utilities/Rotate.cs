﻿using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public int speed;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private void Start() {
		//GetComponentInChildren<TrailRenderer>().sortingLayerName = "Map Markers";
	}

	void Update() {
		transform.Rotate(0, 0, Time.deltaTime * speed);
	}
}
