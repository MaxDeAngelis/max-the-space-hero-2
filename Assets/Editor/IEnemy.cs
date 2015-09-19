using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class IEnemy : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	CONSTANTS						     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isMovementOptionsVisible = true;
	private bool _isSupportingObjectsVisible = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		// Get a reference to the extended class
		EnemyController _enemy = target as EnemyController;
		
		// Display default settings
		_enemy.type = (ENEMY_TYPE)EditorGUILayout.EnumPopup("Type", _enemy.type);

		_isMovementOptionsVisible = EditorGUILayout.Foldout(_isMovementOptionsVisible, "Movement Options");
		if (_isMovementOptionsVisible) {
			// Always needed options
			_enemy.maxSpeed = EditorGUILayout.FloatField("Speed", _enemy.maxSpeed);
			_enemy.sightRange = EditorGUILayout.FloatField("Sight", _enemy.sightRange);
			_enemy.patrolDistance = EditorGUILayout.FloatField("Patrol Distance", _enemy.patrolDistance);

			// Specific movement options based on type
			if (_enemy.type == ENEMY_TYPE.Flying) {
				_enemy.hoverDistance = EditorGUILayout.FloatField("Hover Distance", _enemy.hoverDistance);
			}
		}

		_isSupportingObjectsVisible = EditorGUILayout.Foldout(_isSupportingObjectsVisible, "Supporting Objects");
		if (_isSupportingObjectsVisible) {
			_enemy.gunArm = (Transform)EditorGUILayout.ObjectField("Gun Arm", _enemy.gunArm, typeof(Transform), true);
			_enemy.groundCheck = (Transform)EditorGUILayout.ObjectField("Ground Check", _enemy.groundCheck, typeof(Transform), true);
		}
	}
}
