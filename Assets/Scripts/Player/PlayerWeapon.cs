using UnityEngine;
using System.Collections;

public class PlayerWeapon : Laser {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* SECONDARY FIRE VARIABLES */
	public float secondaryDamage = 5f;
	public float secondaryChargeTime;
	public float secondaryEnergyCost;
	public GameObject secondaryProjectile;
	public AudioClip secondarySoundEffect;
	public AudioClip secondaryChargedSoundEffect;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @public Called on start of game object
	**/
	public override void Start() {
		isPlayer = true;
		base.Start();
	}

	/**
	* @public This function is called from the enemy controller to fire a ranged weapon
	* 
	* @param $Vector3$ origin - The position of the origin of the gun
	* @param $Vector3$ target - The position of the target to fire at
	* @param $Boolean$ firedFromGround - Is the weapon fired from the ground
	**/
	public override void fire(Vector3 origin, Vector3 target, bool firedFromGround) {
		GameManager.Instance.processShot();

		base.fire(origin, target, firedFromGround);
	}

	/**
	 * @public Called from player controller to fire a secondary shot
	 *
	 * @param $Vector3$ origin - The position of the origin of the gun
	 * @param $Vector3$ target - The position of the target to fire at
	 * @param $Boolean$ firedFromGround - Is the weapon fired from the ground
	 **/
	public void fireSecondary(Vector3 origin, Vector3 target, bool firedFromGround) {
		EnergyManager.Instance.useEnergy(secondaryEnergyCost);

		attack(origin, target, secondaryDamage, secondaryProjectile, secondarySoundEffect, firedFromGround);
	}
}