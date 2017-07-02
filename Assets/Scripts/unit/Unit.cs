using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusMachines;

public class Unit : MonoBehaviour {

	StatusMachine mFsm;
	UnitRes mUnitRes;

	void Awake(){
		mFsm = new StatusMachine (gameObject);
		mUnitRes = GetComponentInChildren<UnitRes> (true);
		mFsm.AddAction (UnitStatusContant.STANDBY,new UnitStandByAction());
		mFsm.AddAction (UnitStatusContant.MOVE, new UnitMoveAction ());
		mFsm.AddAction (UnitStatusContant.ATTACK, new UnitAttackAction ());
	}

	public void OnFrameUpdate(){
		mFsm.OnFrameUpdate ();
	}

	public void Move(){
		mFsm.ChangeStatus (UnitStatusContant.MOVE);
	}

	public void Attack(){
		mFsm.ChangeStatus (UnitStatusContant.ATTACK);
	}

	public void StandBy(){
		mFsm.ChangeStatus (UnitStatusContant.STANDBY);
	}

	public UnitRes unitRes{
		get{ 
			return mUnitRes;
		}
	}

}
