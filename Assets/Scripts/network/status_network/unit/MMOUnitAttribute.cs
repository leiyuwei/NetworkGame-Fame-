using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
	[System.Serializable]
	public class MMOUnitAttribute
	{
		public int unitId;
		public int unitType;
		public string unitName;
		public int currentHP;
		public int maxHP;
	}
}