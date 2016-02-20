using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameManager : MonoBehaviour {

	enum State {
		IntroState, 
		WhiteSelectionState, 
		WhiteDestinationState, 
		CheckIfLegalWhiteMoveState, 
		CheckIfWhiteMatesState, 
		CheckIfWhiteStaleMatesState,
		MoveWhitePieceState,
		BlackSelectionState, 
		BlackDestinationState,
		CheckIfLegalBlackMoveState, 
		CheckIfBlackMatesState, 
		CheckIfBlackStaleMatesState,
		MoveBlackPieceState,
		ResultState
	};
	enum InputState {
		WaitingForClick,
		SquareIsClicked
	}
	InputState inputState = InputState.WaitingForClick;
	static string clickedSquareName = "";
	static string selectionSquareName = "";
	static string destinationSquareName = "";
	List<BoardPosition> boardPositions = new List<BoardPosition>();

	int plyMove = 0;

	State state = State.IntroState;
	public List<GameObject> piecesPrefab = new List<GameObject>();
	private List<Square> squares = new List<Square>();
	public ClockController clockController;
	public Text moveNotationText;

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
		DirectToState ();


	}
	void DirectToState() {

		switch (state) {
			case State.IntroState:
				IntroState ();
				break;
			//////////////////
			// White States //
			//////////////////
			case State.WhiteSelectionState:
				WhiteSelectionState ();
				break;
			case State.WhiteDestinationState:
				WhiteDestinationState ();
				break;
			case State.CheckIfLegalWhiteMoveState:
				CheckIfLegalWhiteMoveState ();
				break;
			case State.CheckIfWhiteMatesState:
				CheckIfWhiteMatesState ();
				break;
			case State.CheckIfWhiteStaleMatesState:
				CheckIfWhiteStaleMatesState ();
				break;
			case State.MoveWhitePieceState:
				MoveWhitePieceState ();
				break;
			//////////////////
			// Black States //
			//////////////////
			case State.BlackSelectionState:
				BlackSelectionState ();
				break;
			case State.BlackDestinationState:
				BlackDestinationState ();
				break;
		
			case State.CheckIfLegalBlackMoveState:
				CheckIfLegalBlackMoveState ();
				break;
			case State.CheckIfBlackMatesState:
				CheckIfBlackMatesState ();
				break;
			case State.CheckIfBlackStaleMatesState:
				CheckIfBlackStaleMatesState ();
				break;
			case State.MoveBlackPieceState:
				MoveBlackPieceState ();
				break;
			case State.ResultState:
				ResultState ();
				break;
			default:
				Debug.LogError("Invalid State.");
				break;
		}


	}

	#region States
	void IntroState() {
		GetSquareTransforms ();
		InitializeBoard ();
		state = State.WhiteSelectionState;
		clockController.clockState = ClockController.StartClockState.Wait;
		DirectToState ();
	}
	void WhiteSelectionState() {
		Debug.Log ("Inside WhiteSelectionState");
		if (inputState == InputState.SquareIsClicked) {
			inputState = InputState.WaitingForClick;
			HighlightSquare ();
			selectionSquareName = clickedSquareName;
			state = State.WhiteDestinationState;
			DirectToState ();
		}
		//state = State.WhiteDestinationState;
	}
	void WhiteDestinationState() {
		Debug.Log ("Inside WhiteDestinationState");
		if (inputState == InputState.SquareIsClicked) {
			inputState = InputState.WaitingForClick;
			HighlightSquare ();
			destinationSquareName = clickedSquareName;
			state = State.CheckIfLegalWhiteMoveState;
			DirectToState ();
		}
		//state = State.BlackSelectionState;
	}
	void CheckIfLegalWhiteMoveState () {
		Debug.Log ("Inside CheckIfLegalWhiteMoveState");
		bool legal = CheckIfWhitesMoveIsLegal ();
		if (legal) {
			state = State.MoveWhitePieceState;

		} else {
			Debug.Log ("Illegal move!");

			state = State.WhiteSelectionState;
		}
		UnhighlightSquares ();
		DirectToState ();
	}
	void MoveWhitePieceState() {
		Debug.Log ("Inside MoveWhitePieceState");
		MovePieceToDestination ();
		state = State.CheckIfWhiteMatesState;
		clockController.clockState = ClockController.StartClockState.ForBlack;
		DirectToState ();	

	}
	void CheckIfWhiteMatesState() {
		Debug.Log ("Inside CheckIfWhiteMatesState");
		bool mates = CheckIfWhiteMates ();
		if (mates) {
			state = State.ResultState;
			DirectToState ();
		} else {
			state = State.CheckIfWhiteStaleMatesState;
			DirectToState ();
		}
	}
	void CheckIfWhiteStaleMatesState() {
		Debug.Log ("Inside CheckIfWhiteStaleMatesState");
		bool staleMates = CheckIfWhiteHasStaleMated ();
		if (staleMates) {
			state = State.ResultState;

		} else {
			state = State.BlackSelectionState;
		}
		DirectToState ();
	}

	void BlackSelectionState() {
		Debug.Log ("Inside BlackSelectionState");
		if (inputState == InputState.SquareIsClicked) {
			inputState = InputState.WaitingForClick;
			HighlightSquare ();
			selectionSquareName = clickedSquareName;
			state = State.BlackDestinationState;
			DirectToState ();
		}
	}
	void BlackDestinationState() {
		Debug.Log ("Inside BlackDestinationState");
		if (inputState == InputState.SquareIsClicked) {
			inputState = InputState.WaitingForClick;
			HighlightSquare ();
			destinationSquareName = clickedSquareName;
			state = State.CheckIfLegalBlackMoveState;
			DirectToState ();
		}
	}
	void CheckIfLegalBlackMoveState() {
		Debug.Log ("Inside CheckIfLegalBlackMoveState");
		bool legal = CheckIfBlacksMoveIsLegal ();
		if (legal) {
			state = State.MoveBlackPieceState;

		} else {
			Debug.Log ("Illegal move!");

			state = State.BlackSelectionState;
		}
		UnhighlightSquares ();
		DirectToState ();

	}
	void MoveBlackPieceState() {
		Debug.Log ("Inside MoveBlackPieceState");
		MovePieceToDestination ();
		state = State.CheckIfBlackMatesState;
		clockController.clockState = ClockController.StartClockState.ForWhite;
		DirectToState ();
	}
	void CheckIfBlackMatesState() {
		Debug.Log ("Inside CheckIfBlackMatesState");
		bool mates = CheckIfBlackMates ();
		if (mates) {
			state = State.ResultState;
			DirectToState ();
		} else {
			state = State.CheckIfBlackStaleMatesState;
			DirectToState ();
		}
	}
	void CheckIfBlackStaleMatesState() {
		Debug.Log ("Inside CheckIfBlackStaleMatesState");
		bool staleMates = CheckIfBlackHasStaleMated ();
		if (staleMates) {
			state = State.ResultState;

		} else {
			state = State.WhiteSelectionState;
		}
		DirectToState ();
	}

	void ResultState() {
		Debug.Log ("Inside ResultState");

	}
	#endregion
	#region Events
	public void SquareClicked(string squareName) {
		Debug.Log (squareName + " clicked!");
		clickedSquareName = squareName;
		inputState = InputState.SquareIsClicked;
		DirectToState ();
	}

	#endregion
	#region helper functions
	void MovePieceToDestination() {
		string[,] tempBoard = new string[8, 8];
		string temp = "";
		string textNotation = GetMoveTextNotation();

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				tempBoard[col,row] = boardPositions[plyMove].piecesOnBoard [col, row];

			}
		}
		// find the selection square and assign an empty space to it
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (selectionSquareName == squareNames [col, row]) {
					temp = tempBoard[col,row];
					tempBoard [col, row] = "--"; 
				}
			}
		}
		// Find the destination square and assign the the temp variable holding the selection piece to the destination square.
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (destinationSquareName == squareNames [col, row]) {
					tempBoard [col, row] = temp; 
				}
			}
		}
		// create text notation before plyMove++

		//Debug.Log ("Move: " + textNotation);

		plyMove++;
		BoardPosition boardPosition = new BoardPosition (plyMove, textNotation);
		boardPosition.piecesOnBoard = new string[8, 8];

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				boardPosition.piecesOnBoard[col, row] = tempBoard [col, row];
			}
		}
		boardPositions.Add (boardPosition);
		RefreshMoveNotationBoard();
		RemoveAllPieces ();
		UpdateBoard ();
	}
	string GetMoveTextNotation() {
		string notation = string.Empty;

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (selectionSquareName == squareNames [col, row]) {
					notation += FormatSelectionPartOfNotation (squareNames, col, row);
				}
			}
		}
		if (PieceIsMoving()) {
			notation += "-";
		} else if (PieceIsAttacking()) {
			notation += "x";
		}
		foreach (string sn in squareNames) {
			if (destinationSquareName == sn) {
				notation += sn.ToLower();
			}
		}
		return notation;
	}
	bool PieceIsMoving() {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (squareNames [col, row] == destinationSquareName) {
					return boardPositions [plyMove].piecesOnBoard [col, row] == "--";
				}
			}

		}
		return false;
	}
	bool PieceIsAttacking() {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (squareNames [col, row] == destinationSquareName) {
					return boardPositions [plyMove].piecesOnBoard [col, row] != "--";
				}
			}

		}
		return false;
	}
	void RefreshMoveNotationBoard() {
		string notation = string.Empty;
		int moveCounter = 0;
		int counter = 0;
		foreach (var bp in boardPositions) {
			// if not initial board position which of course has no moves yet
			if (bp.positionNumber != 0) {
				if (counter % 2 == 0) {
					moveCounter++;
					notation += moveCounter + ".";
				} else {
					notation += "  ";
				}


				notation += bp.moveNotation + (counter%2==0 ? "" : "   ");
				counter++;
			}
		}
		//Debug.Log (notation);
		moveNotationText.text = notation;
	}
	string FormatSelectionPartOfNotation(string[,] squares, int col, int row) {
		string notation = string.Empty;
		string piece = boardPositions [plyMove].piecesOnBoard [col, row];
		string square = squares [col, row];

		// add piece letter to beginning of notation (unless it's a pawn)
		switch (piece) {
		case "WR":
		case "BR":
			notation = "R";
			break;
		case "WN":
		case "BN":
			notation = "N";
			break;
		case "WB":
		case "BB":
			notation = "B";
			break;
		case "WQ":
		case "BQ":
			notation = "Q";
			break;
		case "WK":
		case "BK":
			notation = "K";
			break;
		case "WP":
		case "BP":
			// do nothing. Pawn letter is not mentionned in notation.
			break;
		default:
			Debug.LogError ("No piece found in selectionSquare");
			break;

		}
		notation += square.ToLower();
		return notation;
	}
	void UpdateBoard () {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				// "--" means it's an uninhabited square
				if (boardPositions[plyMove].piecesOnBoard[col,row] != "--") {
					Transform squareTransform = FindPosition (row, col);
					GameObject piecePrefab = FindPrefab (row, col, plyMove);
					GameObject piece = Instantiate (piecePrefab, squareTransform.position, Quaternion.identity) as GameObject;
					piece.transform.parent = squareTransform;
					piece.GetComponent<SpriteRenderer> ().sortingOrder = 5;
				}

			}
		}

	}
	void RemoveAllPieces () {

		foreach (var squareName in squareNames) {
			GameObject sq = GameObject.Find (squareName);
			if (sq.transform.childCount > 0) {
				foreach (Transform s in sq.transform) {
					Destroy (s.gameObject);
				}
			}
				
		}
	}
	bool CheckIfWhitesMoveIsLegal()	{
		return true;
	}
	bool CheckIfBlacksMoveIsLegal() {
		return true;
	}
	bool CheckIfWhiteMates() {
		return false;
	}
	bool CheckIfBlackMates() {
		return false;
	}
	bool CheckIfWhiteHasStaleMated() {
		return false;
	}
	bool CheckIfBlackHasStaleMated() {
		return false;
	}
	void HighlightSquare() {
		foreach (var s in squares) {
			if (s.name == clickedSquareName) {
				s.squareTransform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
			}

		}

	}
	void UnhighlightSquares() {
		foreach (var s in squares) {
			if (s.name == selectionSquareName || s.name == destinationSquareName) {
				s.squareTransform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0f);
			}
		}
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
		BoardPosition boardPosition = new BoardPosition(plyMove, "");
		boardPosition.piecesOnBoard = new string[,] { 
			{"BR","BN","BB","BQ","BK","BB","BN","BR"},
			{"BP","BP","BP","BP","BP","BP","BP","BP"},
			{"--","--","--","--","--","--","--","--"},
			{"--","--","--","--","--","--","--","--"},
			{"--","--","--","--","--","--","--","--"},
			{"--","--","--","--","--","--","--","--"},
			{"WP","WP","WP","WP","WP","WP","WP","WP"},
			{"WR","WN","WB","WQ","WK","WB","WN","WR"},
		};
		boardPositions.Add (boardPosition);

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				// "--" means it's an uninhabited square
				if (boardPositions[plyMove].piecesOnBoard[col,row] != "--") {
					Transform squareTransform = FindPosition (row, col);
					GameObject piecePrefab = FindPrefab (row, col, plyMove);
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
	GameObject FindPrefab(int row, int column, int plyMove)
	{
		foreach (var p in piecesPrefab) {
			if (p.GetComponent<Piece> ().shortName == boardPositions[plyMove].piecesOnBoard [column, row]) {
				return p;
			}
		}
		return null;
	}
	#endregion
}
