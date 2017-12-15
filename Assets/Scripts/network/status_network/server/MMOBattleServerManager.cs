using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
	public class MMOBattleServerManager : SingleMonoBehaviour<MMOBattleServerManager>
	{

		public List<BattleTerrain> terrainPrefabs;
		public List<MMOUnit> unitPrefabs;
		public int terrainIndex = 0;
		BattleTerrain mCurrentBattleTerrain;
		Dictionary<int,MMOUnit> mUnitDic;
		List<MMOUnit> mUnitList;
		int mUnitIndex = 0;

		protected override void Awake ()
		{
			base.Awake ();
			InitTerrain (terrainIndex);
			mUnitDic = new Dictionary<int, MMOUnit> ();
			mUnitList = new List<MMOUnit> ();
		}

		void InitTerrain(int terrainIndex){
			terrainIndex = Mathf.Clamp (terrainIndex,0,terrainPrefabs.Count-1);
			mCurrentBattleTerrain = terrainPrefabs [terrainIndex];
			GameObject terrainGo = Instantiate (mCurrentBattleTerrain.terrainPrefab) as GameObject;
			terrainGo.transform.position = Vector3.zero;
		}

		public MMOUnitAttribute InitUnit(int unitType){
			unitType = Mathf.Clamp (unitType,0,unitPrefabs.Count-1);
			GameObject unitPrebfab = unitPrefabs[unitType].gameObject;
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
			return mmoUnit.unitAttribute;
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