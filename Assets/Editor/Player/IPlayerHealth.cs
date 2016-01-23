using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(PlayerHealth))]
public class IPlayerHealth : IHealth {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS										     	 ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void OnInspectorGUI() {
		GUI.changed = false;
		PlayerHealth _health = target as PlayerHealth;

		// If this is a player prompt for health bar information
		_health.healthDisplay = (Text)EditorGUILayout.ObjectField("Health Text", _health.healthDisplay, typeof(Text), true);
		_health.healthBar = (Slider)EditorGUILayout.ObjectField("Health Bar", _health.healthBar, typeof(Slider), true);
		base.OnInspectorGUI();

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_health);
		}
	}
}
