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
	/// <summary>
	/// Called on death
	/// </summary>
	protected override void die() {
		GameManager.Instance.processKill(_damageList, _enemy);

		// If the enemy can eject then eject
		if (isAlienAbleToEject) {
			SpecialEffectsManager.Instance.playAlienEject(gameObject.transform.position, null);
		}

		base.die();
	}
		
	/// <summary>
	/// Called to add to the damage list that is sent to game manager on death for score keeping
	/// </summary>
	/// <param name="modifier">The modifier that is calculated by the hitbox</param>
	/// <param name="damage">The amount of damage that was taken</param>
	protected override void damaged(float modifier, float damage) {
		float[] damageItem = new float[]{ modifier, damage };
		_damageList.Add(damageItem);
	}

	/// <summary>
	/// Called on start of the game obect
	/// </summary>
	protected override void Start() {
		isPlayer = false;

		_enemy = GetComponent<EnemyController>();

		base.Start();
	}
}
