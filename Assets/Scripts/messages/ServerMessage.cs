using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace MultipleBattle
{

	public class CreatePlayer : MessageBase{
		public int[] playerIds;
	}

	[Serializable]
	public class CachedServerMessage : MessageBase{
		public ServerMessage[] serverMessages;
	}

	[Serializable]
	public class ServerMessage : MessageBase
	{
		public int frame = 1;
		public string desc = "";
		[SerializeField]
		public SpawnGameObject[] spawnObjs;
		[SerializeField]
		public PlayerHandle[] playerHandles;
	}

	[Serializable]
	public class SpawnGameObject : MessageBase
	{
		public int id;
		public int objType;
		public int actionType;
		public Vector3 pos;
		public Quaternion qua;
	}

	//key: 0=A,1=S,2=D,3=W,4=J,
	//keyStatus: false=up,true=down;
	[Serializable]
	public class PlayerHandle : MessageBase
	{
		public int playerId;
		public int key = 0;
		public bool keyStatus = false;
	}

	public class PlayerStatusArray:MessageBase {
		public PlayerStatus[] playerStatus;
	}

	public class PlayerStatus : MessageBase{
		public int playerId;
		public bool isReady;
	}

}