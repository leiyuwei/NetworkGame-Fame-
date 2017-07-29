using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class PlayerInfo
	{
		public Transform plant;
		public int playerId;
		public int score;
		public Vector3 mousePos;
	}


	public class BattleClientController : SingleMonoBehaviour<BattleClientController>
	{
		public LocalPositionManager localPositionManager;
		public BattleClient battleClient;
		public int playerId;
		public Dictionary<int,PlayerInfo> players;
		public Vector2 defaultPlantPos;
		public Ball ball;
		public Camera gameCamera;

		protected override void Awake ()
		{
			base.Awake ();
			battleClient = GetComponent<BattleClient> ();
			Reset ();
			#if UNITY_STANDALONE
			Screen.SetResolution(540,960,false);
			#endif
		}

		public void Reset ()
		{
			if (players != null) {
				foreach (PlayerInfo pi in players.Values) {
					Destroy (pi.plant.gameObject);
				}
			}
			players = new Dictionary<int, PlayerInfo> ();
			BattleClient.Instance.Reset ();
		}

		int mFrame;

		public void LogicUpdate (ServerMessage sm)
		{
			mFrame = sm.frame;
			UpdatePlayers (sm);
		}

		void UpdatePlayers(ServerMessage sm){
			for(int i=0;i<sm.playerHandles.Length;i++){
				PlayerHandle ph = sm.playerHandles[i];
				PlayerInfo pi = players [ph.playerId];
				pi.mousePos = ph.mousePos;
				pi.plant.position = new Vector2 (gameCamera.ScreenToWorldPoint(pi.mousePos).x,pi.plant.position.y);
			}
		}

		public void CreatePlayers (CreatePlayer cp)
		{
			for (int i = 0; i < cp.playerIds.Length; i++) {
				GameObject prefab = BattleClientResourceManager.Instance.GetPlantPrefab (cp.playerIds[i]);
				GameObject go = Instantiate (prefab, Vector3.zero, Quaternion.identity);
				go.transform.position = defaultPlantPos;
				PlayerInfo pi = new PlayerInfo ();
				pi.playerId = cp.playerIds [i];
				pi.mousePos = defaultPlantPos;
				pi.plant = go.transform;
				players.Add (cp.playerIds [i], pi);
				go.SetActive (true);
			}
		}

		public void Begin(){
			ball.Play ();
			SoundManager.Instance.PlayBGM ();
		}

		public void Stop ()
		{
			BattleClient.Instance.Disconnect ();
			BattleClientUIManager.Instance.OnDisconnected ();
			BattleClientController.Instance.Reset ();
		}

	}
}
