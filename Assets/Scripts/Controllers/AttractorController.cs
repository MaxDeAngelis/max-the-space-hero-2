﻿using UnityEngine;
using System.Collections;

public class AttractorController : MonoBehaviour {

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private ParticleSystem _particleSystem;
	private ParticleSystem.Particle[] _emittedParticles;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Start () {
		_particleSystem = GetComponent<ParticleSystem>();
		_emittedParticles = new ParticleSystem.Particle[_particleSystem.maxParticles];
	}

	/**
	 * @private Called once per frame handles calculations used durring fixed update
	 **/
	void Update () {
		int particleCount = _particleSystem.GetParticles(_emittedParticles);

		for (int i=0; i < particleCount; i++) {
			_emittedParticles[i].velocity = Vector3.Lerp(_emittedParticles[i].velocity, (transform.position - _emittedParticles[i].position).normalized, 0.1f);
		}
		_particleSystem.SetParticles(_emittedParticles, _emittedParticles.Length);
	}
}
