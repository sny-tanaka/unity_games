using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbarController : MonoBehaviour {

	// メインゲージのSliderオブジェクトの設定
	public GameObject mainBar;
	Slider mainSlider;

	// ダメージ量表示用のSliderオブジェクトの設定
	public GameObject subBar;
	Slider subSlider;

	// maxHP取得用オブジェクト
	public GameObject[] charaSet;

	// HP表示テキスト
	Text text;

	// PlayerかEnemyか
	public bool ifPlayer;

	// 設定数値
	public int maxHp;
	public int hp;

	public AudioSource se;

	void Start () {
		// maxHPの取得
		for (int i = 0; i < charaSet.Length; i++) {
			maxHp += charaSet [i].GetComponent<CharacterSet> ().maxHp;
		}
		if (ifPlayer) {
			// Text_MaxHPにmaxHPの値を書き込み
			GameObject.Find ("Text_MaxHP").GetComponent<Text> ().text = maxHp.ToString ();

			// Text_NowHPの取得と書き込み
			text = GameObject.Find ("Text_NowHP").GetComponent<Text> ();
			text.text = maxHp.ToString ();
		}

		// Sliderコンポーネントの取得
		mainSlider = mainBar.GetComponent<Slider>();
		subSlider = subBar.GetComponent<Slider>();

		// 最大値の設定
		mainSlider.maxValue = maxHp;
		subSlider.maxValue = maxHp;

		// 現在値を最大HPに設定
		hp = maxHp;
		mainSlider.value = hp;
		subSlider.value = hp;
	}
	
	public void barDecrease(){

		// Playerの場合はテキストの値を更新する
		if (ifPlayer) {
			text.text = hp.ToString ();
		}

		// SEを鳴らす
		se.Play();

		// メインゲージを即現在HPに変更する
		mainSlider.value = hp;

		// サブゲージを減らすコルーチンを呼びだす
		StartCoroutine ("subBarDecrease");
	}

	IEnumerator subBarDecrease(){
		// 現在HPの値になるまで徐々に減らす
		float f = subSlider.value;
		float fd = (f - hp) / 100.0f;
		while (f > hp){
			f -= fd;
			subSlider.value = f;
			yield return null;
		}
		nextDmg();
	}
	void nextDmg(){
		int id = GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id;
		if (id == 2) {
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id = 0;
			GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().dmgFin();
		} else {
			GameObject.Find("ButtleManager").GetComponent<ButtleManager_vsNPC>().id += 1;
			GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().damageCulc();
		}
	}

	public void barIncrease(){
		// Playerの場合はテキストの値を更新する
		if (ifPlayer) {
			text.text = hp.ToString ();
		}

		// サブゲージを即現在HPに変更する
		subSlider.value = hp;

		// メインゲージを増やすコルーチンを呼びだす
		StartCoroutine ("mainBarIncrease");
	}

	IEnumerator mainBarIncrease(){
		// 現在HPの値になるまで徐々に増やす
		float f = mainSlider.value;
		float fd = (hp - f) / 100.0f;
		while (f < hp){
			f += fd;
			mainSlider.value = f;
			yield return null;
		}
		nextDmg();
	}
}