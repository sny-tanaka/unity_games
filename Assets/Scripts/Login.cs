using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	public TitleManager tm;

	public void login(){

		// UserIDの取得
		tm.getUserID ();

		// ホーム画面へ遷移

	}
}
