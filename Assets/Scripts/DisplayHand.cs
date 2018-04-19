using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHand : MonoBehaviour {

	public Image image;
	
	// Update is called once per frame
	void Update () {
		// スプライトがないときは透過しておく
		if (image.sprite == null) {
			image.color = new Color (1, 1, 1, 0);
		} else {
			image.color = new Color (1, 1, 1, 1);
		}
	}
}
