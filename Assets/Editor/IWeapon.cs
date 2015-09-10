using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WeaponController))]
public class IWeapon : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	CONSTANTS						     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private enum TYPES {Ranged, Melee, Suicide};			// Enum for options in the type drop down

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isWeaponStatsVisible = true;					// Flag for when to show weapon stats
	private bool _indestructible = true;					// Flag for when to show durability options
	private TYPES _type = TYPES.Ranged;						// Type of weapon defaults to ranged

	public override void OnInspectorGUI() {
		// Get a reference to the extended class
		WeaponController _weapon = target as WeaponController;

		// Display default settings
		_type = (TYPES)EditorGUILayout.EnumPopup("Type", _type);
		_weapon.isPlayer = EditorGUILayout.Toggle("Is Player", _weapon.isPlayer);
		_weapon.attackSoundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", _weapon.attackSoundEffect, typeof(AudioClip), true);

		// Set ranged flag if type is ranged
		if (_type == TYPES.Ranged) {
			_weapon.isRanged = true;
		} else {
			_weapon.isRanged = false;
		}

		// Display expandable layout for weapon stats
		_isWeaponStatsVisible = EditorGUILayout.Foldout(_isWeaponStatsVisible, "Weapon Stats");
		if (_isWeaponStatsVisible) {
			// Indent the content of the layout
			EditorGUI.indentLevel++;

			// Always need damage
			_weapon.damage = EditorGUILayout.FloatField("Damage", _weapon.damage);

			// If a sucide bomb then no speed needed
			if (_type != TYPES.Suicide) {
				_weapon.attackSpeed = EditorGUILayout.FloatField("Attack Speed", _weapon.attackSpeed);
			}

			// If this is a ranged weapon then prompt for range and projectile game object
			if (_type == TYPES.Ranged) {
				_weapon.range = EditorGUILayout.FloatField("Range", _weapon.range);
				_weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", _weapon.projectile, typeof(GameObject), true);
			} else {
				_weapon.range = 0.1f;
			}

			// If not sucide then check if indestructable. If sucide it will destroy automatically
			if (_type != TYPES.Suicide) {
				_indestructible = EditorGUILayout.Toggle("Indestructible", _indestructible);

				// If its not indestructable then prompt for durability and durability lose 
				if (!_indestructible) {
					_weapon.durability = EditorGUILayout.FloatField("Durability", _weapon.durability);
					_weapon.durabilityLossPerAttack = EditorGUILayout.FloatField("Durability lose per attack", _weapon.durabilityLossPerAttack);
				} else {
					_weapon.durability = 1000f;
					_weapon.durabilityLossPerAttack = 0f;
				}
			} else {
				_weapon.durability = 1f;
				_weapon.durabilityLossPerAttack = 1f;
			}


		}
	}
}
