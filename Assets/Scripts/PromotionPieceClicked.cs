using UnityEngine;
using System.Collections;

public class PromotionPieceClicked : MonoBehaviour {

	void OnMouseDown() {
		PromotionController promotionController = GameObject.FindObjectOfType<PromotionController> ();
		promotionController.OnPromotionSquareClicked (transform.name);

	}
}
