using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MMO
{
	[System.Serializable]
	public class MMOUnitAttribute:MessageBase
	{
		public int unitId;
		public int unitType;
		public string unitName;
		public int currentHP;
		public int maxHP;
	}
}