using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Projectile))]
public class IProjectile : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		Projectile _projectile = target as Projectile;


		// Display default settings
		_projectile.type = (PROJECTILE_TYPE)EditorGUILayout.EnumPopup("Type", _projectile.type);

		// Always needed options
		_projectile.speed = EditorGUILayout.FloatField("Speed", _projectile.speed);

		if (_projectile.type == PROJECTILE_TYPE.Bomb) {
			_projectile.explosion =(AudioClip)EditorGUILayout.ObjectField("Explosion Sound", _projectile.explosion, typeof(AudioClip), true);
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_projectile);
		}
	}
}
