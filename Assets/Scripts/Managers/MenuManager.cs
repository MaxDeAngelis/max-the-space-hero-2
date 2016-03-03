using UnityEngine;

public enum MENU_TYPE {Pause, Controls, GameOver, LevelComplete, LevelSelect, MedicalShop, Armory, Tech_1, Tech_2};
public class MenuManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	[Header("Menu Objects")]
	public GameObject pauseMenu;									// Pause Menu canvas game object
	public GameObject controlsMenu;
	public GameObject gameOverMenu;									// Game Over canvas game object
	public GameObject levelCompleteMenu;
	public GameObject levelSelectMenu;
	public GameObject medicalShopMenu;
	public GameObject armoryShopMenu;
	public GameObject techShopMenu1;
	public GameObject techShopMenu2;


	[Header("Sounds")]
	public AudioClip showSound;
	public AudioClip hideSound;

	public static MenuManager Instance;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private bool _pause = false;					// Flag for when the game is paused
	private GameObject _activeMenu;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Awake() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of MenuManager!");
			Destroy(gameObject);
		}
		Instance = this;

		/* -- INIT VARIABLES -- */

		/* -- INIT DISPLAY -- */
	}
		
	/// <summary>
	/// called once per frame. Used to capture key events for later
	/// </summary>
	void Update() {
		if (Input.GetButtonDown("Cancel") && !isPaused()) { 
			showMenu(MENU_TYPE.Pause);
		} else if (Input.GetButtonDown("Cancel") && isPaused()) { 
			hideMenu();

			// If hiding the menu using the esc key play hide sound
			SpecialEffectsManager.Instance.playSound(hideSound);
		}
	}


		
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to show a given menu. Handles pausing the game and setting correct flags
	/// </summary>
	/// <param name="menu">The menu object to show</param>
	public void showMenu(MENU_TYPE menuType) {
		bool pauseTime = true;
		hideMenu();

		switch (menuType) {
		case MENU_TYPE.Pause:
			SpecialEffectsManager.Instance.playSound(showSound);
			_activeMenu = pauseMenu;
			break;
		case MENU_TYPE.Controls:
			_activeMenu = controlsMenu;
			break;
		case MENU_TYPE.GameOver:
			pauseTime = false;
			_activeMenu = gameOverMenu;
			break;
		case MENU_TYPE.LevelComplete:
			_activeMenu = levelCompleteMenu;
			break;
		case MENU_TYPE.LevelSelect:
			_activeMenu = levelSelectMenu;
			break;
		case MENU_TYPE.MedicalShop:
			_activeMenu = medicalShopMenu;
			break;
		case MENU_TYPE.Armory:
			_activeMenu = armoryShopMenu;
			break;
		case MENU_TYPE.Tech_1:
			_activeMenu = techShopMenu1;
			break;
		case MENU_TYPE.Tech_2:
			_activeMenu = techShopMenu2;
			break;
		}

		// If time is suposed to pause then pause it
		if (pauseTime) { 
			pause();
		}

		_activeMenu.SetActive(true);
	}

	/// <summary>
	/// Called to hid what ever menu is currently showing
	/// </summary>
	public void hideMenu() {
		resume();

		if (_activeMenu) {
			_activeMenu.SetActive(false);
			_activeMenu = null;
		}
	}

	/// <summary>
	/// called to pause the game 
	/// </summary>
	public void pause() {
		_pause = true;
		CursorManager.Instance.setCursor(CURSOR_TYPE.Default);
		Time.timeScale = 0;
	}

	/// <summary>
	/// Called to un pause the game
	/// </summary>
	public void resume() {
		_pause = false;
		CursorManager.Instance.resetCursor();
		Time.timeScale = 1;
	}

	/// <summary>
	/// Called to see if the game is paused
	/// </summary>
	/// <returns><c>true</c>, if paused was ised, <c>false</c> otherwise.</returns>
	public bool isPaused() {
		return _pause;
	}
}
