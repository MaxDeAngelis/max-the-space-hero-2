using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelTile : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Button button;
	public Text number;
	public Text title;
	public Text completion;
	public Text time;
	public Text accuracy;
	public Text score;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to set the content of a level tile
	/// </summary>
	/// <param name="levelNumber">The number of the level</param>
	/// <param name="level">The level data object</param>
	public void setContent(int levelNumber, LevelData level) {
		number.text = levelNumber.ToString();
		title.text = level.getTitle();
		completion.text = level.getCompletion();
		time.text = level.getTime ();
		accuracy.text = level.getAccuracy();
		score.text = level.getScore();

		button.onClick.AddListener(delegate{
			GameManager.Instance.loadLevel(level.getName());
		});
	}

	/// <summary>
	/// Handles when the mouse enters the level tile object
	/// </summary>
	public void handleMouseEnter() {
		SpecialEffectsManager.Instance.playButtonHover();
		CursorManager.Instance.setCursorToPointer();
	}

	/// <summary>
	/// Handles when the mouse leaves the level tile
	/// </summary>
	public void handleMouseLeave() {
		CursorManager.Instance.resetCursor();
	}
}
