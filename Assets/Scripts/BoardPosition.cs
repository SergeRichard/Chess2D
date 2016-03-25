using UnityEngine;
using System.Collections;

public class BoardPosition
{
	public string[,] piecesOnBoard;
	public int positionNumber;
	public bool whiteQueenSideCastlingCurrentPosition;
	public bool whiteKingSideCastlingCurrentPosition;
	public bool blackKingSideCastlingCurrentPosition;
	public bool blackQueenSideCastlingCurrentPosition;
	public bool whiteTakesEnPassant;
	public bool blackTakesEnPassant;
	public int whiteTakesEnPassantCol;
	public int blackTakesEnPassantCol;

	public static int enPassantCol = -1;
	public static string promotionChoice = "";

	public static bool whiteQueenSideCastling = true;
	public static bool whiteKingSideCastling = true;
	public static bool blackQueenSideCastling = true;
	public static bool blackKingSideCastling = true;
	public static bool whiteTakesEnPassantFlag = false;
	public static bool blackTakesEnPassantFlag = false;
	public static bool kingInCheck = false;


	public string moveNotation;

	public BoardPosition (int positionNumber, string moveNotation) {
		this.positionNumber = positionNumber;
		this.moveNotation = moveNotation;
		whiteQueenSideCastlingCurrentPosition = whiteQueenSideCastling;
		whiteKingSideCastlingCurrentPosition = whiteKingSideCastling;
		blackKingSideCastlingCurrentPosition = blackKingSideCastling;
		blackQueenSideCastlingCurrentPosition = blackQueenSideCastling;
		whiteTakesEnPassant = whiteTakesEnPassantFlag;
		blackTakesEnPassant = blackTakesEnPassantFlag;
		whiteTakesEnPassantCol = enPassantCol;
		blackTakesEnPassantCol = enPassantCol;
	}
	public void AddCheckToNotation() {
		moveNotation += "+";

	}
	public void AddBlackWinToNotation() {
		moveNotation += "\n0-1";
	}
	public void AddWhiteWinToNotation() {
		moveNotation += "\n1-0";
	}
	public void AddDrawToNotation() {
		moveNotation += "\n½-½";
	}
}

