using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusMachines;

public class UnitAttackAction : UnitAction {

	public override void OnEnter ()
	{
		base.OnEnter ();
		this.mUnit.unitRes.PlayAttack ();
	}

	public override void OnUpdate ()
	{
		base.OnUpdate ();
	}

}
