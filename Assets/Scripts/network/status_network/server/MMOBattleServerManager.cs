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
		}

		void Start ()
		{
			InitMonsters ();
		}

		float mShootInterval = 0.5f;
		float mNextShootTime;

		void Update ()
		{
			//TODO
			server.UpdateMonsters (mMonsterInfos);

//			if(mNextShootTime < Time.time){
//				mNextShootTime = Time.time + mShootInterval;
//				MMOUnit monster = mMonsterList[Random.Range(0,mMonsterList.Count)];
//				Shoot ( monster );
//			}

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

		MMOUnit InitUnit (int unitType)
		{
			unitType = Mathf.Clamp (unitType, 0, unitPrefabs.Count - 1);
			GameObject unitPrebfab = unitPrefabs [unitType].gameObject;
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
			MMOUnit mmoUnit = InitUnit (1);
			mPlayerUnitList.Add (mmoUnit);
			mPlayerInfoList.Add (mmoUnit.unitInfo);
			mPlayerInfos = new UnitInfo[mPlayerUnitList.Count];
			for (int i = 0; i < mPlayerUnitList.Count; i++) {
				mPlayerInfos [i] = mPlayerUnitList [i].unitInfo;
			}
			return mmoUnit;
		}

		void InitMonsters ()
		{
			mMonsterList = new List<MMOUnit> ();
			int monsterCount = 10;
			mMonsterInfos = new UnitInfo[monsterCount];
			for (int i = 0; i < monsterCount; i++) {
				MMOUnit unit = InitUnit (1);
				mMonsterList.Add (unit);
				UnitInfo info = unit.unitInfo;
				unit.transform.position = new Vector3 (1060.9f + Random.Range (-20f, 20f), 24f, 1320 + Random.Range (-20f, 20f));
				unit.GetComponent<SimpleAI> ().Move ();
				mMonsterInfos [i] = info;
			}
		}

		#region TestCode

		public void Shoot (MMOUnit monster)
		{
			GameObject shootGo = Instantiater.Spawn (false, shootPrefabs [0].gameObject, monster.transform.position + new Vector3 (0, 1, 0), monster.transform.rotation * Quaternion.Euler (60, 0, 0));
			Vector3 targetPos = monster.transform.position + monster.transform.forward * 40;
			shootGo.GetComponent<ShootProjectileObject> ().Shoot (monster, targetPos, Vector3.zero);
			monster.unitInfo.attack.attackType = Random.Range (0, 3);
			monster.unitInfo.attack.targetPos = targetPos;
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