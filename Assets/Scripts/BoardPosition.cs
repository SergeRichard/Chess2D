using UnityEngine;
using System.Collections;

public class BoardPosition
{
	public string[,] piecesOnBoard;
	public int positionNumber;
	public bool queenSideCastling = true;
	public bool kingSideCastling = true;

	public string moveNotation;

	public BoardPosition (int positionNumber, string moveNotation, bool queenSideCastling, bool kingSideCastling) {
		this.positionNumber = positionNumber;
		this.moveNotation = moveNotation;
		this.queenSideCastling = queenSideCastling;
		this.kingSideCastling = kingSideCastling;
	}
}

