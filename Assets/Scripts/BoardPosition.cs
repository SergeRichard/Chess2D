using UnityEngine;
using System.Collections;

public class BoardPosition
{
	public string[,] piecesOnBoard;
	public int positionNumber;
	// Use this for initialization

	public BoardPosition (int positionNumber) {
		this.positionNumber = positionNumber;
	}
}

