using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skill : MonoBehaviour {

	// タイプ管理, 外部から設定する	
	public int typeID;

	// プレイヤーかエネミーか
	public bool ifPlayer;

	public void sendChoicedType(){
		// ButtleManagerに選択コマンドを送る
		if (ifPlayer){
			int id = GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id;
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().playerChoicedHands[id] = typeID;
			Texture2D texture = Resources.Load("TypeIcons/" + typeID.ToString()) as Texture2D;
			GameObject.Find ("Player"+ (id + 1).ToString() +"Hand").GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id += 1;
			if (GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id == 3) {
				GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id = 0;
			}
			if (typeID == 1) {
				GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().playerSpecial = 0;
			}
			if (id == 2){
				GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().playerCommandFin();
			} else {
				GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().playerCommand();
			}
		} else {
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().enemyChoicedHand = typeID;
		}
	}
}