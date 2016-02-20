using UnityEngine;
using System.Collections;

public class BoardPosition
{
	public string[,] piecesOnBoard;
	public int positionNumber;

	public string moveNotation;

	public BoardPosition (int positionNumber, string moveNotation) {
		this.positionNumber = positionNumber;
		this.moveNotation = moveNotation;
	}
}

