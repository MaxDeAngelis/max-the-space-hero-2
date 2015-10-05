using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class IEnemy : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	PRIVATE VARIABLES   			     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isMovementOptionsVisible = true;
	private bool _isSupportingObjectsVisible = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		EnemyController _enemy = target as EnemyController;
		
		// Display default settings
		_enemy.type = (ENEMY_TYPE)EditorGUILayout.EnumPopup("Type", _enemy.type);
		_enemy.rank = (ENEMY_RANK)EditorGUILayout.EnumPopup("Rank", _enemy.rank);

		_isMovementOptionsVisible = EditorGUILayout.Foldout(_isMovementOptionsVisible, "Movement Options");
		if (_isMovementOptionsVisible) {
			// Always needed options
			_enemy.maxSpeed = EditorGUILayout.FloatField("Speed", _enemy.maxSpeed);
			_enemy.sightRange = EditorGUILayout.FloatField("Sight", _enemy.sightRange);
			_enemy.patrolDistance = EditorGUILayout.FloatField("Patrol Distance", _enemy.patrolDistance);

			// Specific movement options based on type
			if (_enemy.type == ENEMY_TYPE.Flying) {
				_enemy.patrolDirection = (PATROL)EditorGUILayout.EnumPopup("Patrol Direction", _enemy.patrolDirection);
			}

			// Specify the hover distance to target above the player
			if (_enemy.type == ENEMY_TYPE.Bomber) {
				_enemy.hoverHeight = EditorGUILayout.FloatField("Hover Height", _enemy.hoverHeight);
			} else {
				_enemy.hoverHeight = 0f;
			}
		}

		_isSupportingObjectsVisible = EditorGUILayout.Foldout(_isSupportingObjectsVisible, "Supporting Objects");
		if (_isSupportingObjectsVisible) {
			_enemy.gunArm = (Transform)EditorGUILayout.ObjectField("Gun Arm", _enemy.gunArm, typeof(Transform), true);
			_enemy.groundCheck = (Transform)EditorGUILayout.ObjectField("Ground Check", _enemy.groundCheck, typeof(Transform), true);
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_enemy);
		}
	}
}
