using UnityEngine;
using System.Collections;

public class SceneSettings : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool isMiniMapEnabled;
	public bool isWeaponEnabled;
	public bool isJetpackEnabled;

	public static SceneSettings Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	private void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of SceneManager!");
			Destroy(gameObject);
		}
		Instance = this;
	}

	private void Start() {
		// Set if mini map is enabled for this scene
		GameManager.Instance.setMiniMapState(isMiniMapEnabled);

		// Set the state of the Jetpack
		PlayerManager.Instance.getJetpack().setState(isJetpackEnabled);

		// Set the state of the Weapon
		PlayerManager.Instance.getWeapon().setState(isWeaponEnabled);
	}
}
