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
	private bool _showWeaponStats = true;					// Flag for when to show weapon stats
	private bool _indestructible = true;					// Flag for when to show durability options
	private TYPES _type = TYPES.Ranged;						// Type of weapon defaults to ranged

	public override void OnInspectorGUI() {
		// Get a reference to the extended class
		WeaponController myWeapon = target as WeaponController;

		// Display default settings
		_type = (TYPES)EditorGUILayout.EnumPopup("Type", _type);
		myWeapon.isPlayer = EditorGUILayout.Toggle("Is Player?", myWeapon.isPlayer);
		myWeapon.attackSoundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound effect", myWeapon.attackSoundEffect, typeof(AudioClip), true);

		// Set ranged flag if type is ranged
		if (_type == TYPES.Ranged) {
			myWeapon.isRanged = true;
		} else {
			myWeapon.isRanged = false;
		}

		// Display expandable layout for weapon stats
		_showWeaponStats = EditorGUILayout.Foldout(_showWeaponStats, "Weapon Stats");
		if (_showWeaponStats) {
			// Indent the content of the layout
			EditorGUI.indentLevel++;

			// Always need damage
			myWeapon.damage = EditorGUILayout.FloatField("Damage", myWeapon.damage);

			// If a sucide bomb then no speed needed
			if (_type != TYPES.Suicide) {
				myWeapon.attackSpeed = EditorGUILayout.FloatField("Attack Speed", myWeapon.attackSpeed);
			}

			// If this is a ranged weapon then prompt for range and projectile game object
			if (_type == TYPES.Ranged) {
				myWeapon.range = EditorGUILayout.FloatField("Range", myWeapon.range);
				myWeapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", myWeapon.projectile, typeof(GameObject), true);
			} else {
				myWeapon.range = 0.1f;
			}

			// If not sucide then check if indestructable. If sucide it will destroy automatically
			if (_type != TYPES.Suicide) {
				_indestructible = EditorGUILayout.Toggle("Indestructible", _indestructible);

				// If its not indestructable then prompt for durability and durability lose 
				if (!_indestructible) {
					myWeapon.durability = EditorGUILayout.FloatField("Durability", myWeapon.durability);
					myWeapon.durabilityLossPerAttack = EditorGUILayout.FloatField("Durability lose per attack", myWeapon.durabilityLossPerAttack);
				} else {
					myWeapon.durability = 1000f;
					myWeapon.durabilityLossPerAttack = 0f;
				}
			} else {
				myWeapon.durability = 1f;
				myWeapon.durabilityLossPerAttack = 1f;
			}


		}
	}
}
