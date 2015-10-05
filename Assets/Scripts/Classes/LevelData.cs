using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class LevelData {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private string _name;
	private string _title;
	private float _time;
	private float _accuracy;
	private float _completion;
	private float _score;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public LevelData(string name, string title) {
		_name = name;
		_title = title;
		_time = 0f;
		_accuracy = 0f;
		_completion = 0f;
		_score = 0f;
	}

	public LevelData(string name, string title, float time, float accuracy, float completion, float score) {
		_name = name;
		_title = title;
		_time = time;
		_accuracy = accuracy;
		_completion = completion;
		_score = score;
	}

	public string getTitle() {
		return _title;
	}

	public string getName() {
		return _name;
	}

	public string getCompletion() {
		return _completion.ToString() + "%";
	}

	public void setCompletion(float completion) {
		_completion = completion;
	}
}
