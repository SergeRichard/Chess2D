using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsManager : MonoBehaviour {

	public Dropdown humanPlaysDropDown;
	public InputField time;

	public void ChangeHumanColour() {

		switch (humanPlaysDropDown.value) {
		case 0:
			GameManager.humanPlays = GameManager.HumanPlays.White;
			break;
		case 1:
			GameManager.humanPlays = GameManager.HumanPlays.Black;
			break;
		case 2:
			GameManager.humanPlays = GameManager.humanPlays == GameManager.HumanPlays.White ? GameManager.HumanPlays.Black : GameManager.HumanPlays.White;
			break;
		}
	}
	public void OnTimeChange() {
		Debug.Log ("OnTimeChange");
		if (time.text != string.Empty) {
			GameManager.minutes = int.Parse (time.text);
		}

	}
}
