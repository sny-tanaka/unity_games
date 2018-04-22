using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

	public static int userID;

	public void getUserID () {

		// 初回起動フラグの取得
		int first = PlayerPrefs.GetInt("FirstFlag", 0);

		if (first == 0) {
			// 初回起動なのでUserIDを作成する テスト用に1を設定 本来はサーバーとやり取りして作成する
			PlayerPrefs.SetInt ("UserID", 1);
			userID = 1;
			PlayerPrefs.SetInt ("FirstFlag", 1);
		} else {
			// 初回起動ではないので端末からIDを取得
			userID = PlayerPrefs.GetInt ("UserID", 0);
		}
	}
}
