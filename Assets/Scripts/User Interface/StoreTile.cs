using UnityEngine;
using System.Collections;

public class StoreTile : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public enum TYPE {Health, Energy, Shield, Damage};

	public TYPE type;					// The type of the store this tile is part of
	public int cost;					// The cost of what ever is for sale
	public int amount;					// The amount gained by the purchase

	public AudioClip successSound;		// Sound played on success
	public AudioClip failedSound;		// Sound played on failure

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/**/
	/// <summary>
	/// Called to display a bit of text by an object
	/// </summary>
	/// <param name="tagName">Tag name of the object to display the text over</param>
	/// <param name="text">Text to display</param>
	/// <param name="textColor">Color of the text</param>
	private void _showIncreaseText(string tagName, string text, Color textColor) {
		Utilities.Instance.showFadeAwayText(GameObject.FindGameObjectWithTag(tagName).transform, text, textColor);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on click of a shop tile to buy what is for sale
	/// </summary>
	public void buy() {
		//DataManager.Instance.updateScore(10000);
		// If there is enough score then buy new health otherwise fail
		if (DataManager.Instance.getCurrentPlayerData().getScore() >= cost) {

			// Based on what is being purchased call the singleton to process the order
			switch (type) {
			case TYPE.Health:
				DataManager.Instance.updateHealth(amount);
				_showIncreaseText("HealthTotal", "+" + amount, Color.red);
				break;
			case TYPE.Energy:
				DataManager.Instance.updateEnergy(amount);
				_showIncreaseText("EnergyTotal", "+" + amount, Color.blue);
				break;
			case TYPE.Shield:
				DataManager.Instance.updateShield(amount);
				_showIncreaseText("ShieldTotal", "+" + amount, Color.green);
				break;
			case TYPE.Damage:
				//DataManager.Instance.updateShield(amount);
				_showIncreaseText("DamageTotal", "+" + amount, Color.yellow);
				break;
			}

			// Update scroe, save and refresh hud to display new health and score
			DataManager.Instance.updateScore(-cost);
			DataManager.Instance.save();
			GameManager.Instance.refreshHUD();

			// Play the success sound
			SpecialEffectsManager.Instance.playSound(successSound);
		} else {
			SpecialEffectsManager.Instance.playSound(failedSound);
		}
	}
}
