using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(HealthController))]
public class IHealth : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	CONSTANTS						     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		HealthController _health = target as HealthController;

		// Get the basic information needed for health
		_health.health = EditorGUILayout.FloatField("Health", _health.health);
		_health.damageSoundEffect =(AudioClip)EditorGUILayout.ObjectField("Damage Sound", _health.damageSoundEffect, typeof(AudioClip), true);
		_health.deathSoundEffect =(AudioClip)EditorGUILayout.ObjectField("Death Sound", _health.deathSoundEffect, typeof(AudioClip), true);

		// Flag for if is the player. If so more information is needed
		_health.isPlayer = EditorGUILayout.Toggle("Is Player", _health.isPlayer);

		// If this is a player prompt for health bar information
		if (_health.isPlayer) {
			_health.healthDisplay = (Text)EditorGUILayout.ObjectField("Health Text", _health.healthDisplay, typeof(Text), true);
			_health.healthBar = (Slider)EditorGUILayout.ObjectField("Health Bar", _health.healthBar, typeof(Slider), true);
		} else {
			_health.isAlienAbleToEject = EditorGUILayout.Toggle("Is Alien Able To Eject", _health.isAlienAbleToEject);
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_health);
		}
	}
}
