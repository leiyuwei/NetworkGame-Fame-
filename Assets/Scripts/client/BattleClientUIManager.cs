using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultipleBattle
{
	public class BattleClientUIManager : SingleMonoBehaviour<BattleClientUIManager>
	{

		public InputField input_ip;
		public InputField input_port;
		public Button btn_connect;
		public Button btn_ready;

		public Button btn_replay;
		public Button btn_save;
		public Button btn_disconnect;
//		public GridLayoutGroup grid_saves;
//		public GameObject grid_save_item_prefab;
//		public List<GameObject> grid_save_items;

		public GridLayoutGroup grid_players;
		public GameObject grid_players_item_prefab;
		public List<GameObject> grid_players_items;
		public Dictionary<int,GameObject> grid_players_items_dic;

		void Start ()
		{
			if (PlayerPrefs.HasKey ("SERVER_IP"))
				input_ip.text = PlayerPrefs.GetString ("SERVER_IP");
			else
				input_ip.text = BattleClient.GetInstance ().defaultIP;
			if (PlayerPrefs.HasKey ("SERVER_PORT"))
				input_port.text = PlayerPrefs.GetString ("SERVER_PORT");
			else
				input_port.text = BattleClient.GetInstance ().defaultPort.ToString ();
			btn_connect.onClick.AddListener (Connect);
			btn_ready.onClick.AddListener (SendReadyToServer);
			btn_save.onClick.AddListener (Save);
			btn_replay.onClick.AddListener (Replay);
			btn_disconnect.onClick.AddListener (Stop);
			grid_players_items = new List<GameObject> ();
			grid_players_items_dic = new Dictionary<int, GameObject> ();
		}

		void Connect ()
		{
			BattleClient.GetInstance ().Connect (input_ip.text, int.Parse (input_port.text));
			input_ip.gameObject.SetActive (false);
			input_port.gameObject.SetActive (false);
			btn_connect.gameObject.SetActive (false);
			this.btn_replay.gameObject.SetActive (false);
		}

		void Save(){
			BattleClientReplayManager.GetInstance ().SaveRecord ();
		}

		void Replay(){
			BattleClientReplayManager.GetInstance ().Replay ();
			btn_replay.gameObject.SetActive (false);
			input_ip.gameObject.SetActive (false);
			input_port.gameObject.SetActive (false);
			btn_connect.gameObject.SetActive (false);
		}

		void Stop(){
			BattleClientController.GetInstance ().Stop ();
		}

		void SendReadyToServer(){
			BattleClient.GetInstance ().SendReadyToServer ();
			btn_ready.gameObject.SetActive (false);
		}

		public void OnConnected(){
			PlayerPrefs.SetString ("SERVER_IP",input_ip.text);
			PlayerPrefs.SetString ("SERVER_PORT",input_port.text);
			btn_ready.gameObject.SetActive (true);
			btn_disconnect.gameObject.SetActive (true);
		}

		public void OnDisconnected(){
			input_ip.gameObject.SetActive (true);
			input_port.gameObject.SetActive (true);
			btn_connect.gameObject.SetActive (true);
			btn_ready.gameObject.SetActive (false);
			btn_replay.gameObject.SetActive (true);
			btn_disconnect.gameObject.SetActive (false);
			btn_save.gameObject.SetActive (false);
		}

		public void OnBattleBegin(){
			btn_save.gameObject.SetActive (true);
		}

		public void OnPlayerStatus(PlayerStatusArray psa){
			for(int i=0;i<grid_players_items.Count;i++){
				Destroy (grid_players_items[i]);
			}
			grid_players_items.Clear ();
			grid_players_items_dic.Clear ();
			Debug.Log (psa.playerStatus.Length);
			for(int i=0;i<psa.playerStatus.Length;i++){
				GameObject go = Instantiate<GameObject>(grid_players_item_prefab);
				go.transform.Find ("txt_name").GetComponent<Text>().text = psa.playerStatus[i].playerId.ToString();
				go.transform.Find ("txt_isready").GetComponent<Text> ().text = psa.playerStatus [i].isReady.ToString();
				go.transform.SetParent (grid_players.transform);
				go.SetActive (true);
				grid_players_items.Add (go);
				grid_players_items_dic.Add (psa.playerStatus[i].playerId,go);
			}
		}

		public void SetPlayerScore(PlayerInfo playerInfo){
			GameObject item;
			if (grid_players_items_dic.TryGetValue (playerInfo.playerId, out item)) {
				Text txt_score = item.transform.Find ("txt_score").GetComponent<Text> ();
				txt_score.text = playerInfo.score.ToString ();
			}
		}

	}
}
