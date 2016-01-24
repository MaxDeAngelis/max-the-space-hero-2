using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Bomber))]
public class IBomber : IWeapon {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**
	 * @public Call on GUI to update the custom inspector. Also, calls parent version for common functionality
	**/
	public override void OnInspectorGUI() {
		GUI.changed = false;
		Bomber _weapon = target as Bomber;

		base.OnInspectorGUI();

		_weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", _weapon.projectile, typeof(GameObject), true);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_weapon);
		}
	}
}
