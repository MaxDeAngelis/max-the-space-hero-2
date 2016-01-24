﻿using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static PlayerManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private GameObject _player;
	private ParticleSystem _particle;
	private PlayerHealth _health;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/**
	 * @private Called on start of the game object to init variables
	 **/
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of PlayerManager!");
		}
		Instance = this;

		/* INIT VARIABLES */
		_player = GameObject.FindGameObjectWithTag("Player");
		_particle = _player.GetComponent<ParticleSystem>();
		_health = _player.GetComponent<PlayerHealth>();
	}

	/**
	 * @private called from use to trigger an effect to show the powerup was used
	 **/
	IEnumerator _playParticleEffect(float duration, Color color) {
		// Make sure the alpha is set to 1, for some reason it seems to be 0 alot of the time
		color.a = 1f;

		// Play the particle effect for use if there is one
		_particle.startColor = color;
		_particle.Play();

		// delay for a quarter second
		yield return new WaitForSeconds(duration);
		
		// Destroy after affect is done
		_particle.Stop();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public plays the particle system attached to the player for the given durration with the given color
	 * 
	 * @param float duration - the duration to play the effect for
	 * @param Color color - the color to set the effect to
	 **/
	public void playParticleEffect(float duration, Color color) {
		StartCoroutine(_playParticleEffect(duration, color));
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	    	  FLAGS 	  							     	    	     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public returns true if the player is anchored
	 **/
	public bool isAnchored() {
		return getJetPack().isAnchored();
	}

	/**
	 * @public returns true if the player is flying or not
	 **/
	public bool isFlying() {
		return getJetPack().isFlying();
	}

	/**
	 * @public returns true if the player is grounded
	 **/
	public bool isGrounded() {
		return getController().isGrounded();
	}

	/**
	 * @public returns true if the player is climbing something
	 **/
	public bool isClimbing() {
		return getController().isClimbing();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	    	  GETTERS 	  							     	    	     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public returns the platform that the player is standing on
	 **/
	public BoxCollider2D getCurrentPlatform() {
		return getController().getCurrentPlatform().GetComponent<BoxCollider2D>();
	}

	/**
	 * @public returns the location of the player
	 **/
	public Vector3 getLocation() {
		return _player.transform.position;
	}

	/**
	 * @public returns the players transform
	 **/
	public Transform getTransform() {
		return _player.transform;
	}

	/**
	 * @public returns the player controller of the player
	 **/
	public PlayerController getController() {
		return _player.GetComponent<PlayerController>();
	}

	/**
	 * @public returns the player health controller
	 **/
	public PlayerHealth getHealthController() {
		return _health;
	}

	/**
	 * @public return the players weapon controller
	 **/
	public PlayerWeapon getWeapon() {
		return _player.GetComponentInChildren<PlayerWeapon>();
	}

	/**
	 * @public return the players weapon controller
	 **/
	public JetPack getJetPack() {
		return _player.GetComponentInChildren<JetPack>();
	}
}
