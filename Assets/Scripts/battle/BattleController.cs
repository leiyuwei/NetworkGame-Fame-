using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	//基于send message的控制机制
	public class BattleController : SingleMonoBehaviour<BattleController>
	{
		public Camera battleCamera;
		public SampleUnit unit;
		BattleClient mBattleClient;

		protected override void Awake ()
		{
			base.Awake ();
			mBattleClient = BattleClient.Instance;
		}

		void Start ()
		{
			if (mBattleClient != null)
				mBattleClient.onFrameUpdate = FrameUpdate;
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.G)) {
				Debug.Log ("KeyCode.G");
				mBattleClient.SendResourceReadyToServer ();
			}

			if (Input.GetMouseButtonDown (0)) {
				RaycastHit hit;
				if (Physics.Raycast (battleCamera.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity, 1 << LayerConstant.LAYER_GROUND)) {
					Debug.Log (hit);
					unit.MoveTo (new Vector2(hit.point.x,hit.point.z));
				}
			}

		}

		public void FrameUpdate (ServerMessage sm)
		{
			Debug.Log (JsonUtility.ToJson (sm));
			for (int i = 0; i < sm.playerHandles.Length; i++) {
				Debug.Log (JsonUtility.ToJson (sm.playerHandles [i]));
			}
		}

	}
}