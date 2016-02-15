using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool ignoreClick = false;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Handles Pointer Enter and sets the cursor plus plays the sound
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerEnter(PointerEventData eventData) {
		SpecialEffectsManager.Instance.playButtonHover();
		CursorManager.Instance.setCursorToPointer();
	}

	/// <summary>
	/// Handles Pointer Exit to reset the cursor
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerExit(PointerEventData eventData) {
		CursorManager.Instance.resetCursor();
	}

	/// <summary>
	/// Handles the Pointer Click event
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData) {
		if (!ignoreClick) {
			SpecialEffectsManager.Instance.playButtonClick();
		}
	}
}
