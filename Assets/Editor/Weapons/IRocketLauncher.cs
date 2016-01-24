using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RocketLauncher))]
public class IRocketLauncher : IWeapon {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Call on GUI to update the custom inspector. Also, calls parent version for common functionality
	 **/
	public override void OnInspectorGUI() {
		GUI.changed = false;
		RocketLauncher _weapon = target as RocketLauncher;

		base.OnInspectorGUI();

		_weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", _weapon.projectile, typeof(GameObject), true);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_weapon);
		}
	}
}
