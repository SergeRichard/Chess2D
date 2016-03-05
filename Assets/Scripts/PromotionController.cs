using UnityEngine;
using System.Collections;

public class PromotionController : MonoBehaviour {
	public GameObject promotionWindow;

	public GameObject whiteQueen;
	public GameObject whiteRook;
	public GameObject whiteBishop;
	public GameObject whiteKnight;

	public GameObject blackQueen;
	public GameObject blackRook;
	public GameObject blackBishop;
	public GameObject blackKnight;

	public GameObject promotionText;

	public GameManager gameManager;

	private bool waitForClick = false;

	public void ShowWhitePromotionWindow(bool isShown) {

		waitForClick = isShown;

		promotionWindow.SetActive(isShown);

		whiteQueen.SetActive(isShown);
		whiteRook.SetActive(isShown);
		whiteBishop.SetActive(isShown);
		whiteKnight.SetActive(isShown);

		promotionText.SetActive (isShown);
	}
	public void ShowBlackPromotionWindow(bool isShown) {
		waitForClick = isShown;

		promotionWindow.SetActive(isShown);

		blackQueen.SetActive(isShown);
		blackRook.SetActive(isShown);
		blackBishop.SetActive(isShown);
		blackKnight.SetActive(isShown);

		promotionText.SetActive (isShown);
	}

	public void OnPromotionSquareClicked(string objectName) {
		if (waitForClick) {
			switch (objectName) {
			case "WhiteQueen":
				gameManager.OnWhitePromotionClicked ("WQ");
				break;
			case "WhiteRook":
				gameManager.OnWhitePromotionClicked ("WR");
				break;
			case "WhiteBishop":
				gameManager.OnWhitePromotionClicked ("WB");
				break;
			case "WhiteKnight":
				gameManager.OnWhitePromotionClicked ("WN");
				break;
			case "BlackQueen":
				gameManager.OnBlackPromotionClicked ("BQ");
				break;
			case "BlackRook":
				gameManager.OnBlackPromotionClicked ("BR");
				break;
			case "BlackBishop":
				gameManager.OnBlackPromotionClicked ("BB");
				break;
			case "BlackKnight":
				gameManager.OnBlackPromotionClicked ("BN");
				break;
			}
		}
	}

}
