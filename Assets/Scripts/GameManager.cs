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
		PromoteWhitePawn,
		MoveWhitePieceState,
		WhiteGameAssessmentState,
		BlackSelectionState, 
		BlackDestinationState,
		CheckIfLegalBlackMoveState,
		PromoteBlackPawn,
		MoveBlackPieceState,
		BlackGameAssessmentState,
		Stalemate,
		WhiteMates,
		BlackMates,
		WhiteResigns,
		BlackResigns,
		WhiteFlags,
		BlackFlags
	};
	enum InputState {
		WaitingForClick,
		SquareIsClicked
	}
	public enum HumanPlays {
		White,
		Black
	}
	public static HumanPlays humanPlays = HumanPlays.White;
	public static int minutes = 5;

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
	public Text headerText;
	public GameObject DrawButton;
	public GameObject ResignButton;
	public GameObject NewGameButton;

	public ResultPopupController ResultPopupController;
	private int legalMoveAmount; 

	public PromotionController promotionController;

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
	private string[,] whiteCastlingSquares = new string[,] { 
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","QS","QS","BS","KS","KS","--"}, // QS = Queen side caslting, BS = Both side castling, KS = Kingside castling
	};
	private string[,] blackCastlingSquares = new string[,] { 
		{"--","--","QS","QS","BS","KS","KS","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"},
		{"--","--","--","--","--","--","--","--"}, // QS = Queen side caslting, BS = Both side castling, KS = Kingside castling
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
			case State.PromoteWhitePawn:
				PromoteWhitePawnState ();
				break;
			case State.MoveWhitePieceState:
				MoveWhitePieceState ();
				break;
			case State.WhiteGameAssessmentState:
				WhiteGameAssessmentState ();
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
			case State.PromoteBlackPawn:
				PromoteBlackPawnState ();
				break;
			case State.MoveBlackPieceState:
				MoveBlackPieceState ();
				break;
			case State.BlackGameAssessmentState:
				BlackGameAssessmentState ();
				break;
			case State.WhiteMates:
				WhiteMates ();
				break;
			case State.BlackMates:
				BlackMates ();
				break;
			case State.Stalemate:
				Stalemate ();
				break;
			case State.WhiteFlags:
				WhiteFlagsState ();
				break;
			case State.BlackFlags:
				BlackFlagsState ();
				break;
			case State.WhiteResigns:
				WhiteResigns ();
				break;
			case State.BlackResigns:
				BlackResigns ();
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
		DrawButton.SetActive (true);
		ResignButton.SetActive (true);
		NewGameButton.SetActive (false);
		state = State.WhiteSelectionState;
		clockController.clockState = ClockController.StartClockState.Wait;
		clockController.hoursForWhite = 0;
		clockController.minutesForWhite = minutes;
		clockController.hoursForBlack = 0;
		clockController.minutesForBlack = minutes;
		clockController.ShowStartTimes ();

		headerText.text = humanPlays == HumanPlays.White ? "Human - Computer" : "Computer - Human";

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
		if (legal && state != State.PromoteWhitePawn) {
			state = State.MoveWhitePieceState;

		} else if (state != State.PromoteWhitePawn) {
			Debug.Log ("Illegal move!");

			state = State.WhiteSelectionState;
		}
		UnhighlightSquares ();
		DirectToState ();
	}
	void PromoteWhitePawnState() {
		promotionController.ShowWhitePromotionWindow (true);

	}
	void MoveWhitePieceState() {
		Debug.Log ("Inside MoveWhitePieceState");
		MovePieceToDestination ();
		state = State.WhiteGameAssessmentState;
		clockController.clockState = ClockController.StartClockState.ForBlack;
		DirectToState ();	

	}
	void WhiteGameAssessmentState() {
		Debug.Log ("Inside WhiteGameAssessmentState");
		BoardPosition.kingInCheck = BlackKingInCheck ();
		if (BoardPosition.kingInCheck) {
			boardPositions [plyMove].AddCheckToNotation ();
			RefreshMoveNotationBoard ();
		}

		bool blackHasLegalMovesLeft = BlackHasLegalMovesLeft ();

		var nextState = State.BlackSelectionState;


		if (BoardPosition.kingInCheck && !blackHasLegalMovesLeft) {
			nextState = State.WhiteMates;
		}
		if (!BoardPosition.kingInCheck && !blackHasLegalMovesLeft) {
			nextState = State.Stalemate;
		}
		BoardPosition.kingInCheck = false; // reset check flag
		state = nextState;
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
		if (legal && state != State.PromoteBlackPawn) {
			state = State.MoveBlackPieceState;

		} else if (state != State.PromoteBlackPawn) {
			Debug.Log ("Illegal move!");

			state = State.BlackSelectionState;
		}
		UnhighlightSquares ();
		DirectToState ();

	}
	void PromoteBlackPawnState() {
		Debug.Log ("Inside PromoteBlackPawnState");
		promotionController.ShowBlackPromotionWindow (true);

	}
	void MoveBlackPieceState() {
		Debug.Log ("Inside MoveBlackPieceState");
		MovePieceToDestination ();
		state = State.BlackGameAssessmentState;
		clockController.clockState = ClockController.StartClockState.ForWhite;
		DirectToState ();
	}
	void BlackGameAssessmentState() {
		Debug.Log ("Inside BlackGameAssessmentState");
		BoardPosition.kingInCheck = WhiteKingInCheck ();
		if (BoardPosition.kingInCheck) {
			boardPositions [plyMove].AddCheckToNotation ();
			RefreshMoveNotationBoard ();
		}

		bool whiteHasLegalMovesLeft = WhiteHasLegalMovesLeft ();
		var nextState = State.WhiteSelectionState;
		if (BoardPosition.kingInCheck && !whiteHasLegalMovesLeft) {
			nextState = State.BlackMates;
		}
		if (!BoardPosition.kingInCheck && !whiteHasLegalMovesLeft) {
			nextState = State.Stalemate;
		}
		BoardPosition.kingInCheck = false; // reset check flag

		state = nextState;
		DirectToState ();
	}
	void Stalemate() {
		Debug.Log ("Inside GameDrawn");
		boardPositions [plyMove].AddDrawToNotation ();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		ResultPopupController.ShowStalemateWindow ();
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void WhiteMates() {
		Debug.Log ("Inside WhiteWon");
		boardPositions [plyMove].AddWhiteWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		ResultPopupController.ShowMateWindow ();
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void BlackMates() {
		Debug.Log ("Inside BlackWon");
		boardPositions [plyMove].AddBlackWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		ResultPopupController.ShowMateWindow ();
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void BlackResigns() {
		Debug.Log ("Inside WhiteWon");
		boardPositions [plyMove].AddWhiteWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void WhiteResigns() {
		Debug.Log ("Inside BlackWon");
		boardPositions [plyMove].AddBlackWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void WhiteFlagsState() {
		boardPositions [plyMove].AddBlackWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	void BlackFlagsState() {
		boardPositions [plyMove].AddWhiteWinToNotation();
		RefreshMoveNotationBoard ();
		clockController.clockState = ClockController.StartClockState.GameDone;
		DrawButton.SetActive (false);
		ResignButton.SetActive (false);
		NewGameButton.SetActive (true);
	}
	#endregion
	#region Events
	public void UpdateHumanPlayerColour() {
		//humanPlays = 
	}
	public void SquareClicked(string squareName) {
		Debug.Log (squareName + " clicked!");
		if (ResultPopupController.WindowOpen) {
			ResultPopupController.HideWindow ();
		}
		if (state != State.WhiteResigns && state != State.BlackResigns && state != State.Stalemate && state != State.WhiteMates && state != State.BlackMates) {
			clickedSquareName = squareName;
			inputState = InputState.SquareIsClicked;
			DirectToState ();
		}
	}
	public void OnResignButtonClicked() {
		if (state != State.WhiteResigns && state != State.BlackResigns && state != State.Stalemate && state != State.WhiteMates && state != State.BlackMates) {
			if (state == State.WhiteSelectionState) {
				state = State.WhiteResigns;
			}
			if (state == State.BlackSelectionState) {
				state = State.BlackResigns;
			}
			DirectToState ();
		}

	}
	public void WhiteFlags() {
		state = State.WhiteFlags;
		DirectToState ();
	}
	public void BlackFlags() {
		state = State.BlackFlags;
		DirectToState ();
	}
	public void OnWhitePromotionClicked(string choice) {
		Debug.Log ("You chose " + choice);
		BoardPosition.promotionChoice = choice;
		promotionController.ShowWhitePromotionWindow (false);
		state = State.MoveWhitePieceState;
		DirectToState ();
	}
	public void OnBlackPromotionClicked(string choice) {
		Debug.Log ("You chose " + choice);
		BoardPosition.promotionChoice = choice;
		promotionController.ShowBlackPromotionWindow (false);
		state = State.MoveBlackPieceState;
		DirectToState ();
	}
	#endregion
	#region helper functions
	bool BlackKingInCheck() {

		return !WhiteResponseTest (boardPositions [plyMove].piecesOnBoard);
	}
	bool BlackHasLegalMovesLeft() {
		
		bool anyLegalBlackPawnMoves = AnyLegalBlackPawnMoves ();
		bool anyLegalBlackRookMoves = AnyLegalBlackRookMoves ();
		bool anyLegalBlackKnightMoves = AnyLegalBlackKnightMoves ();
		bool anyLegalBlackBishopMoves = AnyLegalBlackBishopMoves ();
		bool anyLegalBlackQueenMoves = AnyLegalBlackQueenMoves ();
		bool anyLegalBlackKingMoves = AnyLegalBlackKingMoves ();

		return anyLegalBlackPawnMoves || anyLegalBlackKnightMoves || anyLegalBlackRookMoves || anyLegalBlackBishopMoves || anyLegalBlackQueenMoves || anyLegalBlackKingMoves;
	}
	bool AnyLegalBlackPawnMoves() {
		string[,] tempBoard = new string[8, 8];


		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BP") {
					if (selectRow == 1) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] == "--" && boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol] == "--") {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + 2, selectCol] = "BP";
							if (WhiteResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}

					if (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] == "--") {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol] = "BP";
						if (WhiteResponseTest (tempBoard)) {
							return true; // yes, this move is legal, return true
						}
					}
					if (selectCol > 0) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 1] [0] == 'W') {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + 1, selectCol - 1] = "BP";
							if (WhiteResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol < 7) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 1] [0] == 'W') {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + 1, selectCol + 1] = "BP";
							if (WhiteResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol > 0 && selectRow == 4) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] [0] == 'W' && BoardPosition.blackTakesEnPassantFlag && BoardPosition.enPassantCol == selectCol - 1) {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol - 1] = "--";
							tempBoard [selectRow + 1, selectCol - 1] = "BP";
							if (WhiteResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol < 7 && selectRow == 4) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] [0] == 'W' && BoardPosition.blackTakesEnPassantFlag && BoardPosition.enPassantCol == selectCol + 1) {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + 1] = "--";
							tempBoard [selectRow + 1, selectCol + 1] = "BP";
							if (WhiteResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
				}
			}
		}

		return false;
	}
	bool AnyLegalBlackRookMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {

				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BR") {
					// if rook wants to move up
					for (int row = -1; row + selectRow > -1; row--) {
					
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to move down
					for (int row = 1; row + selectRow < 8; row++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves right
					for (int col = 1; col + selectCol < 8; col++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves left
					for (int col = -1; col + selectCol > -1; col--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalBlackKnightMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BN") {
					if (selectRow > 0 && selectCol > 1 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 2] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol - 2] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 1 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol - 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 2, selectCol - 1] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 1 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol + 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 2, selectCol + 1] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 0 && selectCol < 6 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 2] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol + 2] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 7 && selectCol < 6 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 2] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol + 2] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 6 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol + 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 2, selectCol + 1] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 6 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol - 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 2, selectCol - 1] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 7 && selectCol > 1 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 2] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol - 2] = "BN";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
				}
			}
		}

		return false;
	}
	bool AnyLegalBlackBishopMoves() {
		string[,] tempBoard = new string[8, 8];
		int row, col;

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BB") {
					row = selectRow;
					col = selectCol;
					row--;
					col--;
					while (row > -1 && col > -1) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row--;
						col--;

					}
					row = selectRow;
					col = selectCol;
					row--;
					col++;
					while (row > -1 && col < 8) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row--;
						col++;
					}
					row = selectRow;
					col = selectCol;
					row++;
					col++;
					while (row < 8 && col < 8) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row++;
						col++;
					}
					row = selectRow;
					col = selectCol;
					row++;
					col--;
					while (row < 8 && col > -1) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row++;
						col--;
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalBlackQueenMoves() {
		string[,] tempBoard = new string[8, 8];
		int rw, cl;

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BQ") {
					// if rook wants to move up
					for (int row = -1; row + selectRow > -1; row--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to move down
					for (int row = 1; row + selectRow < 8; row++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves right
					for (int col = 1; col + selectCol < 8; col++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves left
					for (int col = -1; col + selectCol > -1; col--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "BR";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					rw = selectRow;
					cl = selectCol;
					rw--;
					cl--;
					while (rw > -1 && cl > -1) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw--;
						cl--;

					}
					rw = selectRow;
					cl = selectCol;
					rw--;
					cl++;
					while (rw > -1 && cl < 8) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw--;
						cl++;
					}
					rw = selectRow;
					cl = selectCol;
					rw++;
					cl++;
					while (rw < 8 && cl < 8) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw++;
						cl++;
					}
					rw = selectRow;
					cl = selectCol;
					rw++;
					cl--;
					while (rw < 8 && cl > -1) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "BB";
							if (WhiteResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 
						}
						rw++;
						cl--;
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalBlackKingMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "BK") {
					// King wants to move 1 up and 1 left
					if (selectRow > 0 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol - 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 up
					if (selectRow > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 up and 1 right
					if (selectRow > 0 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol + 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 right
					if (selectRow > 0 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow, selectCol + 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 right and 1 down
					if (selectRow < 7 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol + 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 down
					if (selectRow < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 left and 1 down
					if (selectRow < 7 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol - 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 left
					if (selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] [0] == 'W')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow, selectCol - 1] = "BK";
						if (WhiteResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow == 0 && selectCol == 4) {
						// King wants to castle king side
						if (boardPositions [plyMove].piecesOnBoard [0, selectCol + 1] == "--" && boardPositions [plyMove].piecesOnBoard [0, selectCol + 2] == "--" && boardPositions [plyMove].piecesOnBoard [0, selectCol + 3] == "BR" && BoardPosition.whiteKingSideCastling) {
							if (WhiteKingSideCastlingTest (selectRow, selectCol, 0, selectCol + 2)) {
								CopyBoardToTemp (out tempBoard);
								tempBoard [0, selectCol] = "--";
								tempBoard [0, selectCol - 4] = "--";
								tempBoard [0, selectCol + 2] = "BK";
								tempBoard [0, selectCol + 1] = "BR";
								if (WhiteResponseTest (tempBoard)) {
									return true; // at least one legal move found, return true
								} 
							}
						}
						// King wants to castle queen side
						if (boardPositions [plyMove].piecesOnBoard [0, selectCol - 1] == "--" && boardPositions [plyMove].piecesOnBoard [0, selectCol - 2] == "--"
						   && boardPositions [plyMove].piecesOnBoard [0, selectCol - 3] == "--" && boardPositions [plyMove].piecesOnBoard [0, selectCol - 4] == "BR" && BoardPosition.whiteQueenSideCastling) {
							if (WhiteQueenSideCastlingTest (0, selectCol, 0, selectCol - 2)) {
								CopyBoardToTemp (out tempBoard);
								tempBoard [0, selectCol] = "--";
								tempBoard [0, selectCol - 4] = "--";
								tempBoard [0, selectCol - 2] = "BK";
								tempBoard [0, selectCol - 1] = "BR";
								if (WhiteResponseTest (tempBoard)) {
									return true; // at least one legal move found, return true
								} 
							}
						}
					}
				}
			}
		}
		return false;
	}
	bool WhiteKingInCheck() {
		return !BlackResponseTest (boardPositions [plyMove].piecesOnBoard);
	}
	bool WhiteHasLegalMovesLeft() {
		bool anyLegalWhitePawnMoves = AnyLegalWhitePawnMoves ();
		bool anyLegalWhiteRookMoves = AnyLegalWhiteRookMoves ();
		bool anyLegalWhiteKnightMoves = AnyLegalWhiteKnightMoves ();
		bool anyLegalWhiteBishopMoves = AnyLegalWhiteBishopMoves ();
		bool anyLegalWhiteQueenMoves = AnyLegalWhiteQueenMoves ();
		bool anyLegalWhiteKingMoves = AnyLegalWhiteKingMoves ();

		return anyLegalWhitePawnMoves || anyLegalWhiteKnightMoves || anyLegalWhiteRookMoves || anyLegalWhiteBishopMoves || anyLegalWhiteQueenMoves || anyLegalWhiteKingMoves;
	}
	bool AnyLegalWhitePawnMoves() {
		// WhitePawnTest2 (int selectRow, int selectCol, int destRow, int destCol)		

		// todo: modify the tempBoard before sending to BlackResponseTest
		string[,] tempBoard = new string[8, 8];


		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WP") {
					if (selectRow == 6) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] == "--" && boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol] == "--") {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow - 2, selectCol] = "WP";
							if (BlackResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}

					if (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] == "--") {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol] = "WP";
						if (BlackResponseTest (tempBoard)) {
							return true; // yes, this move is legal, return true
						}
					}
					if (selectCol > 0) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 1] [0] == 'B') {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow - 1, selectCol - 1] = "WP";
							if (BlackResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol < 7) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 1] [0] == 'B') {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow - 1, selectCol + 1] = "WP";
							if (BlackResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol > 0 && selectRow == 3) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] [0] == 'B' && BoardPosition.blackTakesEnPassantFlag && BoardPosition.enPassantCol == selectCol - 1) {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol - 1] = "--";
							tempBoard [selectRow - 1, selectCol - 1] = "WP";
							if (BlackResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
					if (selectCol < 7 && selectRow == 3) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] [0] == 'B' && BoardPosition.blackTakesEnPassantFlag && BoardPosition.enPassantCol == selectCol + 1) {
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + 1] = "--";
							tempBoard [selectRow - 1, selectCol + 1] = "WP";
							if (BlackResponseTest (tempBoard)) {
								return true; // yes, this move is legal, return true
							}
						}
					}
				}
			}
		}

		return false;
	}
	bool AnyLegalWhiteRookMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WR") {
					// if rook wants to move up
					for (int row = -1; row + selectRow > -1; row--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to move down
					for (int row = 1; row + selectRow < 8; row++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves right
					for (int col = 1; col + selectCol < 8; col++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves left
					for (int col = -1; col + selectCol > -1; col--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalWhiteKnightMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WN") {
					if (selectRow > 0 && selectCol > 1 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 2] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol - 2] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 1 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol - 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 2, selectCol - 1] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 1 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 2, selectCol + 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 2, selectCol + 1] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow > 0 && selectCol < 6 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 2] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol + 2] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 7 && selectCol < 6 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 2] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol + 2] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 6 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol + 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 2, selectCol + 1] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 6 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 2, selectCol - 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 2, selectCol - 1] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow < 7 && selectCol > 1 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 2] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 2] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol - 2] = "WN";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
				}
			}
		}

		return false;
	}
	bool AnyLegalWhiteBishopMoves() {
		string[,] tempBoard = new string[8, 8];
		int row, col;

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WB") {
					row = selectRow;
					col = selectCol;
					row--;
					col--;
					while (row > -1 && col > -1) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row--;
						col--;

					}
					row = selectRow;
					col = selectCol;
					row--;
					col++;
					while (row > -1 && col < 8) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row--;
						col++;
					}
					row = selectRow;
					col = selectCol;
					row++;
					col++;
					while (row < 8 && col < 8) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row++;
						col++;
					}
					row = selectRow;
					col = selectCol;
					row++;
					col--;
					while (row < 8 && col > -1) {
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [row, col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [row, col] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						row++;
						col--;
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalWhiteQueenMoves() {
		string[,] tempBoard = new string[8, 8];
		int rw, cl;

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WQ") {
					// if rook wants to move up
					for (int row = -1; row + selectRow > -1; row--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to move down
					for (int row = 1; row + selectRow < 8; row++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow + row, selectCol] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow + row, selectCol] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves right
					for (int col = 1; col + selectCol < 8; col++) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					// if rook wants to moves left
					for (int col = -1; col + selectCol > -1; col--) {
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + col] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [selectRow, selectCol + col] = "WR";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
					}
					rw = selectRow;
					cl = selectCol;
					rw--;
					cl--;
					while (rw > -1 && cl > -1) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw--;
						cl--;

					}
					rw = selectRow;
					cl = selectCol;
					rw--;
					cl++;
					while (rw > -1 && cl < 8) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw--;
						cl++;
					}
					rw = selectRow;
					cl = selectCol;
					rw++;
					cl++;
					while (rw < 8 && cl < 8) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 

						}
						rw++;
						cl++;
					}
					rw = selectRow;
					cl = selectCol;
					rw++;
					cl--;
					while (rw < 8 && cl > -1) {
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'B') { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} else {
								break;
							}

						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] [0] == 'W') { 							
							break;
						}
						if (boardPositions [plyMove].piecesOnBoard [rw, cl] == "--") { 
							CopyBoardToTemp (out tempBoard);
							tempBoard [selectRow, selectCol] = "--";
							tempBoard [rw, cl] = "WB";
							if (BlackResponseTest (tempBoard)) {
								return true; // at least one legal move found, return true
							} 
						}
						rw++;
						cl--;
					}
				}
			}

		}
		return false;
	}
	bool AnyLegalWhiteKingMoves() {
		string[,] tempBoard = new string[8, 8];

		for (int selectRow = 0; selectRow < 8; selectRow++) {
			for (int selectCol = 0; selectCol < 8; selectCol++) {
				if (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] == "WK") {
					// King wants to move 1 up and 1 left
					if (selectRow > 0 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol - 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 up
					if (selectRow > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 up and 1 right
					if (selectRow > 0 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow - 1, selectCol + 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow - 1, selectCol + 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 right
					if (selectRow > 0 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol + 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow, selectCol + 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 right and 1 down
					if (selectRow < 7 && selectCol < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol + 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol + 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 down
					if (selectRow < 7 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 left and 1 down
					if (selectRow < 7 && selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow + 1, selectCol - 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow + 1, selectCol - 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					// King wants to move 1 left
					if (selectCol > 0 && (boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] == "--" || boardPositions [plyMove].piecesOnBoard [selectRow, selectCol - 1] [0] == 'B')) {
						CopyBoardToTemp (out tempBoard);
						tempBoard [selectRow, selectCol] = "--";
						tempBoard [selectRow, selectCol - 1] = "WK";
						if (BlackResponseTest (tempBoard)) {
							return true; // at least one legal move found, return true
						} 
					}
					if (selectRow == 7 && selectCol == 4) {
						// King wants to castle king side
						if (boardPositions [plyMove].piecesOnBoard [7, selectCol + 1] == "--" && boardPositions [plyMove].piecesOnBoard [7, selectCol + 2] == "--" && boardPositions [plyMove].piecesOnBoard [7, selectCol + 3] == "WR" && BoardPosition.whiteKingSideCastling) {
							if (WhiteKingSideCastlingTest (7, selectCol, 7, selectCol + 2)) {
								CopyBoardToTemp (out tempBoard);
								tempBoard [7, selectCol] = "--";
								tempBoard [7, selectCol - 4] = "--";
								tempBoard [7, selectCol + 2] = "WK";
								tempBoard [7, selectCol + 1] = "WR";
								if (BlackResponseTest (tempBoard)) {
									return true; // at least one legal move found, return true
								} 
							}
						}
						// King wants to castle queen side
						if (boardPositions [plyMove].piecesOnBoard [7, selectCol - 1] == "--" && boardPositions [plyMove].piecesOnBoard [7, selectCol - 2] == "--"
						   && boardPositions [plyMove].piecesOnBoard [7, selectCol - 3] == "--" && boardPositions [plyMove].piecesOnBoard [7, selectCol - 4] == "WR" && BoardPosition.whiteQueenSideCastling) {
							if (WhiteQueenSideCastlingTest (7, selectCol, 7, selectCol - 2)) {
								CopyBoardToTemp (out tempBoard);
								tempBoard [7, selectCol] = "--";
								tempBoard [7, selectCol - 4] = "--";
								tempBoard [7, selectCol - 2] = "WK";
								tempBoard [7, selectCol - 1] = "WR";
								if (BlackResponseTest (tempBoard)) {
									return true; // at least one legal move found, return true
								} 
							}
						}

					}
				}
			}
		}
		return false;
	}
	void MovePieceToDestination() {

		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);


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
				BoardPosition.whiteQueenSideCastling = false;
			}
			if (WhiteKingSideCastling ()) {
				tempBoard [7, 7] = "--";
				tempBoard [7, 5] = "WR";
				BoardPosition.whiteKingSideCastling = false;
			}
			if (selectSquare == "WK") {
				BoardPosition.whiteKingSideCastling = false;
				BoardPosition.whiteQueenSideCastling = false;
			}
			if (selectSquare == "WR" && selectRow == 7 && selectCol == 0) {
				BoardPosition.whiteQueenSideCastling = false;
			}
			if (selectSquare == "WR" && selectRow == 7 && selectCol == 7) {
				BoardPosition.whiteKingSideCastling = false;
			}
			if (WhiteTakesEnPassant ()) {
				tempBoard [3, BoardPosition.enPassantCol] = "--";
			}
			if (WhitePawnPromotes ()) {
				tempBoard [selectRow, selectCol] = "--";
				tempBoard [destRow, destCol] = BoardPosition.promotionChoice;
			}
		}
		if (state == State.MoveBlackPieceState) {
			if (BlackQueenSideCastling ()) {
				tempBoard [0, 0] = "--";
				tempBoard [0, 3] = "BR";
				BoardPosition.blackQueenSideCastling = false;
			}
			if (BlackKingSideCastling ()) {
				tempBoard [0, 7] = "--";
				tempBoard [0, 5] = "BR";
				BoardPosition.blackKingSideCastling = false;
			}
			if (selectSquare == "BK") {
				BoardPosition.blackKingSideCastling = false;
				BoardPosition.blackQueenSideCastling = false;
			}
			if (selectSquare == "BR" && selectRow == 0 && selectCol == 0) {
				BoardPosition.blackQueenSideCastling = false;
			}
			if (selectSquare == "BR" && selectRow == 0 && selectCol == 7) {
				BoardPosition.blackKingSideCastling = false;
			}
			if (BlackTakesEnPassant ()) {
				tempBoard [4, BoardPosition.enPassantCol] = "--";
			}
			if (BlackPawnPromotes ()) {
				tempBoard [selectRow, selectCol] = "--";
				tempBoard [destRow, destCol] = BoardPosition.promotionChoice;
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
	bool WhitePawnPromotes() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectSquare == "WP" && destRow == 0) {
			return true;
		}
		return false;
	}
	bool BlackPawnPromotes() {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		if (selectSquare == "BP" && destRow == 7) {
			return true;
		}
		return false;
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

		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 2 && BoardPosition.blackQueenSideCastling) {
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

		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 6 && BoardPosition.blackKingSideCastling) {
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
		if (BoardPosition.kingInCheck) {
			notation += "+";
		}
		if (state == State.MoveWhitePieceState) {
			if (WhiteQueenSideCastling ()) {
				notation = "O-O-O";
			}
			if (WhiteKingSideCastling ()) {
				notation = "O-O";
			}
		}
		if (state == State.MoveBlackPieceState) {
			if (BlackQueenSideCastling ()) {
				notation = "O-O-O";
			}
			if (BlackKingSideCastling ()) {
				notation = "O-O";
			}
		}
		if (WhitePawnPromotes () || BlackPawnPromotes()) {
			notation += BoardPosition.promotionChoice [1];
		}

		return notation;
	}
	bool PieceIsMoving() {
		if (state == State.MoveWhitePieceState && WhiteTakesEnPassant ()) {
			return false;
		}
		if (state == State.MoveBlackPieceState && BlackTakesEnPassant ()) {
			return false;
		}
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
		if (state == State.MoveWhitePieceState && WhiteTakesEnPassant ()) {
			return true;
		}
		if (state == State.MoveBlackPieceState && BlackTakesEnPassant ()) {
			return true;
		}
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
			if (isLegal && destRow == 0) {
				state = State.PromoteWhitePawn;
			}
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
			if (isLegal && destRow == 7) {
				state = State.PromoteBlackPawn;
			}
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


		bool test1 = WhiteRookTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = WhiteRookTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

		return true;
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
	bool WhiteRookTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		// Even if pawn promotes, doesn't change whether or not white king will or will not be captured by pawn move.
		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WR";
		return BlackResponseTest (tempBoard);

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

		bool test1 = WhiteKnightTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = WhiteKnightTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

		return true;
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
	bool WhiteKnightTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WN";
		return BlackResponseTest (tempBoard);

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

		bool test1 = WhiteBishopTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = WhiteBishopTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		return true;

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
	bool WhiteBishopTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WB";
		return BlackResponseTest (tempBoard);

	}
	#endregion
	#region white queen legal move functions
	bool WhiteQueenMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		bool test1 = WhiteQueenTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = WhiteQueenTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		return true;
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
	bool WhiteQueenTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WQ";
		return BlackResponseTest (tempBoard);

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

		bool test1 = WhiteKingTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = WhiteKingTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

		return true;
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
				return WhiteKingSideCastlingTest(selectRow, selectCol, destRow, destCol);
			} else {
				return false;
			}
		}
		// King wants to castle queen side
		if (selectRow == 7 && selectCol == 4 && destRow == 7 && destCol == 2) {
			if (piecesOnBoard [selectRow, selectCol - 1] == "--" && piecesOnBoard [selectRow, selectCol - 2] == "--" 
				&& piecesOnBoard[selectRow, selectCol - 3] == "--" && piecesOnBoard [selectRow, selectCol - 4] == "WR" && BoardPosition.whiteQueenSideCastling) {
				return WhiteQueenSideCastlingTest(selectRow, selectCol, destRow, destCol);
			} else {
				return false;
			}
		}
		return false;
	}
	bool WhiteKingSideCastlingTest(int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WK";

		bool kingSideCastling = true;


		bool test1 = !BlackPawnPreventsCastling (kingSideCastling, tempBoard);
		bool test2 = !BlackRookPreventsCastling (kingSideCastling, tempBoard);
		bool test3 = !BlackKnightPreventsCastling (kingSideCastling, tempBoard);
		bool test4 = !BlackBishopPreventsCastling (kingSideCastling, tempBoard);
		bool test5 = !BlackQueenPreventsCastling (kingSideCastling, tempBoard);
		bool test6 = !BlackKingPreventsCastling (kingSideCastling, tempBoard);

		return test1 && test2 && test3 && test4 && test5 && test6;
	}
	bool WhiteQueenSideCastlingTest(int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WK";

		bool kingSideCastling = false;

		bool test1 = !BlackPawnPreventsCastling (kingSideCastling, tempBoard);
		bool test2 = !BlackRookPreventsCastling (kingSideCastling, tempBoard);
		bool test3 = !BlackKnightPreventsCastling (kingSideCastling, tempBoard);
		bool test4 = !BlackBishopPreventsCastling (kingSideCastling, tempBoard);
		bool test5 = !BlackQueenPreventsCastling (kingSideCastling, tempBoard);
		bool test6 = !BlackKingPreventsCastling (kingSideCastling, tempBoard);

		return test1 && test2 && test3 && test4 && test5 && test6;
	}
	bool BlackPawnPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BP") {
					if (kingSideCastling) {
						if (col > 0 && (whiteCastlingSquares [row + 1, col - 1] == "KS" || whiteCastlingSquares [row + 1, col - 1] == "BS")) {
							return true;
						}
						if (col < 7 && (whiteCastlingSquares [row + 1, col + 1] == "KS" || whiteCastlingSquares [row + 1, col + 1] == "BS")) {
							return true;
						}
					} else {
						if (col > 0 && (whiteCastlingSquares [row + 1, col - 1] == "QS" || whiteCastlingSquares [row + 1, col - 1] == "BS")) {
							return true;
						}
						if (col < 7 && (whiteCastlingSquares [row + 1, col + 1] == "QS" || whiteCastlingSquares [row + 1, col + 1] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool BlackRookPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BR") {
					if (BlackRookCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool BlackRookCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {
		
		// search down
		for (int row = initialRow + 1; row < 8; row++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, initialCol] == "KS" || whiteCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, initialCol] == "QS" || whiteCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, initialCol][0]== 'B' || tempBoard [row, initialCol][0] == 'W') {
				break;
			}
		}
		// search left
		for (int col = initialCol - 1; col > -1; col++) {
			
			if (kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "KS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "QS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		// search right
		for (int col = initialCol + 1; col < 8; col++) {
			
			if (kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "KS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "QS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		return false;
	}
	bool BlackKnightPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BN") {
					if (kingSideCastling) {
						if (row + 1 < 8 && col - 2 > -1 && (whiteCastlingSquares [row + 1, col - 2] == "KS" || whiteCastlingSquares [row + 1, col - 2] == "BS")) {
							return true;
						}
						if (row + 2 < 8 && col - 1 > -1 && (whiteCastlingSquares [row + 2, col - 1] == "KS" || whiteCastlingSquares [row + 2, col - 1] == "BS")) {
							return true;
						}
						if (row + 2 < 8 && col + 1 < 8 && (whiteCastlingSquares [row + 2, col + 1] == "KS" || whiteCastlingSquares [row + 2, col + 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col + 2 < 8 && (whiteCastlingSquares [row + 1, col + 2] == "KS" || whiteCastlingSquares [row + 1, col + 2] == "BS")) {
							return true;
						}
					} else {
						if (row + 1 < 8 && col - 2 > -1 && (whiteCastlingSquares [row + 1, col - 2] == "QS" || whiteCastlingSquares [row + 1, col - 2] == "BS")) {
							return true;
						}
						if (row + 2 < 8 && col - 1 > -1 && (whiteCastlingSquares [row + 2, col - 1] == "QS" || whiteCastlingSquares [row + 2, col - 1] == "BS")) {
							return true;
						}
						if (row + 2 < 8 && col + 1 < 8 && (whiteCastlingSquares [row + 2, col + 1] == "QS" || whiteCastlingSquares [row + 2, col + 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col + 2 < 8 && (whiteCastlingSquares [row + 1, col + 2] == "QS" || whiteCastlingSquares [row + 1, col + 2] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool BlackBishopPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BB") {
					if (BlackBishopCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool BlackBishopCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {

		// search digaonaly bottom right
		for (int row = initialRow + 1, col = initialCol + 1; row < 8 && col < 8; row++, col++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "KS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "QS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search digaonaly bottom left
		for (int row = initialRow + 1, col = initialCol - 1; row < 8 && col > 0; row++, col--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "KS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "QS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}

		return false;
	}

	bool BlackQueenPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BQ") {
					if (BlackQueenCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool BlackQueenCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {

		// search digaonaly bottom right
		for (int row = initialRow + 1, col = initialCol + 1; row < 8 && col < 8; row++, col++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "KS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "QS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search digaonaly bottom left
		for (int row = initialRow + 1, col = initialCol - 1; row < 8 && col > 0; row++, col--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "KS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, col] == "QS" || whiteCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search down
		for (int row = initialRow + 1; row < 8; row++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (whiteCastlingSquares [row, initialCol] == "KS" || whiteCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [row, initialCol] == "QS" || whiteCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, initialCol][0]== 'B' || tempBoard [row, initialCol][0] == 'W') {
				break;
			}
		}
		// search left
		for (int col = initialCol - 1; col > -1; col++) {

			if (kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "KS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "QS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		// search right
		for (int col = initialCol + 1; col < 8; col++) {

			if (kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "KS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (whiteCastlingSquares [initialRow, col] == "QS" || whiteCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		return false;
	}
	bool BlackKingPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BK") {
					if (kingSideCastling) {
						if (col - 1 > -1 && (whiteCastlingSquares [row, col - 1] == "KS" || whiteCastlingSquares [row, col - 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col - 1 > -1 && (whiteCastlingSquares [row + 1, col - 1] == "KS" || whiteCastlingSquares [row + 1, col - 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && (whiteCastlingSquares [row + 1, col] == "KS" || whiteCastlingSquares [row + 1, col] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col + 1 < 8 && (whiteCastlingSquares [row + 1, col + 1] == "KS" || whiteCastlingSquares [row + 1, col + 1] == "BS")) {
							return true;
						}
						if (col + 1 < 8 && (whiteCastlingSquares [row, col + 1] == "KS" || whiteCastlingSquares [row, col + 1] == "BS")) {
							return true;
						}
					} else {
						if (col - 1 > -1 && (whiteCastlingSquares [row, col - 1] == "QS" || whiteCastlingSquares [row, col - 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col - 1 > -1 && (whiteCastlingSquares [row + 1, col - 1] == "QS" || whiteCastlingSquares [row + 1, col - 1] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && (whiteCastlingSquares [row + 1, col] == "QS" || whiteCastlingSquares [row + 1, col] == "BS")) {
							return true;
						}
						if (row + 1 < 8 && col + 1 < 8 && (whiteCastlingSquares [row + 1, col + 1] == "QS" || whiteCastlingSquares [row + 1, col + 1] == "BS")) {
							return true;
						}
						if (col + 1 < 8 && (whiteCastlingSquares [row, col + 1] == "QS" || whiteCastlingSquares [row, col + 1] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool WhiteKingTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WK";
		return BlackResponseTest (tempBoard);

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



		bool test1 = WhitePawnTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;
		
		bool test2 = WhitePawnTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		return true;
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
	bool WhitePawnTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		// Even if pawn promotes, doesn't change whether or not white king will or will not be captured by pawn move.
		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "WP";
		return BlackResponseTest (tempBoard);

	}

	#endregion
	#region Black response
	bool BlackResponseTest (string[,] tempBoard) {
		if (BlackPawnAttacksKing (tempBoard)) {
			return false;
		}
		if (BlackRookAttacksKing (tempBoard)) {
			return false;
		}
		if (BlackKnightAttacksKing (tempBoard)) {
			return false;
		}
		if (BlackBishopAttacksKing (tempBoard)) {
			return false;
		}
		if (BlackQueenAttacksKing (tempBoard)) {
			return false;
		}
		if (BlackKingAttacksKing (tempBoard)) {
			return false;
		}
		return true;
	}
	bool BlackPawnAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BP") {
					if (col < 7 && tempBoard [row + 1, col + 1] == "WK") {
						return true;
					} 
					if (col > 0 && tempBoard [row + 1, col - 1] == "WK") {
						return true;
					} 
				}

			}
		}
		return false;
	}
	bool BlackRookAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BR") {
					if (BlackRookMovesRightFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackRookMovesLeftFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackRookMovesUpFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackRookMovesDownFindsKing(tempBoard, row, col)) {
						return true;
					}
				}
			}
		}
		return false;
	}
	bool BlackKnightAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BN") {
					if (row > 0 && col > 1 && tempBoard [row - 1, col - 2] == "WK") {
						return true;
					}
					if (row > 1 && col > 0 && tempBoard [row - 2, col - 1] == "WK") {
						return true;
					}
					if (row > 1 && col < 7 && tempBoard [row - 2, col + 1] == "WK") {
						return true;
					}
					if (row > 0 && col < 6 && tempBoard [row - 1, col + 2] == "WK") {
						return true;
					}
					if (row < 7 && col < 6 && tempBoard [row + 1, col + 2] == "WK") {
						return true;
					}
					if (row < 6 && col < 7 && tempBoard [row + 2, col + 1] == "WK") {
						return true;
					}
					if (row < 6 && col > 0 && tempBoard [row + 2, col - 1] == "WK") {
						return true;
					}
					if (row < 7 && col > 1 && tempBoard [row + 1, col - 2] == "WK") {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool BlackBishopAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BB") {
					if (BlackBishopMovesDiagTopLeft (tempBoard, row, col)) {
						return true;
					}
					if (BlackBishopMovesDiagTopRight (tempBoard, row, col)) {
						return true;
					}
					if (BlackBishopMovesDiagBottomLeft (tempBoard, row, col)) {
						return true;
					}
					if (BlackBishopMovesDiagBottomRight (tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool BlackQueenAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BQ") {
					if (BlackQueenMovesDiagTopLeft (tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesDiagTopRight (tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesDiagBottomLeft (tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesDiagBottomRight (tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesRightFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesLeftFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesUpFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (BlackQueenMovesDownFindsKing(tempBoard, row, col)) {
						return true;
					}
				}
			}
		}
		return false;
	}
	bool BlackKingAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "BK") {
					if (col > 0 && tempBoard [row, col - 1] == "WK") {
						return true;
					}
					if (row > 0 && col > 0 && tempBoard [row - 1, col - 1] == "WK") {
						return true;
					}
					if (row > 0 && tempBoard [row - 1, col] == "WK") {
						return true;
					}
					if (row > 0 && col < 7 && tempBoard [row - 1, col + 1] == "WK") {
						return true;
					}
					if (col < 7 && tempBoard [row, col + 1] == "WK") {
						return true;
					}
					if (row < 7 && col < 7 && tempBoard [row + 1, col + 1] == "WK") {
						return true;
					}
					if (row < 7 && tempBoard [row + 1, col] == "WK") {
						return true;
					}
					if (row < 7 && col > 0 && tempBoard [row + 1, col - 1] == "WK") {
						return true;
					}

				}
			}
		}
		return false;

	}
	bool BlackBishopMovesDiagTopLeft (string[,] tempBoard, int row, int col) {
		row--;
		col--;
		while (row > -1 && col > -1) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col--;
		}
		return false;
	}
	bool BlackBishopMovesDiagTopRight (string[,] tempBoard, int row, int col) {
		row--;
		col++;
		while (row > -1 && col < 8) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col++;
		}
		return false;
	}
	bool BlackBishopMovesDiagBottomRight (string[,] tempBoard, int row, int col) {
		row++;
		col++;
		while (row < 8 && col < 8) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col++;
		}
		return false;
	}
	bool BlackBishopMovesDiagBottomLeft (string[,] tempBoard, int row, int col) {
		row++;
		col--;
		while (row < 8 && col > -1) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col--;
		}
		return false;
	}
	bool BlackRookMovesRightFindsKing (string[,] tempBoard, int row, int col) {
		col++;
		while (col < 8) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col++;
		}
		return false;
	}
	bool BlackRookMovesLeftFindsKing (string[,] tempBoard, int row, int col) {
		col--;
		while (col > -1) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col--;
		}
		return false;
	}
	bool BlackRookMovesUpFindsKing (string[,] tempBoard, int row, int col) {
		row--;
		while (row > -1) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row--;
		}
		return false;
	}
	bool BlackRookMovesDownFindsKing (string[,] tempBoard, int row, int col) {
		row++;
		while (row < 8) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row++;
		}
		return false;

	}
	bool BlackQueenMovesDiagTopLeft (string[,] tempBoard, int row, int col) {
		row--;
		col--;
		while (row > -1 && col > -1) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col--;
		}
		return false;
	}
	bool BlackQueenMovesDiagTopRight (string[,] tempBoard, int row, int col) {
		row--;
		col++;
		while (row > -1 && col < 8) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col++;
		}
		return false;
	}
	bool BlackQueenMovesDiagBottomRight (string[,] tempBoard, int row, int col) {
		row++;
		col++;
		while (row < 8 && col < 8) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col++;
		}
		return false;
	}
	bool BlackQueenMovesDiagBottomLeft (string[,] tempBoard, int row, int col) {
		row++;
		col--;
		while (row < 8 && col > -1) {
			if (tempBoard [row, col] == "WK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col--;
		}
		return false;
	}
	bool BlackQueenMovesRightFindsKing (string[,] tempBoard, int row, int col) {
		col++;
		while (col < 8) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col++;
		}
		return false;
	}
	bool BlackQueenMovesLeftFindsKing (string[,] tempBoard, int row, int col) {
		col--;
		while (col > -1) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col--;
		}
		return false;
	}
	bool BlackQueenMovesUpFindsKing (string[,] tempBoard, int row, int col) {
		row--;
		while (row > -1) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row--;
		}
		return false;
	}
	bool BlackQueenMovesDownFindsKing (string[,] tempBoard, int row, int col) {
		row++;
		while (row < 8) {
			if (tempBoard [row, col] == "WK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row++;
		}
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

		bool test1 = BlackRookTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackRookTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		return true;
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
	bool BlackRookTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BR";
		return WhiteResponseTest (tempBoard);

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

		bool test1 = BlackKnightTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackKnightTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		return true;
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
	bool BlackKnightTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BN";
		return WhiteResponseTest (tempBoard);

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

		bool test1 = BlackBishopTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackBishopTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;
		
		return true;
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
	bool BlackBishopTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BB";
		return WhiteResponseTest (tempBoard);

	}
	#endregion
	#region black queen legal move functions
	bool BlackQueenMoveLegal () {
		string selectSquare;
		int selectCol, selectRow;
		FindPieceOrSpaceAndLocationOnSquare(selectionSquareName, out selectSquare, out selectRow, out selectCol);

		string destArea;
		int destCol, destRow;
		FindPieceOrSpaceAndLocationOnSquare(destinationSquareName, out destArea, out destRow, out destCol);

		bool test1 = BlackQueenTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackQueenTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

		return true;
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
	bool BlackQueenTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BQ";
		return WhiteResponseTest (tempBoard);

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

		bool test1 = BlackKingTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackKingTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

		return true;
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
				return BlackKingSideCastlingTest(selectRow, selectCol, destRow, destCol);
			} else {
				return false;
			}
		}
		// King wants to castle queen side
		if (selectRow == 0 && selectCol == 4 && destRow == 0 && destCol == 2) {
			if (piecesOnBoard [selectRow, selectCol - 1] == "--" && piecesOnBoard [selectRow, selectCol - 2] == "--" 
				&& piecesOnBoard[selectRow, selectCol - 3] == "--" && piecesOnBoard [selectRow, selectCol - 4] == "BR" && BoardPosition.blackQueenSideCastling) {
				return BlackQueenSideCastlingTest(selectRow, selectCol, destRow, destCol);
			} else {
				return false;
			}
		}
		return false;
	}
	bool BlackKingSideCastlingTest(int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BK";

		bool kingSideCastling = true;


		bool test1 = !WhitePawnPreventsCastling (kingSideCastling, tempBoard);
		bool test2 = !WhiteRookPreventsCastling (kingSideCastling, tempBoard);
		bool test3 = !WhiteKnightPreventsCastling (kingSideCastling, tempBoard);
		bool test4 = !WhiteBishopPreventsCastling (kingSideCastling, tempBoard);
		bool test5 = !WhiteQueenPreventsCastling (kingSideCastling, tempBoard);
		bool test6 = !WhiteKingPreventsCastling (kingSideCastling, tempBoard);

		return test1 && test2 && test3 && test4 && test5 && test6;
	}
	bool BlackQueenSideCastlingTest(int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BK";

		bool kingSideCastling = false;

		bool test1 = !WhitePawnPreventsCastling (kingSideCastling, tempBoard);
		bool test2 = !WhiteRookPreventsCastling (kingSideCastling, tempBoard);
		bool test3 = !WhiteKnightPreventsCastling (kingSideCastling, tempBoard);
		bool test4 = !WhiteBishopPreventsCastling (kingSideCastling, tempBoard);
		bool test5 = !WhiteQueenPreventsCastling (kingSideCastling, tempBoard);
		bool test6 = !WhiteKingPreventsCastling (kingSideCastling, tempBoard);

		return test1 && test2 && test3 && test4 && test5 && test6;
	}
	bool WhitePawnPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WP") {
					if (kingSideCastling) {
						if (col > 0 && (blackCastlingSquares [row - 1, col - 1] == "KS" || blackCastlingSquares [row - 1, col - 1] == "BS")) {
							return true;
						}
						if (col < 7 && (blackCastlingSquares [row - 1, col + 1] == "KS" || blackCastlingSquares [row - 1, col + 1] == "BS")) {
							return true;
						}
					} else {
						if (col > 0 && (blackCastlingSquares [row - 1, col - 1] == "QS" || blackCastlingSquares [row - 1, col - 1] == "BS")) {
							return true;
						}
						if (col < 7 && (blackCastlingSquares [row - 1, col + 1] == "QS" || blackCastlingSquares [row - 1, col + 1] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool WhiteRookPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WR") {
					if (WhiteRookCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool WhiteRookCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {

		// search up
		for (int row = initialRow - 1; row > -1; row--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, initialCol] == "KS" || blackCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, initialCol] == "QS" || blackCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, initialCol][0]== 'B' || tempBoard [row, initialCol][0] == 'W') {
				break;
			}
		}
		// search left
		for (int col = initialCol - 1; col > -1; col++) {

			if (kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "KS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "QS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		// search right
		for (int col = initialCol + 1; col < 8; col++) {

			if (kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "KS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "QS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		return false;
	}
	bool WhiteKnightPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WN") {
					if (kingSideCastling) {
						if (row - 1 > -1 && col - 2 > -1 && (blackCastlingSquares [row - 1, col - 2] == "KS" || blackCastlingSquares [row - 1, col - 2] == "BS")) {
							return true;
						}
						if (row - 2 > -1 && col - 1 > -1 && (blackCastlingSquares [row - 2, col - 1] == "KS" || blackCastlingSquares [row - 2, col - 1] == "BS")) {
							return true;
						}
						if (row - 2 > -1 && col + 1 < 8 && (blackCastlingSquares [row - 2, col + 1] == "KS" || blackCastlingSquares [row - 2, col + 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col + 2 < 8 && (blackCastlingSquares [row - 1, col + 2] == "KS" || blackCastlingSquares [row - 1, col + 2] == "BS")) {
							return true;
						}
					} else {
						if (row - 1 > -1 && col - 2 > -1 && (blackCastlingSquares [row - 1, col - 2] == "QS" || blackCastlingSquares [row - 1, col - 2] == "BS")) {
							return true;
						}
						if (row - 2 > -1 && col - 1 > -1 && (blackCastlingSquares [row - 2, col - 1] == "QS" || blackCastlingSquares [row - 2, col - 1] == "BS")) {
							return true;
						}
						if (row - 2 > -1 && col + 1 < 8 && (blackCastlingSquares [row - 2, col + 1] == "QS" || blackCastlingSquares [row - 2, col + 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col + 2 < 8 && (blackCastlingSquares [row - 1, col + 2] == "QS" || blackCastlingSquares [row - 1, col + 2] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool WhiteBishopPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WB") {
					if (WhiteBishopCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool WhiteBishopCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {

		// search digaonaly top right
		for (int row = initialRow - 1, col = initialCol + 1; row > -1 && col < 8; row--, col++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, col] == "KS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, col] == "QS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search digaonaly top left
		for (int row = initialRow - 1, col = initialCol - 1; row > -1 && col > 0; row--, col--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, col] == "KS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, col] == "QS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}

		return false;
	}

	bool WhiteQueenPreventsCastling (bool kingSideCastling, string[,] tempBoard) {

		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WQ") {
					if (WhiteQueenCastlingSquaresSearch (kingSideCastling, tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool WhiteQueenCastlingSquaresSearch(bool kingSideCastling, string[,] tempBoard, int initialRow, int initialCol) {

		// search digaonaly top right
		for (int row = initialRow - 1, col = initialCol + 1; row > -1 && col < 8; row--, col++) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, col] == "KS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, col] == "QS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search digaonaly top left
		for (int row = initialRow - 1, col = initialCol - 1; row > -1 && col > 0; row--, col--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, col] == "KS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, col] == "QS" || blackCastlingSquares [row, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, col][0]== 'B' || tempBoard [row, col][0] == 'W') {
				break;
			}
		}
		// search up
		for (int row = initialRow - 1; row > -1; row--) {		
			// Order is important here. There has to be a caslting square check. If found return true, else proceed to checking if black rook can move any further. If not, return false
			// for not finding a castling square.
			if (kingSideCastling) {
				if (blackCastlingSquares [row, initialCol] == "KS" || blackCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [row, initialCol] == "QS" || blackCastlingSquares [row, initialCol] == "BS") {
					return true;
				}
			}
			if (tempBoard [row, initialCol][0]== 'B' || tempBoard [row, initialCol][0] == 'W') {
				break;
			}
		}
		// search left
		for (int col = initialCol - 1; col > -1; col++) {

			if (kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "KS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "QS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		// search right
		for (int col = initialCol + 1; col < 8; col++) {

			if (kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "KS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (!kingSideCastling) {
				if (blackCastlingSquares [initialRow, col] == "QS" || blackCastlingSquares [initialRow, col] == "BS") {
					return true;
				}
			}
			if (tempBoard [initialRow, col][0]== 'B' || tempBoard [initialRow, col][0] == 'W') {
				break;
			}
		}
		return false;
	}
	bool WhiteKingPreventsCastling (bool kingSideCastling, string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WK") {
					if (kingSideCastling) {
						if (col - 1 > -1 && (blackCastlingSquares [row, col - 1] == "KS" || blackCastlingSquares [row, col - 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col - 1 > -1 && (blackCastlingSquares [row - 1, col - 1] == "KS" || blackCastlingSquares [row - 1, col - 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && (blackCastlingSquares [row - 1, col] == "KS" || blackCastlingSquares [row - 1, col] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col + 1 < 8 && (blackCastlingSquares [row - 1, col + 1] == "KS" || blackCastlingSquares [row - 1, col + 1] == "BS")) {
							return true;
						}
						if (col + 1 < 8 && (blackCastlingSquares [row, col + 1] == "KS" || blackCastlingSquares [row, col + 1] == "BS")) {
							return true;
						}
					} else {
						if (col - 1 > -1 && (blackCastlingSquares [row, col - 1] == "QS" || blackCastlingSquares [row, col - 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col - 1 > -1 && (blackCastlingSquares [row - 1, col - 1] == "QS" || blackCastlingSquares [row - 1, col - 1] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && (blackCastlingSquares [row - 1, col] == "QS" || blackCastlingSquares [row - 1, col] == "BS")) {
							return true;
						}
						if (row - 1 > -1 && col + 1 < 8 && (blackCastlingSquares [row - 1, col + 1] == "QS" || blackCastlingSquares [row - 1, col + 1] == "BS")) {
							return true;
						}
						if (col + 1 < 8 && (blackCastlingSquares [row, col + 1] == "QS" || blackCastlingSquares [row, col + 1] == "BS")) {
							return true;
						}
					}

				}
			}
		}

		return false;
	}
	bool BlackKingTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BK";
		return WhiteResponseTest (tempBoard);

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

		bool test1 = BlackPawnTest1 (selectRow, selectCol, destRow, destCol);
		if (test1 == false)
			return false;

		bool test2 = BlackPawnTest2 (selectRow, selectCol, destRow, destCol);
		if (test2 == false)
			return false;

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
	bool BlackPawnTest2 (int selectRow, int selectCol, int destRow, int destCol) {
		string[,] tempBoard = new string[8, 8];
		CopyBoardToTemp (out tempBoard);

		// Even if pawn promotes, doesn't change whether or not white king will or will not be captured by pawn move.
		tempBoard [selectRow, selectCol] = "--";
		tempBoard [destRow, destCol] = "BP";
		return WhiteResponseTest (tempBoard);

	}

	#endregion
	#region White response
	bool WhiteResponseTest (string[,] tempBoard) {
		if (WhitePawnAttacksKing (tempBoard)) {
			return false;
		}
		if (WhiteRookAttacksKing (tempBoard)) {
			return false;
		}
		if (WhiteKnightAttacksKing (tempBoard)) {
			return false;
		}
		if (WhiteBishopAttacksKing (tempBoard)) {
			return false;
		}
		if (WhiteQueenAttacksKing (tempBoard)) {
			return false;
		}
		if (WhiteKingAttacksKing (tempBoard)) {
			return false;
		}
		return true;
	}
	bool WhitePawnAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WP") {
					if (col < 7 && tempBoard [row - 1, col + 1] == "BK") {
						return true;
					} 
					if (col > 0 && tempBoard [row - 1, col - 1] == "BK") {
						return true;
					} 
				}

			}
		}
		return false;
	}
	bool WhiteRookAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WR") {
					if (WhiteRookMovesRightFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteRookMovesLeftFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteRookMovesUpFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteRookMovesDownFindsKing(tempBoard, row, col)) {
						return true;
					}
				}
			}
		}
		return false;
	}
	bool WhiteKnightAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WN") {
					if (row > 0 && col > 1 && tempBoard [row - 1, col - 2] == "BK") {
						return true;
					}
					if (row > 1 && col > 0 && tempBoard [row - 2, col - 1] == "BK") {
						return true;
					}
					if (row > 1 && col < 7 && tempBoard [row - 2, col + 1] == "BK") {
						return true;
					}
					if (row > 0 && col < 6 && tempBoard [row - 1, col + 2] == "BK") {
						return true;
					}
					if (row < 7 && col < 6 && tempBoard [row + 1, col + 2] == "BK") {
						return true;
					}
					if (row < 6 && col < 7 && tempBoard [row + 2, col + 1] == "BK") {
						return true;
					}
					if (row < 6 && col > 0 && tempBoard [row + 2, col - 1] == "BK") {
						return true;
					}
					if (row < 7 && col > 1 && tempBoard [row + 1, col - 2] == "BK") {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool WhiteBishopAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WB") {
					if (WhiteBishopMovesDiagTopLeft (tempBoard, row, col)) {
						return true;
					}
					if (WhiteBishopMovesDiagTopRight (tempBoard, row, col)) {
						return true;
					}
					if (WhiteBishopMovesDiagBottomLeft (tempBoard, row, col)) {
						return true;
					}
					if (WhiteBishopMovesDiagBottomRight (tempBoard, row, col)) {
						return true;
					}

				}
			}
		}
		return false;
	}
	bool WhiteQueenAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WQ") {
					if (WhiteQueenMovesDiagTopLeft (tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesDiagTopRight (tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesDiagBottomLeft (tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesDiagBottomRight (tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesRightFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesLeftFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesUpFindsKing(tempBoard, row, col)) {
						return true;
					}
					if (WhiteQueenMovesDownFindsKing(tempBoard, row, col)) {
						return true;
					}
				}
			}
		}
		return false;
	}
	bool WhiteKingAttacksKing (string[,] tempBoard) {
		for (int row = 0; row < 8; row++) {
			for (int col = 0; col < 8; col++) {
				if (tempBoard [row, col] == "WK") {
					if (col > 0 && tempBoard [row, col - 1] == "BK") {
						return true;
					}
					if (row > 0 && col > 0 && tempBoard [row - 1, col - 1] == "BK") {
						return true;
					}
					if (row > 0 && tempBoard [row - 1, col] == "BK") {
						return true;
					}
					if (row > 0 && col < 7 && tempBoard [row - 1, col + 1] == "BK") {
						return true;
					}
					if (col < 7 && tempBoard [row, col + 1] == "BK") {
						return true;
					}
					if (row < 7 && col < 7 && tempBoard [row + 1, col + 1] == "BK") {
						return true;
					}
					if (row < 7 && tempBoard [row + 1, col] == "BK") {
						return true;
					}
					if (row < 7 && col > 0 && tempBoard [row + 1, col - 1] == "BK") {
						return true;
					}

				}
			}
		}
		return false;

	}
	bool WhiteBishopMovesDiagTopLeft (string[,] tempBoard, int row, int col) {
		row--;
		col--;
		while (row > -1 && col > -1) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col--;
		}
		return false;
	}
	bool WhiteBishopMovesDiagTopRight (string[,] tempBoard, int row, int col) {
		row--;
		col++;
		while (row > -1 && col < 8) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col++;
		}
		return false;
	}
	bool WhiteBishopMovesDiagBottomRight (string[,] tempBoard, int row, int col) {
		row++;
		col++;
		while (row < 8 && col < 8) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col++;
		}
		return false;
	}
	bool WhiteBishopMovesDiagBottomLeft (string[,] tempBoard, int row, int col) {
		row++;
		col--;
		while (row < 8 && col > -1) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col--;
		}
		return false;
	}
	bool WhiteRookMovesRightFindsKing (string[,] tempBoard, int row, int col) {
		col++;
		while (col < 8) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col++;
		}
		return false;
	}
	bool WhiteRookMovesLeftFindsKing (string[,] tempBoard, int row, int col) {
		col--;
		while (col > -1) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col--;
		}
		return false;
	}
	bool WhiteRookMovesUpFindsKing (string[,] tempBoard, int row, int col) {
		row--;
		while (row > -1) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row--;
		}
		return false;
	}
	bool WhiteRookMovesDownFindsKing (string[,] tempBoard, int row, int col) {
		row++;
		while (row < 8) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row++;
		}
		return false;

	}
	bool WhiteQueenMovesDiagTopLeft (string[,] tempBoard, int row, int col) {
		row--;
		col--;
		while (row > -1 && col > -1) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col--;
		}
		return false;
	}
	bool WhiteQueenMovesDiagTopRight (string[,] tempBoard, int row, int col) {
		row--;
		col++;
		while (row > -1 && col < 8) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row--;
			col++;
		}
		return false;
	}
	bool WhiteQueenMovesDiagBottomRight (string[,] tempBoard, int row, int col) {
		row++;
		col++;
		while (row < 8 && col < 8) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col++;
		}
		return false;
	}
	bool WhiteQueenMovesDiagBottomLeft (string[,] tempBoard, int row, int col) {
		row++;
		col--;
		while (row < 8 && col > -1) {
			if (tempBoard [row, col] == "BK") {
				return true;
			}
			if (tempBoard [row, col] [0] == 'B' || tempBoard [row, col] [0] == 'W') {
				return false;
			}
			row++;
			col--;
		}
		return false;
	}
	bool WhiteQueenMovesRightFindsKing (string[,] tempBoard, int row, int col) {
		col++;
		while (col < 8) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col++;
		}
		return false;
	}
	bool WhiteQueenMovesLeftFindsKing (string[,] tempBoard, int row, int col) {
		col--;
		while (col > -1) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			col--;
		}
		return false;
	}
	bool WhiteQueenMovesUpFindsKing (string[,] tempBoard, int row, int col) {
		row--;
		while (row > -1) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row--;
		}
		return false;
	}
	bool WhiteQueenMovesDownFindsKing (string[,] tempBoard, int row, int col) {
		row++;
		while (row < 8) {
			if (tempBoard [row, col] == "BK")
				return true;
			if (tempBoard [row, col] [0] == 'W' || tempBoard [row, col] [0] == 'B')
				return false;
			row++;
		}
		return false;

	}

	#endregion
	#endregion
}
