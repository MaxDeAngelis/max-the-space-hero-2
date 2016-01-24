using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Laser))]
public class ILaser : IWeapon {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Call on GUI to update the custom inspector. Also, calls parent version for common functionality
	 **/
	public override void OnInspectorGUI() {
		GUI.changed = false;
		Laser _weapon = target as Laser;

		base.OnInspectorGUI();

		_weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", _weapon.projectile, typeof(GameObject), true);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_weapon);
		}
	}
}
