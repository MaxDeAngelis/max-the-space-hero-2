using UnityEngine;
using System.Collections;

public class DontDestroyOnLoad : MonoBehaviour {
	public static DontDestroyOnLoad Instance;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on awake
	/// </summary>
	void Awake() {
		if (Instance != null) {
			Debug.Log("Multiple instances of DontDestroyOnLoad!");
			DestroyImmediate(gameObject);
		} else {
			Instance = this;

			DontDestroyOnLoad(gameObject);
		}
	}
}
