using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : Health {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool isAlienAbleToEject = false;	// Flag for if the alien can eject

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/* ---- OBJECTS/CONTROLLERS ---- */
	private EnemyController _enemy;
	private List<float[]> _damageList = new List<float[]>();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	  PROTECTED FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @protected Called on death
	**/
	protected override void die() {
		GameManager.Instance.processKill(_damageList, _enemy);

		// If the enemy can eject then eject
		if (isAlienAbleToEject) {
			SpecialEffectsManager.Instance.playAlienEject(gameObject.transform.position, null);
		}

		base.die();
	}

	/**
	 * @protected Called to add to the damage list that is sent to game manager on death for score keeping
	 * 
	 * @param $Float$ modifier - The modifier that is calculated by the hitbox
	 * @param $Float$ damage - The amount of damage that was taken
	 **/
	protected override void damaged(float modifier, float damage) {
		float[] damageItem = new float[]{ modifier, damage };
		_damageList.Add(damageItem);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Called on start of game object
	 **/
	public override void Start() {
		isPlayer = false;

		_enemy = GetComponent<EnemyController>();

		base.Start();
	}
}
