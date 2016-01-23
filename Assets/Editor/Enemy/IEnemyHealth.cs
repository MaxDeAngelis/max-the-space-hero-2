using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(EnemyHealth))]
public class IEnemyHealth : IHealth {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS										     	 ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void OnInspectorGUI() {
		GUI.changed = false;
		EnemyHealth _health = target as EnemyHealth;

		_health.health = EditorGUILayout.FloatField("Health", _health.health);
		_health.isAlienAbleToEject = EditorGUILayout.Toggle("Is Alien Able To Eject", _health.isAlienAbleToEject);
		base.OnInspectorGUI();

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_health);
		}
	}
}
