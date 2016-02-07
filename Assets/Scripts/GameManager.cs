using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	enum State {WhiteSelectionState, WhiteDestinationState, BlackSelectionState, BlackDestinationState};
	State state = State.WhiteSelectionState;

	// Use this for initialization
	void Start () {
		// TODO Initialize board.

	}

}
