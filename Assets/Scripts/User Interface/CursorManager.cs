using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CURSOR_TYPE { Default, Crosshairs, Pointer};
public class CursorManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	[Header("Cursors Textures")]
	public Texture2D crosshairCursor;								// Crosshair cursor
	public Texture2D pointerCursor;									// Pointer cursor
	public Texture2D defaultCursor;									// Default cursor

	public static CursorManager Instance;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private CURSOR_TYPE _originalCursor;			// Stores the originally used cursor

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of CursorManager!");
			Destroy(gameObject);
		}
		Instance = this;

		// Call OnLevelLoad to init all cursor settings
		OnLevelWasLoaded(0);
	}

	/// <summary>
	/// Called on level load to set the renderer camera of the HUD
	/// </summary>
	/// <param name="level">The level number</param>
	void OnLevelWasLoaded(int level) {
		/* -- INIT VARIABLES -- */
		_originalCursor = SceneSettings.Instance.startingCursor;

		/* -- INIT DISPLAY -- */
		setCursor(_originalCursor);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// called to change the cursor that is being used
	/// </summary>
	/// <param name="type">The type of cursor to change to</param>
	public void setCursor(CURSOR_TYPE type) {
		Texture2D _cursorTexture = null;
		Vector2 _cursorOffset = Vector2.zero;

		// Figure out what cursor to use
		switch(type) {
		case CURSOR_TYPE.Crosshairs:
			_cursorTexture = crosshairCursor;
			_cursorOffset = new Vector2(13f, 13f);
			break;
		case CURSOR_TYPE.Pointer:
			_cursorTexture = pointerCursor;
			_cursorOffset = new Vector2(12f, 3f);
			break;
		case CURSOR_TYPE.Default:
			_cursorTexture = defaultCursor;
			break;
		}

		// Set the cursor texture
		Cursor.SetCursor(_cursorTexture, _cursorOffset, CursorMode.Auto);
	}

	public void setCursorToPointer() {
		setCursor(CURSOR_TYPE.Pointer);
	}

	/// <summary>
	/// Called to reset the cursor back to the original
	/// </summary>
	public void resetCursor() {
		setCursor(_originalCursor);
	}
}
