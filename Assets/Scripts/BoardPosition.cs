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

	public static bool whiteQueenSideCastling = true;
	public static bool whiteKingSideCastling = true;
	public static bool blackQueenSideCastling = true;
	public static bool blackKingSideCastling = true;


	public string moveNotation;

	public BoardPosition (int positionNumber, string moveNotation) {
		this.positionNumber = positionNumber;
		this.moveNotation = moveNotation;
		whiteQueenSideCastlingCurrentPosition = whiteQueenSideCastling;
		whiteKingSideCastlingCurrentPosition = whiteKingSideCastling;
		blackKingSideCastlingCurrentPosition = blackKingSideCastling;
		blackQueenSideCastlingCurrentPosition = blackQueenSideCastling;
	}
}

