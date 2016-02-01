using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Jetpack))]
public class IJetpack : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	   	PUBLIC FUNCTIONS    										     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		Jetpack _jetpack = target as Jetpack;

		// Display expandable section for movement related variables
		EditorGUILayout.LabelField("Movement Options", EditorStyles.boldLabel);
		_jetpack.maximumVelocity = EditorGUILayout.FloatField("Maximum Velocity", _jetpack.maximumVelocity);
		_jetpack.boostForce = EditorGUILayout.FloatField("Boost Force", _jetpack.boostForce);

		// Display section for landing options
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Landing Options", EditorStyles.boldLabel);
		_jetpack.topLandingCheck = (Transform)EditorGUILayout.ObjectField("Top Landing Range Check", _jetpack.topLandingCheck, typeof(Transform), true);
		_jetpack.bottomLandingCheck = (Transform)EditorGUILayout.ObjectField("Bottom Landing Range Check", _jetpack.bottomLandingCheck, typeof(Transform), true);

		// Display expandable section for energy related options.
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Energy Options", EditorStyles.boldLabel);
		_jetpack.boostCost = EditorGUILayout.FloatField("Boost Cost", _jetpack.boostCost);
		_jetpack.takeOffCost = EditorGUILayout.FloatField("Take Off Cost", _jetpack.takeOffCost);
		_jetpack.flyingEnergyRegenRate = EditorGUILayout.IntField("Flying Regen Rate (frames before regen)", _jetpack.flyingEnergyRegenRate);
		_jetpack.anchoredEnergyRegenRate = EditorGUILayout.IntField("Anchored Regen Rate (frames before regen)", _jetpack.anchoredEnergyRegenRate);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_jetpack);
		}
	}
}
