using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSet : MonoBehaviour {

	// 外部から設定する
	public bool ifPlayer;
	public int personalMonsterID;

	public int monsterID;

	// 各種ステータス
	public string monsterName;
	public int type;
	public int lv;
	public int maxHp;
	public int[] status = new int[4];
	public int agi;

	// 技の設定
	public string[] skillNames = new string[4];
	public int[] skillpows = new int[4];

	// ボイスの設定
	public AudioClip[] voice = new AudioClip[10];

	// アクティブゲージ
	public GameObject activeGage;
	public float gageSpeed;

	void Start(){
		// CSVReader取得
		CSVReader csvReader = GameObject.Find("CSVReader").GetComponent<CSVReader>();

		// PlayerMonsterもしくはEnemyMonsterからplayerMonsterIDの行だけを配列として取得
		string[] personalMonsterDatas;
		if (ifPlayer){
			personalMonsterDatas = csvReader.CSVReadLine("PlayerMonster", personalMonsterID);
		} else {
			personalMonsterDatas = csvReader.CSVReadLine("EnemyMonster", personalMonsterID);
		}

		// レベルを更新
		lv = int.Parse(personalMonsterDatas[2]);

		// MonsterからMonsterIDの行を配列として取得
		monsterID = int.Parse(personalMonsterDatas[1]);
		string[] monsterDatas = csvReader.CSVReadLine("Monster", int.Parse(personalMonsterDatas[1]));

		// monsterDatasとpersonalMonsterDatasからステータスを算出
		monsterName = monsterDatas[1];
		type = int.Parse(monsterDatas[2]);
		maxHp = int.Parse(monsterDatas[3]) + int.Parse(monsterDatas[3])*(lv-1)*2/5 + int.Parse(personalMonsterDatas[3]);
		for (int i = 1; i < 4; i++) {
			status [i] = int.Parse (monsterDatas [i + 3]) + int.Parse (monsterDatas [i + 3]) * (lv - 1) * 2 / 5 + int.Parse (personalMonsterDatas [i + 3]);
		}
		status [0] = status [1] + status [2] + status [3];
		agi = int.Parse(monsterDatas[7]) + int.Parse(monsterDatas[7])*(lv-1)*2/5 + int.Parse(personalMonsterDatas[7]);

		// Skillから技データを取得し格納
		for (int i=0; i<4; i++){
			string[] skillDatas = csvReader.CSVReadLine("Skill", int.Parse(personalMonsterDatas[i+7]));
			skillNames[i] = skillDatas[1];
			skillpows[i] = int.Parse(skillDatas[3]);
		}

		// 画像をフォルダから探して設定
		Texture2D texture = Resources.Load("images/" + personalMonsterDatas[1]) as Texture2D;
		GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);

		// ボイスをフォルダから探して設定
		for (int i=0; i<10; i++){
			voice[i] = Resources.Load("Voices/"+personalMonsterDatas[1]+"_"+i.ToString()) as AudioClip;
		}

		// アクティブゲージを作成
		activeGage = Instantiate((GameObject)Resources.Load ("Prefabs/ActiveGage"), new Vector2(0, -200), Quaternion.identity);
		activeGage.transform.SetParent (GameObject.Find ("ActiveGageField").transform, false);

		// ゲージ速度設定
		gageSpeed = agi / 160.0f;
	}

	void Update(){
		if (activeGage.GetComponent<Slider> ().value >= activeGage.GetComponent<Slider> ().maxValue) {
			// アクション中にする
			GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().isActionNow = true;
			// アクション処理

			// ゲージを0にする
			activeGage.GetComponent<Slider> ().value = 0;

			// アクション中を解除
			GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().isActionNow = false;

			// 一旦終了
			return null;
		}
		if (GameObject.Find ("ButtleManager").GetComponent<ButtleManager_vsNPC> ().isActionNow) {
			// 何もしない
			return null;
		} else {
			// アクティブゲージを進める
			activeGage.GetComponent<Slider> ().value += gageSpeed;
		}
	}
}
