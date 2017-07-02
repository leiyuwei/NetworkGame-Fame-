using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class BattleServerController : SingleMonoBehaviour<BattleServerController>
	{

		public GameObject battlePrefab;
		[HideInInspector]
		public GameObject battleObjects;
		[HideInInspector]
		public LocalPositionManager localPositionManager;

		public GameObject playerPrefab;
		public GameObject bulletPrefab;

		public Dictionary<int,GameObject> playerObjects;
		public Dictionary<int,GameObject> bulletObjects;

		BattleServer mBattleServer;

		bool mIsBattleBegin;

		protected override void Awake ()
		{
			base.Awake ();
//			battleObjects = Instantiate (battlePrefab, Vector3.zero, Quaternion.identity);
//			battleObjects.GetComponentInChildren<Camera> (true).enabled = false;
//			localPositionManager = battleObjects.GetComponent<LocalPositionManager> ();
			mBattleServer = GetComponent<BattleServer> ();
		}

		public void BattleBegin(){
			mIsBattleBegin = true;
//			CreatePlayerObject (GetObjectID(),localPositionManager.leftPosList[0].position,localPositionManager.leftPosList[0].rotation);
		}

//		public void CreatePlayerObject (int id, Vector3 pos, Quaternion qua)
//		{
//			SpawnGameObject sgo = new SpawnGameObject ();
//			sgo.id = GetInstanceID ();
//			sgo.actionType = ActionConstant.ACITON_CREATE_PLAYER;
//			sgo.pos = pos;
//			sgo.qua = qua;
//			mBattleServer.spawnObjectList.Add (sgo);
//		}

		public void MovePlayerObject (int id, Vector3 direct, float speed)
		{
	
		}

		public void StopPlayerObject (int id)
		{
	
		}

		public void Shoot (int id, Vector3 direct, float speed)
		{
	
		}

		public void SpawnBullet (int id, Vector3 direct, float speed)
		{
	
		}

		public void Hit (int buttetId, int targetId)
		{
		
		}

		int mCurrentId;

		public int GetObjectID ()
		{
			mCurrentId++;
			return mCurrentId;
		}

	}

}