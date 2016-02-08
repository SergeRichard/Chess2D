using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {

	enum State {WhiteSelectionState, WhiteDestinationState, BlackSelectionState, BlackDestinationState};
	State state = State.WhiteSelectionState;

	public static string[,] board = new string[,] { 
		{"BR","BN","BB","BK","BQ","BB","BN","BR"},
		{"BP","BP","BP","BP","BP","BP","BP","BP"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"WP","WP","WP","WP","WP","WP","WP","WP"},
		{"WR","WN","WB","WK","WQ","WB","WN","WR"},
	};
	[System.Serializable]
	public class Pieces {

		public string name;
		public string column;
		public string row;
		public float xpos;
		public float ypos;
	}

	// Use this for initialization
	void Start () {
		//foreach (var piece in board) {


		//}

	}

}
