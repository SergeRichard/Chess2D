using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameManager : MonoBehaviour {
	
	enum State {WhiteSelectionState, WhiteDestinationState, BlackSelectionState, BlackDestinationState};
	State state = State.WhiteSelectionState;
	public List<GameObject> piecesPrefab = new List<GameObject>();
	private List<Square> squares = new List<Square>();

	public static string[,] piecesOnBoard = new string[,] { 
		{"BR","BN","BB","BK","BQ","BB","BN","BR"},
		{"BP","BP","BP","BP","BP","BP","BP","BP"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"WP","WP","WP","WP","WP","WP","WP","WP"},
		{"WR","WN","WB","WK","WQ","WB","WN","WR"},
	};
	public static string[,] squareNames = new string[,] { 
		{"A8","B8","C8","D8","E8","F8","G8","H8"},
		{"A7","B7","C7","D7","E7","F7","G7","H7"},
		{"A6","B6","C6","D6","E6","F6","G6","H6"},
		{"A5","B5","C5","D5","E5","F5","G5","H5"},
		{"A4","B4","C4","D4","E4","F4","G4","H4"},
		{"A3","B3","C3","D3","E3","F3","G3","H3"},
		{"A2","B2","C2","D2","E2","F2","G2","H2"},
		{"A1","B1","C1","D1","E1","F1","G1","H1"},
	};
	[Serializable]
	public class Square
	{
		public Transform squareTransform;
		public string name;

		public Square(Transform t, string n) {
			squareTransform = t;
			name = n;
		}

	}
	// Use this for initialization
	void Start () {
		GetSquareTransforms ();
		InitializeBoard ();

	}
	void GetSquareTransforms()	{
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				string squareName = squareNames [col, row];
				Transform t = GameObject.Find (squareName).transform;
				Square square = new Square (t, squareName);
				squares.Add (square);
			}
		}

	}
	void InitializeBoard() {
		//Instantiate (piecesPrefab [0], new Vector3 (0, 0, 0), Quaternion.identity);
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				// "--" means it's an uninhabited square
				if (piecesOnBoard[col,row] != "--") {
					Transform squareTransform = FindPosition (row, col);
					GameObject piecePrefab = FindPrefab (row, col);
					GameObject piece = Instantiate (piecePrefab, squareTransform.position, Quaternion.identity) as GameObject;
					piece.transform.parent = squareTransform;
					piece.GetComponent<SpriteRenderer> ().sortingOrder = 5;
				}

			}
		}
	}
	Transform FindPosition(int row, int column) {
		// TODO find square with matching squareNames name, then return the transform of the square.
		foreach (var square in squares) {
			if (squareNames[column,row] == square.name) {
				return square.squareTransform;
			}

		}
		return null;
	}
	GameObject FindPrefab(int row, int column)
	{
		foreach (var p in piecesPrefab) {
			if (p.GetComponent<Piece> ().shortName == piecesOnBoard [column, row]) {
				return p;
			}
		}
		return null;
	}
}
