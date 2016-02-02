using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelList : MonoBehaviour {	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public GameObject levelTile;
	public Transform levelList;	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Start() {
		List<LevelData> levels = DataManager.Instance.getCurrentGameData().getLevels();
		int count = 1;

		// Loops over all levels stored off and creates a button for them
		foreach(LevelData currentLevel in levels) {
			GameObject newLevelTitle = Instantiate(levelTile);
			LevelTile newLevelTitleController = newLevelTitle.GetComponent<LevelTile>();
			
			newLevelTitleController.setContent(count++, currentLevel);
			newLevelTitle.transform.SetParent(levelList, false);
		}
	}
}