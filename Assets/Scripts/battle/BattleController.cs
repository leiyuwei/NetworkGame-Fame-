using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	//基于send message的控制机制
	public class BattleController : SingleMonoBehaviour<BattleController>
	{

		BattleClient mBattleClient;
		float mDelayBegin = 3f;

		protected override void Awake ()
		{
			base.Awake ();
			mBattleClient = BattleClient.Instance;
		}

		void Start(){
			if(mBattleClient!=null)
				mBattleClient.onFrameUpdate = FrameUpdate;
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.G)) {
				Debug.Log ("KeyCode.G");
				mBattleClient.SendResourceReadyToServer ();
			}
		}

		public void FrameUpdate(ServerMessage sm){
			Debug.Log (JsonUtility.ToJson(sm));
			for(int i=0;i<sm.playerHandles.Length;i++){
				Debug.Log (JsonUtility.ToJson(sm.playerHandles[i]));
			}
		}

	}
}