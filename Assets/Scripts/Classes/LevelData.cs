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
	private int _score;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTORS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public LevelData(string name, string title) {
		_name = name;
		_title = title;
		_time = 0f;
		_accuracy = 0f;
		_completion = 0f;
		_score = 0;
	}

	public LevelData(string name, string title, float time, float accuracy, float completion, int score) {
		_name = name;
		_title = title;
		_time = time;
		_accuracy = accuracy;
		_completion = completion;
		_score = score;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			GETTERS													     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public string getTitle() {
		return _title;
	}

	public string getName() {
		return _name;
	}

	public string getCompletion() {
		return _completion.ToString() + "%";
	}

	public string getTime() {
		string minutes = Mathf.Floor(_time / 60).ToString("00");
		string seconds = (_time % 60).ToString("00");
		
		return minutes + ":" + seconds;
	}

	public string getAccuracy() {
		return _accuracy.ToString() + "%";
	}

	public string getScore() {
		return _score.ToString();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     				SETTERS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void setCompletion(float completion) {
		_completion = Mathf.Round(completion);
	}

	public void setTime(float time) {
		_time = time;
	}

	public void setAccuracy(float accuracy) {
		_accuracy = Mathf.Round(accuracy);
	}

	public void setScore(int score) {
		_score = score;
	}
}
