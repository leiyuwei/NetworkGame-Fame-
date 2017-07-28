using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class PlayerInfo
	{
		public GameObject playerGo;
		public Unit unit;
		public int playerId;
		public Vector3 direction;
		public bool isShoot;
		public bool isJump;
		public int score;
		public Dictionary<KeyCode,bool> playerKeys = new Dictionary<KeyCode, bool> ();
	}


	public class BattleClientController : SingleMonoBehaviour<BattleClientController>
	{
		public GameObject battlePrefab;
		[HideInInspector]
		public GameObject battleObjects;
		[HideInInspector]
		public LocalPositionManager localPositionManager;
		//		public GameObject playerPrefab;
		public GameObject bulletPrefab;
		public List<BattleBullet> bullets;
		public BattleClient battleClient;

		public int playerId;
		public Dictionary<int,PlayerInfo> players;

		public bool useArtResource = false;
		//		public PlayerKeys mPlayerKeys;

		protected override void Awake ()
		{
			base.Awake ();
			battleObjects = Instantiate (battlePrefab, Vector3.zero, Quaternion.identity);
			localPositionManager = battleObjects.GetComponent<LocalPositionManager> ();
			battleClient = GetComponent<BattleClient> ();
			Reset ();
		}

		public void Reset ()
		{
			if (players != null) {
				foreach (PlayerInfo pi in players.Values) {
					Destroy (pi.playerGo);
				}
			}
			players = new Dictionary<int, PlayerInfo> ();
			if (bullets != null) {
				foreach (BattleBullet bb in bullets) {
					Destroy (bb.bullet);
				}
			}
			bullets = new List<BattleBullet> ();
			BattleClient.GetInstance ().Reset ();
		}

		float mMoveSpeed = 0.066f;
		float mBulletSpeed = 0.2f;
		int mFrame;


		public void LogicUpdate (ServerMessage sm)
		{
			mFrame = sm.frame;
			UpdatePlayers (sm);
			UpdateBullets ();
		}

		void UpdatePlayers(ServerMessage sm){
			foreach (PlayerHandle ph in sm.playerHandles) {
				PlayerInfo pi = players [ph.playerId];
				Vector3 direction = pi.direction;
				KeyCode key = (KeyCode)ph.key;
				if (!pi.playerKeys.ContainsKey (key)) {
					pi.playerKeys.Add (key, ph.keyStatus);
				} else {
					pi.playerKeys [key] = ph.keyStatus;
				}
			}

			//Cover user handle from server data.
			//サーバーからユーザーの手顺
			foreach (PlayerInfo pi in players.Values) {
				int x = 0, z = 0;
				foreach (KeyCode key in pi.playerKeys.Keys) {
					if(!pi.playerKeys[key])
					{
						continue;
					}
					switch (key) {
					case KeyCode.A:
						x -= 1;
						break;
					case KeyCode.D:
						x += 1;
						break;
					case KeyCode.S:
						z -= 1;
						break;
					case KeyCode.W:
						z += 1;
						break;
					case KeyCode.F:
						pi.isShoot = true;
						break;
					case KeyCode.J:
						pi.isJump = true;
						break;
					}
				}
				Vector3 direction = new Vector3 (x, 0, z);
				pi.direction = direction;
			}

			foreach (PlayerInfo pi in players.Values) {
				if (pi.direction != Vector3.zero) {
					pi.unit.Move ();
					pi.playerGo.transform.forward = pi.direction;
					pi.playerGo.transform.position += pi.direction.normalized * mMoveSpeed;
				} else {
					pi.unit.StandBy ();
				}
				if (pi.isShoot)
					bullets.Add (CreateBullet (pi.playerId));
				pi.isShoot = false;
			}
		}


		BattleBullet CreateBullet (int id)
		{
			BattleBullet bb = new BattleBullet ();
			bb.destoryFrame = mFrame + 300;
			GameObject go = Instantiate<GameObject> (this.bulletPrefab);
			go.transform.position = players [id].unit.unitRes.weaponPoint;
			go.transform.forward = players [id].playerGo.transform.forward;
			bb.bullet = go;
			bb.playerId = id;
			return bb;
		}

		float mMinBulletHidDistance = 1f;

		void UpdateBullets ()
		{
			for (int i = 0; i < bullets.Count; i++) {
				if (bullets [i].destoryFrame <= mFrame) {
					Destroy (bullets [i].bullet);
					bullets.RemoveAt (i);
					i--;
				} else {
					bullets [i].bullet.transform.position += bullets [i].bullet.transform.forward * mBulletSpeed;
					foreach (PlayerInfo pi in players.Values) {
						if (pi.playerId != bullets [i].playerId) {
							if (Vector3.Distance (pi.playerGo.transform.position, bullets [i].bullet.transform.position) <= mMinBulletHidDistance) {
								Debug.Log ("HIT");
								PlayerInfo attacker;
								if (players.TryGetValue (bullets [i].playerId, out attacker)) {
									attacker.score++;
									BattleClientUIManager.GetInstance ().SetPlayerScore (attacker);
								}

								Destroy (bullets [i].bullet);
								bullets.RemoveAt (i);
								i--;
								break;
							}
						}
					}
				}
			}
		}

		public void CreatePlayers (CreatePlayer cp)
		{
			for (int i = 0; i < cp.playerIds.Length; i++) {
				GameObject prefab = BattleClientResourceManager.GetInstance ().basePrefab;
				if (useArtResource) {
					int index = cp.playerIds [i];
					index = Mathf.Clamp (index, 0, BattleClientResourceManager.GetInstance ().prefabs.Count - 1);
					prefab = BattleClientResourceManager.GetInstance ().prefabs [index];
				}
				GameObject go = Instantiate (prefab, Vector3.zero, Quaternion.identity);
				PlayerInfo pi = new PlayerInfo ();
				pi.playerId = cp.playerIds [i];
				pi.playerGo = go;
				pi.unit = go.GetComponent<Unit> ();
				if (pi.unit == null)
					pi.unit = go.AddComponent<Unit> ();
				players.Add (cp.playerIds [i], pi);
			}
		}

		public void Stop ()
		{
			BattleClient.GetInstance ().Disconnect ();
			BattleClientUIManager.GetInstance ().OnDisconnected ();
			BattleClientController.GetInstance ().Reset ();
		}

	}
}
