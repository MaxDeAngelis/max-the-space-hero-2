using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerWeapon))]
public class IPlayerWeapon : ILaser {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	CONSTANTS						     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;
		PlayerWeapon _weapon = target as PlayerWeapon;

		base.OnInspectorGUI();

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Secondary Weapon Stats", EditorStyles.boldLabel);

		_weapon.secondaryDamage = EditorGUILayout.FloatField("Damage", _weapon.secondaryDamage);
		_weapon.secondaryChargeTime = EditorGUILayout.FloatField("Charge Time", _weapon.secondaryChargeTime);
		_weapon.secondaryEnergyCost = EditorGUILayout.FloatField("Energy Cost", _weapon.secondaryEnergyCost);
		_weapon.secondaryChargedSoundEffect = (AudioClip)EditorGUILayout.ObjectField("Charged Sound Effect", _weapon.secondaryChargedSoundEffect, typeof(AudioClip), true);
		_weapon.secondarySoundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", _weapon.secondarySoundEffect, typeof(AudioClip), true);
		_weapon.secondaryProjectile = (GameObject)EditorGUILayout.ObjectField("Projectile", _weapon.secondaryProjectile, typeof(GameObject), true);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_weapon);
		}
	}
}
