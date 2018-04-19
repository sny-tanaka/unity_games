using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtleManager_vsNPC : MonoBehaviour {

	// 外部から設定される
	public int[] playerChoicedHands = new int[3];
    public int enemyChoicedHand;

    // 勝ち負け
    int[] WoL = new int[3];

	// 必殺技ゲージ
	public int playerSpecial;
	public int enemySpecial;

	// コマンド待機用
	public bool commandReceiveFlag;

	// 終了フラグ
	bool finishFlag = true;

	public void begin(){
		StartCoroutine("buttleRunning");
	}

	// 一連の流れ
	IEnumerator buttleRunning(){
		priming ();
		yield return 0;

		if (finishFlag) {
			StartCoroutine("buttleRunning");
		}
	}

    // スタートの掛け声
    void priming(){
        // 戦闘画面に"じゃんけん"と表示
		GameObject.Find("Text_Center").GetComponent<Text>().text = "ジャン喧！";
		playerCommand ();
    }

	// Playerにコマンド選択させる
	void playerCommand(){
		StartCoroutine ("playerCommandCor");
	}
	IEnumerator playerCommandCor(){
		// ボタンの取得
		Button gu = GameObject.Find("Gu").GetComponent<Button>();
		Button choki = GameObject.Find("Choki").GetComponent<Button>();
		Button pa = GameObject.Find("Pa").GetComponent<Button>();
		Button zenbu = GameObject.Find("Zenbu").GetComponent<Button>();

		// ボタンの有効化
		gu.interactable = true;
		choki.interactable = true;
		pa.interactable = true;

		Texture2D texture;

		for (int i = 0; i < 3; i++) {
			commandReceiveFlag = true;
			// Specialについては毎回判定
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
			dark [i] = true;
			for (int j = 0; j < 3; j++) {
				if (dark [j]) {
					GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				} else {
					GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f, 1);
				}
			}
			// コマンドが選択されるまで待機
			while (commandReceiveFlag) {
				yield return null;
			}

			// 出手のスプライトを取得
			texture = Resources.Load("TypeIcons/" + playerChoicedHands[i].ToString()) as Texture2D;
			Debug.Log ("Player" + (i + 1).ToString () + "Hand");
			GameObject.Find ("Player"+ (i + 1).ToString() +"Hand").GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		}

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
        // enemyの手をランダムで決める
        int h = Random.Range(2, 5);
        enemyChoicedHand = h;

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
		damageCulc ();
    }

    // ダメージ計算
    void damageCulc(){
        float originalDmg;
        int dmg;
        for (int i=0; i<3; i++){
			// 対象のキャラ以外暗くする
			bool[] dark = new bool[3];
			for (int j=0; j<3; j++){
				dark[j] = false;
			}
			dark [i] = true;
			for (int j = 0; j < 3; j++) {
				if (dark [j]) {
					GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				} else {
					GameObject.Find ("Player" + (j + 1).ToString ()).GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f, 1);
				}
			}
			// あいこなら何もしない
            if (WoL[i] == 0){
                break;
            }
            // 攻撃側と防御側のコンポーネント取得
            CharacterSet atkSide;
            CharacterSet defSide;
            int handType;
            if (WoL[i] == 1){
                atkSide = GameObject.Find("Player"+ (i+1).ToString()).GetComponent<CharacterSet>();
                defSide = GameObject.Find("Enemy").GetComponent<CharacterSet>();
                handType = playerChoicedHands[i];
            } else {
                atkSide = GameObject.Find("Enemy").GetComponent<CharacterSet>();
                defSide = GameObject.Find("Player"+ (i+1).ToString()).GetComponent<CharacterSet>();
                handType = enemyChoicedHand;
            }

            // 必要数値の設定
            int atkType = atkSide.type;
            int defType = defSide.type;
            int atk = atkSide.status[handType];
            int pow = atkSide.skillpows[handType];
            int rand = Random.Range(85, 101);
            int def = defSide.status[handType];

            // 基本ダメージ
			originalDmg = atk * atk * pow * rand / (atk + def) / 10000.0f;

            // タイプ一致ボーナス
            if (atkType == handType){
                originalDmg *= 1.2f;
            }

            // 弱点・耐性ボーナス
            int chemi = judgingDeadlock(atkType, defType);
            if (chemi == 1){
                originalDmg *= 1.5f;
            } else if (chemi == 2){
                originalDmg *= 0.8f;
            }

            // 小数点以下切り捨て
            dmg = (int)originalDmg;
			Debug.Log (dmg);

			// HPを減らす
			if (WoL [i] == 1) {
				GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().hp -= dmg;
				GameObject.Find ("EnemyHP").GetComponent<HPbarController> ().barDecrease ();
			} else {
				GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp -= dmg;
				GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().barDecrease ();
			}
        }
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
			// 勝ち
			Debug.Log("You Win!");
		} else if (GameObject.Find ("PlayerHP").GetComponent<HPbarController> ().hp <= 0) {
			// 負け
			Debug.Log("You Lose");
		}
		initHand ();
	}

	// 選んだ手の初期化
	void initHand(){
		for (int i = 0; i < 3; i++) {
			playerChoicedHands [i] = 0;
		}
		enemyChoicedHand = 0;
		GameObject.Find("Text_Center").GetComponent<Text>().text = "";
	}
}