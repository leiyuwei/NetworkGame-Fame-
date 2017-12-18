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
		public int terrainIndex = 0;
		BattleTerrain mCurrentBattleTerrain;
		Dictionary<int,MMOUnit> mUnitDic;
		List<MMOUnit> mMonsterList;
		UnitInfo[] mMonsterInfos;
		List<MMOUnit> mUnitList;
		int mUnitIndex = 0;

		protected override void Awake ()
		{
			base.Awake ();
			InitTerrain (terrainIndex);
			mUnitDic = new Dictionary<int, MMOUnit> ();
			mUnitList = new List<MMOUnit> ();
		}

		void Start(){
			InitMonsters ();
		}

		void Update(){
			//TODO
			server.UpdateMonsters (mMonsterInfos);
		}

		void InitTerrain(int terrainIndex){
			terrainIndex = Mathf.Clamp (terrainIndex,0,terrainPrefabs.Count-1);
			mCurrentBattleTerrain = terrainPrefabs [terrainIndex];
			GameObject terrainGo = Instantiate (mCurrentBattleTerrain.terrainPrefab) as GameObject;
			terrainGo.transform.position = Vector3.zero;
		}

		public MMOUnit InitUnit(int unitType){
			unitType = Mathf.Clamp (unitType,0,unitPrefabs.Count-1);
			GameObject unitPrebfab = unitPrefabs[unitType].gameObject;
			unitPrebfab.SetActive (false);
			GameObject unitGo = Instantiate (unitPrebfab) as GameObject;
			unitGo.transform.position = mCurrentBattleTerrain.playerSpawnPosition;
			MUnit mUnit = CSVManager.Instance.GetUnit (unitType);
			MMOUnit mmoUnit = unitGo.GetComponent<MMOUnit> ();
			mmoUnit.unitAttribute.unitId = mUnitIndex;
			mmoUnit.unitAttribute.unitType = unitType;
			mmoUnit.unitAttribute.unitName = mUnit.name;
			mmoUnit.unitAttribute.currentHP = mUnit.max_hp;
			mmoUnit.unitAttribute.maxHP = mUnit.max_hp;
			mUnitList.Add (mmoUnit);
			mUnitDic.Add (mUnitIndex,mmoUnit);
			mUnitIndex++;
			unitGo.SetActive (true);
			return mmoUnit;
		}

		void InitMonsters(){
			mMonsterList = new List<MMOUnit> ();
			int monsterCount = 10;
			mMonsterInfos = new UnitInfo[monsterCount];
			for(int i=0;i<monsterCount;i++){
				MMOUnit unit = InitUnit (1);
				mMonsterList.Add (unit);
				UnitInfo info = new UnitInfo ();
				info.attribute = unit.unitAttribute;
				info.animation = unit.unitAnimation;
				info.transform = unit.unitTransform;
				unit.transform.position = new Vector3 (1060.9f+ Random.Range(-20f,20f), 24f, 1320 + Random.Range(-20f,20f));
				unit.GetComponent<SimpleAI> ().Move ();
				mMonsterInfos[i] = info;
			}
		}


	}

	[System.Serializable]
	public class BattleTerrain{
		public Vector3 playerSpawnPosition;
		public GameObject terrainPrefab;
	}

	[System.Serializable]
	public class UnitAttribute{
		public int unitId;
		public string unitName;
	}

}