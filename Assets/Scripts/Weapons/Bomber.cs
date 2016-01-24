using UnityEngine;
using System.Collections;

public class Bomber : Weapon {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PROTECTED FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	* @protected Called from attack to adjust the projectiles rotation. Overriden in different weapon classes
	*
	* @param $Vector3$ origin - The position of the origin of the gun
	* @param $Vector3$ target - The position of the target to fire at
	* @param $GameObject$ projectile - The projectile game object to adjust its location
	* @param $Vector2$ velocity - The projectile velocity
	**/
	protected override Vector2 adjustProjectile(Vector3 origin, Vector3 target, GameObject projectile, Vector2 velocity) {
		velocity = new Vector2(0f, -1f);

		return velocity;
	}
}