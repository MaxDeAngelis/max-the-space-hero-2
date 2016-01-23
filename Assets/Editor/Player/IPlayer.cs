using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class IPlayer : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	CONSTANTS						     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _isMovementOptionsVisible = true;
	private bool _isEnergyOptionsVisible = true;
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
			_player.maximumVelocity = EditorGUILayout.FloatField("Maximum Velocity", _player.maximumVelocity);
			_player.boostForce = EditorGUILayout.FloatField("Boost Force", _player.boostForce);
			EditorGUI.indentLevel--;
		}

		// Display expandable section for energy related options
		_isEnergyOptionsVisible = EditorGUILayout.Foldout(_isEnergyOptionsVisible, "Energy Options");
		if (_isEnergyOptionsVisible) {
			// Indent the content of the layout
			EditorGUI.indentLevel++;
			_player.boostCost = EditorGUILayout.FloatField("Boost Cost", _player.boostCost);
			_player.takeOffCost = EditorGUILayout.FloatField("Take Off Cost", _player.takeOffCost);
			_player.flyingEnergyRegenRate = EditorGUILayout.IntField("Flying Regen Rate (frames before regen)", _player.flyingEnergyRegenRate);
			_player.anchoredEnergyRegenRate = EditorGUILayout.IntField("Anchored Regen Rate (frames before regen)", _player.anchoredEnergyRegenRate);

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
			_player.topLandingCheck = (Transform)EditorGUILayout.ObjectField("Top Landing Check", _player.topLandingCheck, typeof(Transform), true);
			_player.bottomLandingCheck = (Transform)EditorGUILayout.ObjectField("Bottom Landing Check", _player.bottomLandingCheck, typeof(Transform), true);
			EditorGUI.indentLevel--;
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_player);
		}
	}
}
