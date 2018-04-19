using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill : MonoBehaviour {

	// タイプ管理, 外部から設定する
	public int id = 0;
	public int typeID;

	// プレイヤーかエネミーか
	public bool ifPlayer;

	public void sendChoicedType(){
		// ButtleManagerに選択コマンドを送る
		if (ifPlayer){
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().playerChoicedHands[id] = typeID;
			id += 1;
			if (id == 3) {
				id = 0;
			}
			if (typeID == 1) {
				GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().playerSpecial = 0;
			}
			GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().commandReceiveFlag = false;
		} else {
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().enemyChoicedHand = typeID;
		}
	}
}