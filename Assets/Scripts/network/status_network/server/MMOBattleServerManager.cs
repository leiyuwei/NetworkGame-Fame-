using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
	public class MMOBattleServerManager : SingleMonoBehaviour<MMOBattleServerManager>
	{
		public MMOServer server;
		public List<BattleTerrain> terrainPrefabs;
		public List<MMOUnit> unitPrefabs;
		public List<ShootObject> shootPrefabs;
		public int terrainIndex = 0;
		BattleTerrain mCurrentBattleTerrain;
		Dictionary<int,MMOUnit> mUnitDic;
		List<MMOUnit> mUnitList;

		List<MMOUnit> mMonsterList;
		UnitInfo[] mMonsterInfos;

		List<MMOUnit> mPlayerUnitList;
		List<UnitInfo> mPlayerInfoList;
		//プーレアが追加された時とか、削除された時とか、このアレイを変更される（こうやってはネット通信するために）
		UnitInfo[] mPlayerInfos;

		int mUnitIndex = 0;

		protected override void Awake ()
		{
			base.Awake ();
			InitTerrain (terrainIndex);
			mUnitDic = new Dictionary<int, MMOUnit> ();
			mUnitList = new List<MMOUnit> ();
			mPlayerUnitList = new List<MMOUnit> ();
			mPlayerInfoList = new List<UnitInfo> ();
			mNextShootTime = Time.time + mShootInterval;
			//TODO
			server.onRecievePlayerMessage = OnPlayerSkill;
		}

		void Start ()
		{
			InitMonsters ();
		}

		float mShootInterval = 0.5f;
		float mNextShootTime;

		void Update ()
		{
			server.UpdateMonsters (mMonsterInfos);
			if (Input.GetKeyDown (KeyCode.F)) {
				MMOUnit monster = mMonsterList [0];
				Shoot (monster);
			}
		}

		void InitTerrain (int terrainIndex)
		{
			terrainIndex = Mathf.Clamp (terrainIndex, 0, terrainPrefabs.Count - 1);
			mCurrentBattleTerrain = terrainPrefabs [terrainIndex];
			GameObject terrainGo = Instantiate (mCurrentBattleTerrain.terrainPrefab) as GameObject;
			terrainGo.transform.position = Vector3.zero;
		}

		MMOUnit InstantiateUnit (int unitType)
		{
			//TODO prefabObject should match the unitType.
			GameObject unitPrebfab = unitPrefabs [0].gameObject;
			unitPrebfab.SetActive (false);
			GameObject unitGo = Instantiate (unitPrebfab) as GameObject;
			unitGo.transform.position = mCurrentBattleTerrain.playerSpawnPosition;
			MUnit mUnit = CSVManager.Instance.GetUnit (unitType);
			MMOUnit mmoUnit = unitGo.GetComponent<MMOUnit> ();
			mmoUnit.unitInfo = new UnitInfo ();
			mmoUnit.unitInfo.attribute.unitId = mUnitIndex;
			mmoUnit.unitInfo.attribute.unitType = unitType;
			mmoUnit.unitInfo.attribute.unitName = mUnit.name;
			mmoUnit.unitInfo.attribute.currentHP = mUnit.max_hp;
			mmoUnit.unitInfo.attribute.maxHP = mUnit.max_hp;
			mUnitList.Add (mmoUnit);
			mUnitDic.Add (mUnitIndex, mmoUnit);
			mUnitIndex++;
			unitGo.SetActive (true);
			return mmoUnit;
		}

		public MMOUnit AddPlayer ()
		{
			MMOUnit mmoUnit = InstantiateUnit (1);
			mPlayerUnitList.Add (mmoUnit);
			mPlayerInfoList.Add (mmoUnit.unitInfo);
			mPlayerInfos = new UnitInfo[mPlayerUnitList.Count];
			for (int i = 0; i < mPlayerUnitList.Count; i++) {
				mPlayerInfos [i] = mPlayerUnitList [i].unitInfo;
			}
			return mmoUnit;
		}
			
		public void OnPlayerSkill(PlayerInfo playerInfo){
			MMOUnit mmoUnit = mUnitDic [playerInfo.unitInfo.attribute.unitId];
			if (mmoUnit.unitInfo.action.attackType > 0) {
				Shoot (mmoUnit);
			}
		}

		void InitMonsters ()
		{
			mMonsterList = new List<MMOUnit> ();
			int monsterCount = 10;
			List<UnitInfo> unitInfoList = new List<UnitInfo>();
			for (int i = 0; i < monsterCount; i++) {
				MMOUnit unit = InstantiateUnit (2);
				mMonsterList.Add (unit);
				UnitInfo info = unit.unitInfo;
				unit.transform.position = new Vector3 (1060.9f + Random.Range (-20f, 20f), 24f, 1320 + Random.Range (-20f, 20f));
				if(i!=0)
					unit.GetComponent<SimpleAI> ().Move (0);
				else
					unit.GetComponent<SimpleAI> ().Move (1);
				unitInfoList.Add(info);
			}

			for (int i = 0; i < monsterCount; i++) {
				MMOUnit unit = InstantiateUnit (1);
				mMonsterList.Add (unit);
				UnitInfo info = unit.unitInfo;
				unit.transform.position = new Vector3 (1148.3f + Random.Range (-20f, 20f), 24f, 1220.9f + Random.Range (-20f, 20f));
				if(i!=0)
					unit.GetComponent<SimpleAI> ().Move (0);
				else
					unit.GetComponent<SimpleAI> ().Move (1);
				unitInfoList.Add(info);
			}
			mMonsterInfos = unitInfoList.ToArray ();
		}

		#region TestCode
		//TODO
		//入れてなんちゃんて
		public void Shoot (MMOUnit monster)
		{
			GameObject shootGo = Instantiater.Spawn (false, shootPrefabs [0].gameObject, monster.transform.position + new Vector3 (0, 1, 0), monster.transform.rotation * Quaternion.Euler (60, 0, 0));
			Vector3 targetPos = monster.transform.position + monster.transform.forward * 40;
			shootGo.GetComponent<ShootProjectileObject> ().Shoot (monster, targetPos, Vector3.zero);
			monster.unitInfo.action.attackType = Random.Range (0, 3);
			monster.unitInfo.action.targetPos = targetPos;
		}
		#endregion

	}

	[System.Serializable]
	public class BattleTerrain
	{
		public Vector3 playerSpawnPosition;
		public GameObject terrainPrefab;
	}

	[System.Serializable]
	public class UnitAttribute
	{
		public int unitId;
		public string unitName;
	}

}