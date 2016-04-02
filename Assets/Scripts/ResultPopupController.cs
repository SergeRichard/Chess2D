using UnityEngine;
using System.Collections;

public class ResultPopupController : MonoBehaviour {
	public GameObject Background;
	public GameObject MateText;
	public GameObject StalemateText;

	public bool WindowOpen = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ShowMateWindow() {
		Background.SetActive (true);
		MateText.SetActive (true);
		WindowOpen = true;
	}
	public void ShowStalemateWindow() {
		Background.SetActive (true);
		StalemateText.SetActive (true);
		WindowOpen = true;
	}
	public void HideWindow() {
		Background.SetActive (false);
		MateText.SetActive (false);
		StalemateText.SetActive (false);
		WindowOpen = false;
	}
}
