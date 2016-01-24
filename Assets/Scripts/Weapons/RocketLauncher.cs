using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {
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
		projectile.transform.LookAt(new Vector3(target.x, projectile.transform.position.y, target.z));
		projectile.transform.Rotate(new Vector3(0,-90,0), Space.Self);
		velocity.y = 0f;

		return velocity;
	}
}