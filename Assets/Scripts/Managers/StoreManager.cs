using UnityEngine;
using System.Collections;

public class StoreManager : MonoBehaviour {
	public static StoreManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Called when the game object wakes up
	/// </summary>
	private void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of StoreManager!");
		}

		Instance = this;
	}
		
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Buys new health for the player
	/// </summary>
	/// <returns><c>true</c>, if health was bought, <c>false</c> otherwise.</returns>
	/// <param name="amount">Amount of heath to buy</param>
	/// <param name="cost">Cost of health</param>
	public bool buyHealth(int amount, int cost) {
		// If there is enough score then buy new health otherwise fail
		if (DataManager.Instance.getCurrentPlayerData().getScore() >= cost) {
			// Add to the health and remove from score then save
			DataManager.Instance.updateHealth(amount);
			DataManager.Instance.updateScore(-cost);
			DataManager.Instance.save();

			// Display the health added on HUD
			Utilities.Instance.showFadeAwayText(GameObject.FindGameObjectWithTag("Health").transform, "+" + amount, Color.red);

			// Refresh hud to display new health and score
			GameManager.Instance.refreshHUD();
			return true;
		} else {
			return false;
		}
	}
}
