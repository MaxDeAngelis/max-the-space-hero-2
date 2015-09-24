using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProjectileController))]
public class IProjectile : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		// Get a reference to the extended class
		ProjectileController _projectile = target as ProjectileController;


		// Display default settings
		_projectile.type = (PROJECTILE_TYPE)EditorGUILayout.EnumPopup("Type", _projectile.type);

		// Always needed options
		_projectile.speed = EditorGUILayout.FloatField("Speed", _projectile.speed);

	}
}
