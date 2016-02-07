using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	enum State {WhiteSelectionState, WhiteDestinationState, BlackSelectionState, BlackDestinationState};
	State state = State.WhiteSelectionState;

	#region Presentation states
	WhiteSelectionPresentationState whiteSelectionPresentationState;
	WhiteDestinationPresentationState whiteDestinationPresentationState;
	BlackSelectionPresentationState blackSelectionPresentationState;
	BlackDestinationPresentationState blackDestinationPresentationState;
	#endregion

	// Use this for initialization
	void Start () {
		// TODO Initialize board.
		whiteSelectionPresentationState = new WhiteSelectionPresentationState ();
		whiteDestinationPresentationState = new WhiteDestinationPresentationState ();
		blackSelectionPresentationState = new BlackSelectionPresentationState ();
		blackDestinationPresentationState = new BlackDestinationPresentationState ();
		StatesCoordinator ();
	}
	void StatesCoordinator () {
		switch (state) {
			case State.WhiteSelectionState:
				WhiteSelectionState();
				break;
			case State.WhiteDestinationState:
				WhiteDestinationState();
				break;
			case State.BlackSelectionState:
				BlackSelectionState();
				break;
			case State.BlackDestinationState:
				BlackDestinationState();
				break;
			default:
				Debug.LogError("Not a valid state!");
				break;
		}


	}
	#region States
	void WhiteSelectionState() {
		whiteSelectionPresentationState.OnEnter ();
	}
	void WhiteDestinationState() {
		whiteDestinationPresentationState.OnEnter ();
	}
	void BlackSelectionState() {
		blackSelectionPresentationState.OnEnter ();
	}
	void BlackDestinationState() {
		blackDestinationPresentationState.OnEnter ();
	}

	#endregion
	#region Helper functions


	#endregion
}
