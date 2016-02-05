using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float distance = -7.5f;				// Z Distance the camera should maintain
	public float damper = 0.1f;					// Damper value to user for smooth follow
	
	public GameObject rightBoundry;				// Right boundry game object
	public GameObject leftBoundry;				// Left boundry game object
	public GameObject topBoundry;				// Top boundry game object
	public GameObject bottomBoundry;			// Bottom boundry game object

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private Vector3 _target;					// The loaction to target for the camera
	private Vector3 _velocity = Vector3.zero;	// Reference value for zero velocity
	private Camera _camera;
	private float _cameraWidth;					// Camera width in game
	private float _cameraHeight;				// Camera height in game
	private bool _targetMouse = false;

	private CameraManager Instance;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on start of the game object to init variables
	/// </summary>
	void Start() {
		// Register the singleton
		if (Instance != null) {
			Debug.LogError("Multiple instances of CameraManager!");
		}

		Instance = this;

		transform.position = PlayerManager.Instance.getLocation();

		/* INIT COMPONENTS */
		_camera = GetComponent<Camera>();

		// Set width and height of the camera
		_cameraHeight = 2f * _camera.orthographicSize;
		_cameraWidth = _cameraHeight * _camera.aspect;
	}

	/// <summary>
	/// Called once per frame after all updates and fixed updates finish
	/// </summary>
	void LateUpdate () {
		/* TODO: Commented out the target mouse logic because as it targets is keeps following mouse
		 * seems a bit weird but didnt want to loose code so leaving disabled for now
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			_targetMouse = true;

			Vector3 pos = Input.mousePosition;
			pos.z = transform.position.z - Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			
			_target = pos;
		} 
		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			_targetMouse = false;
		}
		*/

		if (!_targetMouse) {
			_target = PlayerManager.Instance.getLocation();
		}

		// Setup default positions
		float newXLocation = _target.x;
		float newYLocation = _target.y;

		// Calculate the boundries of the screen relative to the game space
		float cameraRightBorder = newXLocation + (_cameraWidth/2);
		float cameraLeftBorder = newXLocation - (_cameraWidth/2);
		float cameraTopBorder = newYLocation + (_cameraHeight/2);
		float cameraBottomBorder = newYLocation - (_cameraHeight/2);

		/* ---- RIGHT BOUNDRY ---- */
		if (cameraRightBorder >= rightBoundry.transform.position.x) {
			newXLocation = rightBoundry.transform.position.x - (_cameraWidth/2);
		}

		/* ---- LEFT BOUNDRY ---- */
		if (cameraLeftBorder <= leftBoundry.transform.position.x) {
			newXLocation = leftBoundry.transform.position.x + (_cameraWidth/2);
		}

		/* ---- TOP BOUNDRY ---- */
		if (cameraTopBorder >= topBoundry.transform.position.y) {
			newYLocation = topBoundry.transform.position.y - (_cameraHeight/2);
		}

		/* ---- BOTTOM BOUNDRY ---- */
		if (cameraBottomBorder <= bottomBoundry.transform.position.y) {
			newYLocation = bottomBoundry.transform.position.y + (_cameraHeight/2);
		}

		// Move camera using smooth damp to give a smooth follow feel
		transform.position = Vector3.SmoothDamp(transform.position, new Vector3(newXLocation, newYLocation, distance), ref _velocity, damper);
	}
		
	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		float z = 1f;
		// Get a reference to boundries positions to make math cleaner
		Vector3 top = topBoundry.transform.position;
		Vector3 bottom = bottomBoundry.transform.position;
		Vector3 left = leftBoundry.transform.position;
		Vector3 right = rightBoundry.transform.position;

		// Draw a line on all 4 sides
		Gizmos.DrawLine(new Vector3(left.x, top.y, z), new Vector3(left.x, bottom.y, z));
		Gizmos.DrawLine(new Vector3(left.x, top.y, z), new Vector3(right.x, top.y, z));
		Gizmos.DrawLine(new Vector3(right.x, top.y, z), new Vector3(right.x, bottom.y, z));
		Gizmos.DrawLine(new Vector3(right.x, bottom.y, z), new Vector3(left.x, bottom.y, z));
	}
}
