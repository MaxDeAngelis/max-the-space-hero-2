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
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on click of a shop tile to buy what is for sale
	/// </summary>
	public void buy() {
		bool isAbleToBuy = false;

		// Based on what is being purchased call the singleton to process the order
		switch (type) {
			case TYPE.Health:
				isAbleToBuy = StoreManager.Instance.buyHealth(amount, cost);
				break;
			case TYPE.Energy:
				isAbleToBuy = StoreManager.Instance.buyEnergy(amount, cost);
				break;
		}

		// If you were able to buy something something then play correct sound effect
		if (isAbleToBuy) {
			SpecialEffectsManager.Instance.playSound(successSound);
		} else {
			SpecialEffectsManager.Instance.playSound(failedSound);
		}
	}
}
