using UnityEngine;
using System.Collections;
using UnityEditor;

public class IWeapon : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		// Get a reference to the extended class
		Weapon _weapon = target as Weapon;

		// Display expandable layout for weapon stats
		EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);

		// Always need damage
		_weapon.damage = EditorGUILayout.FloatField("Damage", _weapon.damage);

		// If a sucide bomb then no speed needed
		_weapon.attackSpeed = EditorGUILayout.FloatField("Attack Speed", _weapon.attackSpeed);

		// If this is a ranged weapon then prompt for range and projectile game object
		_weapon.range = EditorGUILayout.FloatField("Range", _weapon.range);

		_weapon.attackSoundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", _weapon.attackSoundEffect, typeof(AudioClip), true);
	}
}
