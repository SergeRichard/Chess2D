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
		GameDrawn,
		WhiteWon,
		BlackWon
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
			case State.WhiteWon:
				WhiteWon ();
				break;
			case State.BlackWon:
				BlackWon ();
				break;
			case State.GameDrawn:
				GameDrawn ();
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
			state = State.WhiteWon;
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
			state = State.GameDrawn;

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
			state = State.BlackWon;
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
			state = State.GameDrawn;

		} else {
			state = State.WhiteSelectionState;
		}
		DirectToState ();
	}

	void GameDrawn() {
		Debug.Log ("Inside GameDrawn");
	}
	void WhiteWon() {
		Debug.Log ("Inside WhiteWon");
	}
	void BlackWon() {
		Debug.Log ("Inside BlackWon");
	}

	#endregion
	#region Events
	public void SquareClicked(string squareName) {
		Debug.Log (squareName + " clicked!");
		if (!(state == State.BlackWon || state == State.WhiteWon || state == State.GameDrawn)) {
			clickedSquareName = squareName;
			inputState = InputState.SquareIsClicked;
			DirectToState ();
		}
	}
	public void WhiteFlags() {
		state = State.BlackWon;
		DirectToState ();
	}
	public void BlackFlags() {
		state = State.WhiteWon;
		DirectToState ();
	}
	#endregion
	#region helper functions
	void MovePieceToDestination() {
				
		string[,] tempBoard = new string[8, 8];
		string temp = "";
		string textNotation = GetMoveTextNotation();

		// copy all of the board to the tempBoard
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				tempBoard[row,col] = boardPositions[plyMove].piecesOnBoard [row, col];

			}
		}

		// find the selection square and assign an empty space to it
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (selectionSquareName == squareNames [row, col]) {
					temp = tempBoard[row,col];
					tempBoard [row, col] = "--"; 
				}
			}
		}
		// Find the destination square and assign the the temp variable holding the selection piece to the destination square.
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (destinationSquareName == squareNames [row, col]) {
					tempBoard [row, col] = temp; 
				}
			}
		}
		// Modify board if castling
		if (state == State.MoveWhitePieceState) {
			if (WhiteQueenSideCastling ()) {
				tempBoard [7, 0] = "--";
				tempBoard [7, 3] = "WR";

			}
			if (WhiteKingSideCastling ()) {
				tempBoard [7, 7] = "--";
				tempBoard [7, 5] = "WR";
			}
			if (WhiteTakesEnPassant ()) {
				tempBoard [3, BoardPosition.enPassantCol] = "--";
			}
		}
		if (state == State.MoveBlackPieceState) {
			if (BlackQueenSideCastling ()) {
				tempBoard [0, 0] = "--";
				tempBoard [0, 3] = "BR";

			}
			if (BlackKingSideCastling ()) {
				tempBoard [0, 7] = "--";
				tempBoard [0, 5] = "BR";
			}
			if (BlackTakesEnPassant ()) {
				tempBoard [4, BoardPosition.enPassantCol] = "--";
			}

		}
		// create text notation before plyMove++

		//Debug.Log ("Move: " + textNotation);



		plyMove++;
		BoardPosition boardPosition = new BoardPosition (plyMove, textNotation);
		boardPosition.piecesOnBoard = new string[8, 8];

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				boardPosition.piecesOnBoard[row, col] = tempBoard [row, col];
			}
		}
		boardPositions.Add (boardPosition);
		RefreshMoveNotationBoard();
		RemoveAllPieces ();
		UpdateBoard ();
	}
	bool WhiteTakesEnPassant() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectSquare == "WP" && BoardPosition.whiteTakesEnPassantFlag) {
			if (selectRow == 3 && destRow == 2 && (selectCol - 1 == BoardPosition.enPassantCol || selectCol + 1 == BoardPosition.enPassantCol)) {
				return true;
			}
		}
		return false;
	}
	bool BlackTakesEnPassant() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectSquare == "BP" && BoardPosition.blackTakesEnPassantFlag) {
			if (selectRow == 4 && destRow == 5 && (selectCol - 1 == BoardPosition.enPassantCol || selectCol + 1 == BoardPosition.enPassantCol)) {
				return true;
			}
		}
		return false;
	}
	bool WhiteQueenSideCastling() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == 7 && selectCol == 4 && destRow == 7 && destCol == 2 && BoardPosition.whiteQueenSideCastling) {
			if (selectSquare == "WK") {
				return true;
			}
		}

		return false;
	}
	bool WhiteKingSideCastling() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == 7 && selectCol == 4 && destRow == 7 && destCol == 6 && BoardPosition.whiteKingSideCastling) {
			if (selectSquare == "WK") {
				return true;
			}
		}

		return false;
	}
	bool BlackQueenSideCastling() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 2 && BoardPosition.whiteQueenSideCastling) {
			if (selectSquare == "BK") {
				return true;
			}
		}

		return false;

	}
	bool BlackKingSideCastling() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 6 && BoardPosition.whiteKingSideCastling) {
			if (selectSquare == "BK") {
				return true;
			}
		}

		return false;
	}
	string GetMoveTextNotation() {
		string notation = string.Empty;

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (selectionSquareName == squareNames [row, col]) {
					notation += FormatSelectionPartOfNotation (squareNames, row, col);
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
				if (squareNames [row, col] == destinationSquareName) {
					return boardPositions [plyMove].piecesOnBoard [row, col] == "--";
				}
			}

		}
		return false;
	}
	bool PieceIsAttacking() {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (squareNames [row, col] == destinationSquareName) {
					return boardPositions [plyMove].piecesOnBoard [row, col] != "--";
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
	string FormatSelectionPartOfNotation(string[,] squares, int row, int col) {
		string notation = string.Empty;
		string piece = boardPositions [plyMove].piecesOnBoard [row, col];
		string square = squares [row, col];

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
				if (boardPositions[plyMove].piecesOnBoard[row,col] != "--") {
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
		
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == destRow && selectCol == destCol) {
			return false;
		}

		bool isLegal = false;

		switch (selectSquare) {
		case "WR":
			isLegal = WhiteRookMoveLegal ();
			break;
		case "WN":
			isLegal = WhiteKnightMoveLegal ();
			break;
		case "WB":
			isLegal = WhiteBishopMoveLegal ();
			break;
		case "WQ":
			isLegal = WhiteQueenMoveLegal ();
			break;
		case "WK":
			isLegal = WhiteKingMoveLegal ();
			break;
		case "WP":
			isLegal = WhitePawnMoveLegal ();
			break;
		}

		return isLegal;
	}

	void CopyBoardToTemp (out string[,] tempBoard) {
		tempBoard = new string[8,8];
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				tempBoard[row,col] = boardPositions [plyMove].piecesOnBoard [row, col];
			}
		}

	}
	void FindPieceOrSpaceAndLocationOnSquare(string squareName, out string piece, out int row, out int col) {
		col = 0;
		row = 0;
		piece = string.Empty;
		for (int r = 0; r < 8; r++) {
			for (int c = 0; c < 8; c++) {
				if (squareNames[r,c] == squareName) {
					row = r;
					col = c;
					piece = boardPositions [plyMove].piecesOnBoard [r, c];
				}

			}
		}

	}
	bool CheckIfBlacksMoveIsLegal() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectRow == destRow && selectCol == destCol) {
			return false;
		}

		bool isLegal = false;

		switch (selectSquare) {
		case "BR":
			isLegal = BlackRookMoveLegal ();
			break;
		case "BN":
			isLegal = BlackKnightMoveLegal ();
			break;
		case "BB":
			isLegal = BlackBishopMoveLegal ();
			break;
		case "BQ":
			isLegal = BlackQueenMoveLegal ();
			break;
		case "BK":
			isLegal = BlackKingMoveLegal ();
			break;
		case "BP":
			isLegal = BlackPawnMoveLegal ();
			break;
		}

		return isLegal;

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
				string squareName = squareNames [row, col];
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
				if (boardPositions[plyMove].piecesOnBoard[row,col] != "--") {
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
			if (squareNames[row,column] == square.name) {
				return square.squareTransform;
			}

		}
		return null;
	}
	GameObject FindPrefab(int row, int column, int plyMove)
	{
		foreach (var p in piecesPrefab) {
			if (p.GetComponent<Piece> ().shortName == boardPositions[plyMove].piecesOnBoard [row, column]) {
				return p;
			}
		}
		return null;
	}
	#endregion
	#region white legal moves
	#region white rook legal functions
	bool WhiteRookMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhiteRookTest1(selectRow, selectCol, destRow, destCol);

		return test1;
	}
	bool WhiteRookTest1(int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		// if rook wants to move up
		if (selectCol == destCol && selectRow > destRow) {
			for (int row = -1; row + selectRow > destRow - 1; row--) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move down
		if (selectCol == destCol && selectRow < destRow) {
			for (int row = 1; row + selectRow < destRow + 1; row++) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move right
		if (selectRow == destRow && selectCol < destCol) {
			for (int col = 1; col + selectCol < destCol + 1; col++) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move left
		if (selectRow == destRow && selectCol > destCol) {
			for (int col = -1; col + selectCol > destCol - 1; col--) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region white knight legal functions
	bool WhiteKnightMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhiteKnightTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool WhiteKnightTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;
		// Knight wants to move 2 up and 1 left
		if ((selectRow - destRow == 2) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 up and 1 right
		if ((selectRow - destRow == 2) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 up and 2 right
		if ((selectRow - destRow == 1) && (selectCol - destCol == -2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 down and 2 right
		if ((selectRow - destRow == -1) && (selectCol - destCol == -2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 down and 1 right
		if ((selectRow - destRow == -2) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 down and 1 left
		if ((selectRow - destRow == -2) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 down and 2 left
		if ((selectRow - destRow == -1) && (selectCol - destCol == 2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 up and 2 left
		if ((selectRow - destRow == 1) && (selectCol - destCol == 2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	#endregion
	#region white bishop legal functions
	bool WhiteBishopMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhiteBishopTest1 (selectRow, selectCol, destRow, destCol);
		return test1;

	}
	bool WhiteBishopTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		bool rowUp = selectRow - destRow > 0;
		bool colLeft = selectCol - destCol > 0; 

		// Bishop wants to move diagonaly up and left
		if ((selectRow - destRow == selectCol - destCol) && rowUp && colLeft) {
			for (int rowDiag = -1, colDiag = -1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly up and right
		if (selectRow - destRow == (-1 * (selectCol - destCol)) && rowUp && !colLeft) {
			for (int rowDiag = -1, colDiag = 1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly down and right
		if ((selectRow - destRow == selectCol - destCol) && !rowUp && !colLeft) {
			for (int rowDiag = 1, colDiag = 1; selectRow + rowDiag < destRow + 1 ; rowDiag++, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly down and left
		if ((-1 * (selectRow - destRow) == selectCol - destCol) && !rowUp && colLeft) {
			for (int rowDiag = 1, colDiag = -1; selectRow + rowDiag < destRow + 1; rowDiag++, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region white legal move functions
	bool WhiteQueenMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhiteQueenTest1(selectRow, selectCol, destRow, destCol);

		return test1;
	}
	bool WhiteQueenTest1(int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		// if queen wants to move up
		if (selectCol == destCol && selectRow > destRow) {
			for (int row = -1; row + selectRow > destRow - 1; row--) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move down
		if (selectCol == destCol && selectRow < destRow) {
			for (int row = 1; row + selectRow < destRow + 1; row++) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move right
		if (selectRow == destRow && selectCol < destCol) {
			for (int col = 1; col + selectCol < destCol + 1; col++) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move left
		if (selectRow == destRow && selectCol > destCol) {
			for (int col = -1; col + selectCol > destCol - 1; col--) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		bool rowUp = selectRow - destRow > 0;
		bool colLeft = selectCol - destCol > 0; 

		// Queen wants to move diagonaly up and left
		if ((selectRow - destRow == selectCol - destCol) && rowUp && colLeft) {
			for (int rowDiag = -1, colDiag = -1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly up and right
		if (selectRow - destRow == (-1 * (selectCol - destCol)) && rowUp && !colLeft) {
			for (int rowDiag = -1, colDiag = 1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly down and right
		if ((selectRow - destRow == selectCol - destCol) && !rowUp && !colLeft) {
			for (int rowDiag = 1, colDiag = 1; selectRow + rowDiag < destRow + 1 ; rowDiag++, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly down and left
		if ((-1 * (selectRow - destRow) == selectCol - destCol) && !rowUp && colLeft) {
			for (int rowDiag = 1, colDiag = -1; selectRow + rowDiag < destRow + 1; rowDiag++, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region white king legal functions
	bool WhiteKingMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhiteKingTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool WhiteKingTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;
		// King wants to move 1 up and 1 left
		if ((selectRow - destRow == 1) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 up
		if ((selectRow - destRow == 1) && (selectCol - destCol == 0)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 up and 1 right
		if ((selectRow - destRow == 1) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 right
		if ((selectRow - destRow == 0) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 right and 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == 0)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 left and 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 left
		if ((selectRow - destRow == 0) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'B') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to castle king side
		if (selectRow == 7 && selectCol == 4 && destRow == 7 && destCol == 6) {
			if (piecesOnBoard [selectRow, selectCol + 1] == "--" && piecesOnBoard [selectRow, selectCol + 2] == "--" && piecesOnBoard [selectRow, selectCol + 3] == "WR" && BoardPosition.whiteKingSideCastling) {
				return true;
			} else {
				return false;
			}
		}
		// King wants to castle queen side
		if (selectRow == 7 && selectCol == 4 && destRow == 7 && destCol == 2) {
			if (piecesOnBoard [selectRow, selectCol - 1] == "--" && piecesOnBoard [selectRow, selectCol - 2] == "--" 
				&& piecesOnBoard[selectRow, selectCol - 3] == "--" && piecesOnBoard [selectRow, selectCol - 4] == "WR" && BoardPosition.whiteQueenSideCastling) {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	#endregion
	#region White pawn legal functions
	bool WhitePawnMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = WhitePawnTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool WhitePawnTest1 (int selectRow, int selectCol, int destRow, int destCol)
	{
		BoardPosition.blackTakesEnPassantFlag = false;
		if (selectRow == 6 && destRow == 4 && selectCol == destCol) {
			if (boardPositions [plyMove].piecesOnBoard [selectRow - 1, destCol] == "--" && boardPositions [plyMove].piecesOnBoard [selectRow - 2, destCol] == "--") {
				BoardPosition.enPassantCol = destCol;
				BoardPosition.blackTakesEnPassantFlag = true;
				boardPositions [plyMove].blackTakesEnPassantCol = destCol;
				return true;
			} else {
				return false;
			}
				
		}
		if (selectCol == destCol && (selectRow - destRow == 1)) {
			return true;
		}
		if ((destCol == selectCol - 1) && (destRow == selectRow - 1)) {
			if (boardPositions [plyMove].piecesOnBoard [destRow, destCol] [0] == 'B') {
				return true;
			} else if (selectRow == 3 && BoardPosition.whiteTakesEnPassantFlag && destCol == BoardPosition.enPassantCol) {
				return true;
			} else {
				return false;
			}
		}
		if ((destCol == selectCol + 1) && (destRow == selectRow - 1)) {
			if (boardPositions [plyMove].piecesOnBoard [destRow, destCol] [0] == 'B') {
				return true;
			} else if (selectRow == 3 && BoardPosition.whiteTakesEnPassantFlag && destCol == BoardPosition.enPassantCol) {
				return true;
			} else {
				return false;
			}
		}

		// TODO en-passant
		return false;
	}
	#endregion
	#endregion

	#region black legal moves
	#region black rook legal function
	bool BlackRookMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackRookTest1(selectRow, selectCol, destRow, destCol);

		return test1;
	}
	bool BlackRookTest1(int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		// if rook wants to move up
		if (selectCol == destCol && selectRow > destRow) {
			for (int row = -1; row + selectRow > destRow - 1; row--) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a Black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move down
		if (selectCol == destCol && selectRow < destRow) {
			for (int row = 1; row + selectRow < destRow + 1; row++) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move right
		if (selectRow == destRow && selectCol < destCol) {
			for (int col = 1; col + selectCol < destCol + 1; col++) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if rook wants to move left
		if (selectRow == destRow && selectCol > destCol) {
			for (int col = -1; col + selectCol > destCol - 1; col--) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region black knight legal functions
	bool BlackKnightMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackKnightTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool BlackKnightTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;
		// Knight wants to move 2 up and 1 left
		if ((selectRow - destRow == 2) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 up and 1 right
		if ((selectRow - destRow == 2) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 up and 2 right
		if ((selectRow - destRow == 1) && (selectCol - destCol == -2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 down and 2 right
		if ((selectRow - destRow == -1) && (selectCol - destCol == -2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 down and 1 right
		if ((selectRow - destRow == -2) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 2 down and 1 left
		if ((selectRow - destRow == -2) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 down and 2 left
		if ((selectRow - destRow == -1) && (selectCol - destCol == 2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// Knight wants to move 1 up and 2 left
		if ((selectRow - destRow == 1) && (selectCol - destCol == 2)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	#endregion
	#region black bishop legal functions
	bool BlackBishopMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackBishopTest1 (selectRow, selectCol, destRow, destCol);
		return test1;

	}
	bool BlackBishopTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		bool rowUp = selectRow - destRow > 0;
		bool colLeft = selectCol - destCol > 0; 

		// Bishop wants to move diagonaly up and left
		if ((selectRow - destRow == selectCol - destCol) && rowUp && colLeft) {
			for (int rowDiag = -1, colDiag = -1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly up and right
		if (selectRow - destRow == (-1 * (selectCol - destCol)) && rowUp && !colLeft) {
			for (int rowDiag = -1, colDiag = 1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly down and right
		if ((selectRow - destRow == selectCol - destCol) && !rowUp && !colLeft) {
			for (int rowDiag = 1, colDiag = 1; selectRow + rowDiag < destRow + 1 ; rowDiag++, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Bishop wants to move diagonaly down and left
		if ((-1 * (selectRow - destRow) == selectCol - destCol) && !rowUp && colLeft) {
			for (int rowDiag = 1, colDiag = -1; selectRow + rowDiag < destRow + 1; rowDiag++, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region black legal move functions
	bool BlackQueenMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackQueenTest1(selectRow, selectCol, destRow, destCol);

		return test1;
	}
	bool BlackQueenTest1(int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;

		// if queen wants to move up
		if (selectCol == destCol && selectRow > destRow) {
			for (int row = -1; row + selectRow > destRow - 1; row--) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move down
		if (selectCol == destCol && selectRow < destRow) {
			for (int row = 1; row + selectRow < destRow + 1; row++) {
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow + row, destCol] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectRow + row == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move right
		if (selectRow == destRow && selectCol < destCol) {
			for (int col = 1; col + selectCol < destCol + 1; col++) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// if queen wants to move left
		if (selectRow == destRow && selectCol > destCol) {
			for (int col = -1; col + selectCol > destCol - 1; col--) {
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { // if you find a black piece along the way, automatic false;
					return false;
				}
				if (piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { // if you find a white piece along the way, but you are still not at destination then return false else true;
					if (selectCol + col == destCol) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		bool rowUp = selectRow - destRow > 0;
		bool colLeft = selectCol - destCol > 0; 

		// Queen wants to move diagonaly up and left
		if ((selectRow - destRow == selectCol - destCol) && rowUp && colLeft) {
			for (int rowDiag = -1, colDiag = -1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly up and right
		if (selectRow - destRow == (-1 * (selectCol - destCol)) && rowUp && !colLeft) {
			for (int rowDiag = -1, colDiag = 1; selectRow + rowDiag > destRow - 1; rowDiag--, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly down and right
		if ((selectRow - destRow == selectCol - destCol) && !rowUp && !colLeft) {
			for (int rowDiag = 1, colDiag = 1; selectRow + rowDiag < destRow + 1 ; rowDiag++, colDiag++) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		// Queen wants to move diagonaly down and left
		if ((-1 * (selectRow - destRow) == selectCol - destCol) && !rowUp && colLeft) {
			for (int rowDiag = 1, colDiag = -1; selectRow + rowDiag < destRow + 1; rowDiag++, colDiag--) {
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'B') {
					return false;
				}
				if (piecesOnBoard [selectRow + rowDiag, selectCol + colDiag] [0] == 'W') {
					if (selectRow + rowDiag == destRow) {
						return true;
					} else {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	#endregion
	#region black king legal functions
	bool BlackKingMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackKingTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool BlackKingTest1 (int selectRow, int selectCol, int destRow, int destCol) {
		var piecesOnBoard = boardPositions [plyMove].piecesOnBoard;
		// King wants to move 1 up and 1 left
		if ((selectRow - destRow == 1) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 up
		if ((selectRow - destRow == 1) && (selectCol - destCol == 0)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 up and 1 right
		if ((selectRow - destRow == 1) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 right
		if ((selectRow - destRow == 0) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 right and 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == -1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == 0)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 left and 1 down
		if ((selectRow - destRow == -1) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to move 1 left
		if ((selectRow - destRow == 0) && (selectCol - destCol == 1)) {
			if (piecesOnBoard [destRow, destCol] == "--" || piecesOnBoard [destRow, destCol][0] == 'W') {
				return true;
			} else {
				return false;
			}
		}
		// King wants to castle king side
		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 6) {
			if (piecesOnBoard [selectRow, selectCol + 1] == "--" && piecesOnBoard [selectRow, selectCol + 2] == "--" && piecesOnBoard [selectRow, selectCol + 3] == "BR" && BoardPosition.blackKingSideCastling) {
				return true;
			} else {
				return false;
			}
		}
		// King wants to castle queen side
		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 2) {
			if (piecesOnBoard [selectRow, selectCol - 1] == "--" && piecesOnBoard [selectRow, selectCol - 2] == "--" 
				&& piecesOnBoard[selectRow, selectCol - 3] == "--" && piecesOnBoard [selectRow, selectCol - 4] == "BR" && BoardPosition.blackQueenSideCastling) {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
	#endregion
	#region Black pawn legal functions
	bool BlackPawnMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		bool test1 = false;

		test1 = BlackPawnTest1 (selectRow, selectCol, destRow, destCol);
		return test1;
	}
	bool BlackPawnTest1 (int selectRow, int selectCol, int destRow, int destCol)
	{
		BoardPosition.whiteTakesEnPassantFlag = false;
		if (selectRow == 1 && destRow == 3 && selectCol == destCol) {
			if (boardPositions [plyMove].piecesOnBoard [selectRow + 1, destCol] == "--" && boardPositions [plyMove].piecesOnBoard [selectRow + 2, destCol] == "--") {
				BoardPosition.enPassantCol = destCol;
				BoardPosition.whiteTakesEnPassantFlag = true;
				boardPositions [plyMove].whiteTakesEnPassantCol = destCol;
				return true;
			} else {
				return false;
			}

		}
		if (selectCol == destCol && (selectRow - destRow == -1)) {
			return true;
		}
		if ((destCol == selectCol - 1) && (destRow == selectRow + 1)) {
			if (boardPositions [plyMove].piecesOnBoard [destRow, destCol] [0] == 'W') {
				return true;
			} else if (selectRow == 4 && BoardPosition.blackTakesEnPassantFlag && destCol == BoardPosition.enPassantCol) {
				return true;
			} else {
				return false;
			}
		}
		if ((destCol == selectCol + 1) && (destRow == selectRow + 1)) {
			if (boardPositions [plyMove].piecesOnBoard [destRow, destCol] [0] == 'W') {
				return true;
			} else if (selectRow == 4 && BoardPosition.blackTakesEnPassantFlag && destCol == BoardPosition.enPassantCol) {
				return true;
			} else {
				return false;
			}
		}
		// TODO en-passant
		return false;
	}
	#endregion
	#endregion
}
