using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtleManager_vsNPC : MonoBehaviour {

	// 外部から設定される
	public int id = 0;
	public int[] playerChoicedHands = new int[3];
    public int enemyChoicedHand;
	public int nextEnemyHand;

	// ターン数
	int turn = 0;

    // 勝ち負け
    int[] WoL = new int[3];

	// 必殺技ゲージ
	public int playerSpecial;
	public int enemySpecial;

	// 終了フラグ
	bool finishFlag = true;

	// ボイス再生オブジェクト
	public AudioSource voiceSource;

	// 勝利サウンド
	public AudioSource victory;

	// ゲームオーバーサウンド
	public AudioSource gameover;

	void Start(){
		// Playerの中からランダムで戦闘開始時のボイスを再生
		int i = Random.Range(1, 4);
		voiceSource.clip = GameObject.Find("Player"+i.ToString()).GetComponent<CharacterSet>().voice[7];
		voiceSource.Play();
		
		// バトルスタート
		StartCoroutine("buttleRunning");
	}

	// 一連の流れ
	IEnumerator buttleRunning(){
		priming ();
		yield return 0;
	}

    // スタートの掛け声
    void priming(){
        // 戦闘画面に"じゃんけん"と表示
		GameObject.Find("Text_Center").GetComponent<Text>().text = "ジャン喧！";

		// ターン数を表示
		turn += 1;
		GameObject.Find("Text_TurnCount").GetComponent<Text>().text = "ターン"+turn.ToString();
		playerCommand ();

		// nextEnemyHandに値があれば表示する
		if (nextEnemyHand != 0) {
			Texture2D texture = Resources.Load ("TypeIcons/" + nextEnemyHand.ToString ()) as Texture2D;
			GameObject.Find ("NextEnemyHand").GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}
    }

	// Playerにコマンド選択させる
	public void playerCommand(){
		// ボタンの取得
		Button gu = GameObject.Find("Gu").GetComponent<Button>();
		Button choki = GameObject.Find("Choki").GetComponent<Button>();
		Button pa = GameObject.Find("Pa").GetComponent<Button>();
		Button zenbu = GameObject.Find("Zenbu").GetComponent<Button>();

		// ボタンの有効化
		gu.interactable = true;
		choki.interactable = true;
		pa.interactable = true;

		// Specialゲージが溜まっていれば有効化
		if (playerSpecial >= 100) {
			zenbu.interactable = true;
		} else {
			zenbu.interactable = false;
		}

		// 対象のキャラクター以外を暗くする
		bool[] dark = new bool[3];
		for (int j=0; j<3; j++){
			dark[j] = false;
		}
		dark [id] = true;
		for (int j = 0; j < 3; j++) {
			if (dark [j]) {
				GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
			} else {
				GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f, 1);
			}
		}
	}

	public void playerCommandFin(){

		// ボタンの取得
		Button gu = GameObject.Find("Gu").GetComponent<Button>();
		Button choki = GameObject.Find("Choki").GetComponent<Button>();
		Button pa = GameObject.Find("Pa").GetComponent<Button>();
		Button zenbu = GameObject.Find("Zenbu").GetComponent<Button>();

		// ボタンの無効化
		gu.interactable = false;
		choki.interactable = false;
		pa.interactable = false;
		zenbu.interactable = false;

		// 全キャラ明るくする
		for (int j = 0; j < 3; j++) {
			GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		}
		enemyRandomHand ();
	}

    // NPCの出手を決める
    void enemyRandomHand(){
		// nextEnemyHandに値があればそれを出す
		if (nextEnemyHand != 0) {
			enemyChoicedHand = nextEnemyHand;
			nextEnemyHand = 0;
			GameObject.Find ("NextEnemyHand").GetComponent<Image> ().sprite = null;
		} else {
			// enemyの手をランダムで決める
			enemyChoicedHand = Random.Range (2, 5);
		}
		displayHands ();
    }

    // Enemyの手を画面上に表示する
    void displayHands(){
		// 戦闘画面に"ポン"と表示
		GameObject.Find("Text_Center").GetComponent<Text>().text = "PON！";

		Texture2D texture = Resources.Load("TypeIcons/" + enemyChoicedHand.ToString()) as Texture2D;
		GameObject.Find ("EnemyHand").GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);

		WoLJudge ();
    }

    // 手の勝ち負けを判定する
    void WoLJudge(){
		for (int i = 0; i < 3; i++) {
			WoL [i] = judgingDeadlock (playerChoicedHands [i], enemyChoicedHand);
		}
		StartCoroutine("specialPaformance");
    }

	// 全員が勝ったときの特別演出
	IEnumerator specialPaformance(){
		if (WoL [0] == 1 && WoL [1] == 1 && WoL [2] == 1) {
			GameObject inst1 = Instantiate((GameObject)Resources.Load ("Prefabs/CutIn1"), new Vector3(0, 0, 0), Quaternion.identity);
			Texture2D texture = Resources.Load("CutIn/" + GameObject.Find("Player1").GetComponent<CharacterSet>().monsterID.ToString()) as Texture2D;
			Transform sp1 = inst1.transform.GetChild (0);
			sp1.GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector3.zero);
			inst1.transform.SetParent (GameObject.Find ("CutInField").transform, false);
			sp1.localPosition = new Vector3 (-200.0f, 0f, 0f);
			for (int i = 0; i < 25; i++) {
				sp1.localPosition += new Vector3 (8f, 0f, 0f);
				yield return null;
			}
			yield return new WaitForSeconds (1.0f);

			GameObject inst2 = Instantiate((GameObject)Resources.Load ("Prefabs/CutIn2"), new Vector3(0, -120, 0), Quaternion.identity);
			Texture2D texture2 = Resources.Load("CutIn/" + GameObject.Find("Player2").GetComponent<CharacterSet>().monsterID.ToString()) as Texture2D;
			Transform sp2 = inst2.transform.GetChild (0);
			sp2.GetComponent<Image> ().sprite = Sprite.Create (texture2, new Rect (0, 0, texture2.width, texture2.height), Vector3.zero);
			inst2.transform.SetParent (GameObject.Find ("CutInField").transform, false);
			sp2.localPosition = new Vector3 (-200.0f, 0f, 0f);
			for (int i = 0; i < 25; i++) {
				sp2.localPosition += new Vector3 (8f, 0f, 0f);
				yield return null;
			}
			yield return new WaitForSeconds (1.0f);

			GameObject inst3 = Instantiate((GameObject)Resources.Load ("Prefabs/CutIn3"), new Vector3(0, -240, 0), Quaternion.identity);
			Texture2D texture3 = Resources.Load("CutIn/" + GameObject.Find("Player3").GetComponent<CharacterSet>().monsterID.ToString()) as Texture2D;
			Transform sp3 = inst3.transform.GetChild (0);
			sp3.GetComponent<Image> ().sprite = Sprite.Create (texture3, new Rect (0, 0, texture3.width, texture3.height), Vector3.zero);
			inst3.transform.SetParent (GameObject.Find ("CutInField").transform, false);
			sp3.localPosition = new Vector3 (-200.0f, 0f, 0f);
			for (int i = 0; i < 25; i++) {
				sp3.localPosition += new Vector3 (8f, 0f, 0f);
				yield return null;
			}
			yield return new WaitForSeconds (1.0f);

			Destroy (inst1);
			Destroy (inst2);
			Destroy (inst3);
		}
		damageCulc ();
	}

    // ダメージ計算
    public void damageCulc(){
        float originalDmg;
        int dmg;

		// 対象のキャラ以外暗くする
		bool[] dark = new bool[3];
		for (int j=0; j<3; j++){
			dark[j] = false;
		}
		dark [id] = true;
		for (int j = 0; j < 3; j++) {
			if (dark [j]) {
				GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
			} else {
				GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f, 1);
			}
		}
		// あいこなら何もしない
        if (WoL[id] == 0){
            if (id == 2){
				id = 0;
				dmgFin();
				return;
			} else {
				id += 1;
				damageCulc();
				return;
			}
        }
        // 攻撃側と防御側のコンポーネント取得
        CharacterSet atkSide;
        CharacterSet defSide;
        int handType;
        if (WoL[id] == 1){
            atkSide = GameObject.Find("Player"+ (id+1).ToString()).GetComponent<CharacterSet>();
            defSide = GameObject.Find("Enemy").GetComponent<CharacterSet>();
            handType = playerChoicedHands[id];
        } else {
            atkSide = GameObject.Find("Enemy").GetComponent<CharacterSet>();
            defSide = GameObject.Find("Player"+ (id+1).ToString()).GetComponent<CharacterSet>();
            handType = enemyChoicedHand;
        }
        // 必要数値の設定
        int atkType = atkSide.type;
        int defType = defSide.type;
        int atk = atkSide.status[handType-1];
        int pow = atkSide.skillpows[handType-1];
        int rand = Random.Range(85, 101);
        int def = defSide.status[handType-1];
		string nam = atkSide.skillNames[handType-1];

		// スキル名の表示
		GameObject inst = Instantiate((GameObject)Resources.Load ("Prefabs/CutIn_bg"), new Vector2(0, -200), Quaternion.Euler(0, 0, 30));
		inst.transform.GetChild(0).GetComponent<Text>().text = nam;
		inst.transform.SetParent (GameObject.Find ("CutInField").transform, false);
		Destroy(inst, 1.0f);

		// 回復技のときの処理
		if (pow < 0){
			// ボイスの再生
			voiceSource.clip = atkSide.voice[5];
			voiceSource.Play();

			// 基礎回復量
			float recovery = -1.0f * atk * atk * pow * rand;
			recovery = recovery / (atk * 2);
			recovery = recovery / 10000.0f;

			// タイプ一致ボーナス
			if (atkType == handType){
				recovery *= 1.5f;
			}
			
			// 小数点以下切り捨て
			int rec = (int)recovery;
			Debug.Log ("回復:"+rec.ToString());

			// SEを鳴らす


			// HPを増やす
			if (WoL [id] == 1) {
				HPbarController playerHP = GameObject.Find ("PlayerHP").GetComponent<HPbarController> ();
				if (playerHP.hp + rec < playerHP.maxHp){
					playerHP.hp += rec;
				} else {
					playerHP.hp = playerHP.maxHp;
				}
				playerHP.barIncrease ();
			} else {
				HPbarController enemyHP = GameObject.Find ("EnemyHP").GetComponent<HPbarController> ();
				if (enemyHP.hp + rec < enemyHP.maxHp){
					enemyHP.hp += rec;
				} else{
					enemyHP.hp = enemyHP.maxHp;
				}
				enemyHP.barIncrease ();
			}
			return;
		}

		// 攻撃ボイスの再生
		voiceSource.clip = atkSide.voice[handType];
		voiceSource.PlayOneShot(atkSide.voice[handType], 1.0f);

        // 基本ダメージ
		originalDmg = 1.0f * atk * atk * pow * rand;
		originalDmg = originalDmg / (atk + def);
		originalDmg = originalDmg / 10000.0f;
		Debug.Log(atk.ToString()+"*"+atk.ToString()+"*"+pow.ToString()+"*"+rand.ToString()+"/("+atk.ToString()+"+"+def.ToString()+")/10000="+originalDmg.ToString());
        // タイプ一致ボーナス
        if (atkType == handType){
            originalDmg *= 1.2f;
        }

		// 全員が勝利していたらダメージ2倍
		if (WoL [0] == 1 && WoL [1] == 1 && WoL [2] == 1) {
			originalDmg *= 2.0f;
		}

        // 弱点をついて敵の手が透ける処理
		if (WoL [id] == 1) {
        	int chemi = judgingDeadlock(handType, defType);
			if (chemi == 1) {
				// 次の相手の手が表示される
				nextEnemyHand = Random.Range (2, 5);
			}
        }

        // 小数点以下切り捨て
        dmg = (int)originalDmg;
		Debug.Log ("ダメージ:"+dmg.ToString());

		// 被ダメボイスの再生
		voiceSource.clip = defSide.voice[6];
		voiceSource.Play();

		// HPを減らす
		if (WoL [id] == 1) {
			if (GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().hp > dmg){
				GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().hp -= dmg;
			} else {
				GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().hp = 0;
			}
			displayDmg(dmg);
			GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().barDecrease ();
		} else {
			if (GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp > dmg){
				GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp -= dmg;
			} else{
				GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp = 0;
			}
			GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().barDecrease ();
		}
	}
	public void dmgFin(){
		// 全キャラ明るくする
		for (int j = 0; j < 3; j++) {
			GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
		}
		deathJudge ();
    }

    // 3すくみのa目線の勝ち負け判定(勝ち=1,負け=2,あいこ=0)
    int judgingDeadlock(int a, int b){
        if (a == b){
            return 0;
        } else if (a==2 && b==4){
            // aがグー、bがパー
            return 2;
        } else if (a==4 && b==2){
            // aがパー、bがグー
            return 1;
        } else if (a < b){
            return 1;
        } else {
            return 2;
        }
    }

	// HPがゼロかどうかの判定
	void deathJudge(){
		if (GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().hp <= 0) {
			// BGMを止める
			GameObject.Find("BGM").GetComponent<AudioSource>().Stop();

			// 勝利サウンドを流す
			victory.Play();

			// 勝ち
			Debug.Log("You Win!");
			finishFlag = false;
			return;
		} else if (GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp <= 0) {
			// BGMを止める
			GameObject.Find("BGM").GetComponent<AudioSource>().Stop();

			// ゲームオーバーサウンドを流す
			gameover.Play();

			// 画面を暗くする
			GameObject inst = Instantiate((GameObject)Resources.Load ("Prefabs/mask_bg"), Vector2.zero, Quaternion.identity);
			inst.transform.SetParent (GameObject.Find ("CutInField").transform, false);

			// 負け
			Debug.Log("You Lose");
			finishFlag = false;
			return;
		}
		initHand ();
		StartCoroutine("buttleRunning");
	}

	// 選んだ手の初期化
	void initHand(){
		for (int i = 0; i < 3; i++) {
			playerChoicedHands [i] = 0;
		}
		enemyChoicedHand = 0;
		GameObject.Find("Text_Center").GetComponent<Text>().text = "";
		GameObject.Find ("EnemyHand").GetComponent<Image> ().sprite = null;
		GameObject.Find ("Player1Hand").GetComponent<Image> ().sprite = null;
		GameObject.Find ("Player2Hand").GetComponent<Image> ().sprite = null;
		GameObject.Find ("Player3Hand").GetComponent<Image> ().sprite = null;
	}

	// ダメージを設定範囲内のランダムな位置に表示
	void displayDmg(int dmg){
		// xの範囲
		float x = Random.Range(70, 100);

		// yの範囲
		float y = Random.Range(150, 220);

		// 角度
		float r = Random.Range(-30, 30);

		// プレハブからインスタンスを作成
		GameObject inst = Instantiate((GameObject)Resources.Load ("Prefabs/DmgText"), new Vector3(x, y, 0), Quaternion.Euler(0, 0, r));

		// テキストにダメージをセット
		inst.GetComponent<Text>().text = dmg.ToString();

		// baseキャンバス上に表示
		inst.transform.SetParent (GameObject.Find ("base").transform, false);

		// 表示後1.5秒で破壊
		Destroy(inst, 1.5f);
	}
}