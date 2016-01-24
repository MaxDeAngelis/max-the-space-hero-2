using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class IHealth : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public override void OnInspectorGUI() {
		Health _health = target as Health;

		// Get the basic information needed for health
		_health.damageSoundEffect =(AudioClip)EditorGUILayout.ObjectField("Damage Sound", _health.damageSoundEffect, typeof(AudioClip), true);
		_health.deathSoundEffect =(AudioClip)EditorGUILayout.ObjectField("Death Sound", _health.deathSoundEffect, typeof(AudioClip), true);
	}
}
