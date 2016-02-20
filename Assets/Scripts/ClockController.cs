﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClockController : MonoBehaviour {
	private string timeToDisplayForWhite;
	private string timeToDisplayForBlack;
	public int hoursForWhite;
	public int minutesForWhite;
	public float secondsForWhite;
	public int hoursForBlack;
	public int minutesForBlack;
	public float secondsForBlack;

	public Text TimeWhiteText;
	public Text TimeBlackText;

	public enum StartClockState {Wait, ForWhite, ForBlack};

	public StartClockState clockState = StartClockState.Wait;

	void Start() {
		timeToDisplayForWhite = FormatTime(hoursForWhite,minutesForWhite,secondsForWhite);
		timeToDisplayForBlack = FormatTime(hoursForBlack,minutesForBlack,secondsForBlack);
		TimeWhiteText.text = timeToDisplayForWhite;
		TimeBlackText.text = timeToDisplayForBlack;
	}

	public void StartClock(StartClockState clockState) {
		this.clockState = clockState;
	}
	public void Update() {
		switch (clockState) {
		case StartClockState.ForWhite:
			CalculateTimeForWhite ();
			timeToDisplayForWhite = FormatTime (hoursForWhite,minutesForWhite,secondsForWhite);
			TimeWhiteText.text = timeToDisplayForWhite;
			break;
		case StartClockState.ForBlack:
			CalculateTimeForBlack ();
			timeToDisplayForBlack = FormatTime (hoursForBlack,minutesForBlack,secondsForBlack);
			TimeBlackText.text = timeToDisplayForBlack;
			break;
		}
	}
	#region helper methods
	void CalculateTimeForWhite()
	{
		// adding seconds
		secondsForWhite -= Time.deltaTime;
		// adding minutes
		if (Mathf.Floor (secondsForWhite) < 0) {
			secondsForWhite = 60;
			minutesForWhite--;
		}
		if (minutesForWhite < 0) {
			minutesForWhite = 59;
			hoursForWhite--;
		}

	}
	void CalculateTimeForBlack()
	{
		// adding seconds
		secondsForBlack -= Time.deltaTime;
		// adding minutes
		if (Mathf.Floor (secondsForBlack) < 0) {
			secondsForBlack = 60;
			minutesForBlack--;
		}
		if (minutesForBlack < 0) {
			minutesForBlack = 59;
			hoursForBlack--;
		}

	}
	string FormatTime(int hours, int minutes, float seconds) {
		return hours.ToString () 
			+ ":" + (minutes<10 ? "0" + minutes.ToString() : minutes.ToString ()) 
			+ ":" + (Mathf.Floor (seconds) < 10 ? "0" + Mathf.Floor (seconds).ToString () : Mathf.Floor (seconds).ToString ());

	}
	#endregion
}
