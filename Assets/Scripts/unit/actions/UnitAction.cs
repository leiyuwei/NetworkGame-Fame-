using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusMachines;

public class UnitAction : StatusAction {

	protected Unit mUnit;

	public override void OnAwake ()
	{
		base.OnAwake ();
		mUnit = GO.GetComponent<Unit> ();
	}

}
