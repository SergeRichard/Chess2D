using UnityEngine;
using System.Collections;

public class DetectSquareClick : MonoBehaviour {

	void OnMouseDown() {
		GameManager gameManager = GameObject.FindObjectOfType<GameManager> ();
		gameManager.SquareClicked (transform.name);

	}
}
