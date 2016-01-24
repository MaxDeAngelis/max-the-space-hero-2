using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class IPlayer : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isMovementOptionsVisible = true;
	private bool _isSupportTransformsVisible = false;
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		PlayerController _player = target as PlayerController;

		// Display expandable section for movement related variables
		_isMovementOptionsVisible = EditorGUILayout.Foldout(_isMovementOptionsVisible, "Movement Options");
		if (_isMovementOptionsVisible) {
			// Indent the content of the layout
			EditorGUI.indentLevel++;
			_player.movementSpeed = EditorGUILayout.FloatField("Movement Speed", _player.movementSpeed);
			EditorGUI.indentLevel--;
		}

		// Display expandable section for all supporting transform objects
		_isSupportTransformsVisible = EditorGUILayout.Foldout(_isSupportTransformsVisible, "Support Transforms");
		if (_isSupportTransformsVisible) {
			// Indent the content of the layout
			EditorGUI.indentLevel++;
			_player.gunArm = (Transform)EditorGUILayout.ObjectField("Gun Arm", _player.gunArm, typeof(Transform), true);
			_player.forwardGroundCheck = (Transform)EditorGUILayout.ObjectField("Front Ground Check", _player.forwardGroundCheck, typeof(Transform), true);
			_player.backwardGroundCheck = (Transform)EditorGUILayout.ObjectField("Rear Ground Check", _player.backwardGroundCheck, typeof(Transform), true);
			EditorGUI.indentLevel--;
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_player);
		}
	}
}
