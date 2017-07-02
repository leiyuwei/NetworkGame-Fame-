using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class BattlePlayer : SingleMonoBehaviour<BattlePlayer>
	{
		ETCJoystick mJoystick;

		List<KeyCode> mHandleKeyCodes;

		void Awake ()
		{
			mHandleKeyCodes = new List<KeyCode> ();
			mHandleKeyCodes.Add (KeyCode.A);
			mHandleKeyCodes.Add (KeyCode.S);
			mHandleKeyCodes.Add (KeyCode.D);
			mHandleKeyCodes.Add (KeyCode.W);
			mHandleKeyCodes.Add (KeyCode.F);
			mHandleKeyCodes.Add (KeyCode.J);
			mJoystick = FindObjectOfType<ETCJoystick> ();
			#if UNITY_EDITOR
			mJoystick.transform.parent.gameObject.SetActive(false);
			#endif
		}

		void OnKeyDown(KeyCode keyCode){
			OnHandled ((int)keyCode,true);
		}

		void OnKeyUp(KeyCode keyCode){
			OnHandled ((int)keyCode,false);
		}

		void Update ()
		{
			for(int i=0;i<mHandleKeyCodes.Count;i++){
				if(Input.GetKeyDown(mHandleKeyCodes[i])){
					OnKeyDown (mHandleKeyCodes[i]);
				}
				if(Input.GetKeyUp(mHandleKeyCodes[i])){
					OnKeyUp (mHandleKeyCodes[i]);
				}
			}

			if (Input.GetKeyUp (KeyCode.Q)) {
				Application.Quit ();
			}

			#if !UNITY_EDITOR
			if (mJoystick != null) {
				ETCDPadUpdate ();
			}
			#endif
		}

		int mPreAxisXValue;
		int mPreAxisYValue;

		void ETCDPadUpdate ()
		{
			int x = Mathf.RoundToInt (Mathf.Abs(mJoystick.axisX.axisValue * 2));
			if (mJoystick.axisX.axisValue < 0)
				x = -x;
			int y = Mathf.RoundToInt (Mathf.Abs(mJoystick.axisY.axisValue * 2));
			if (mJoystick.axisY.axisValue < 0)
				y = -y;
			Debug.Log (x + "||" + y);
			if (x != mPreAxisXValue) {
				if (x == 1 || x == -1 || x == 0) {
					if (mPreAxisXValue == 1) {
						OnKeyUp (KeyCode.D);
					} else if (mPreAxisXValue == -1) {
						OnKeyUp (KeyCode.A);
					}

					if(x == 1){
						OnKeyDown (KeyCode.D);
					}else if(x == -1){
						OnKeyDown (KeyCode.A);
					}
					mPreAxisXValue = x;
				}
			}
			if (y != mPreAxisYValue) {
				if (y == 1 || y == -1 || y == 0) {
					if (mPreAxisYValue == 1) {
						OnKeyUp (KeyCode.W);
					} else if (mPreAxisYValue == -1) {
						OnKeyUp (KeyCode.S);
					}

					if(y == 1){
						OnKeyDown (KeyCode.W);
					}else if(y == -1){
						OnKeyDown (KeyCode.S);
					}
					mPreAxisYValue = y;
				}
			}
		}

		void OnHandled (int key, bool handle)
		{
			PlayerHandle ph = new PlayerHandle ();
			ph.key = key;
			ph.keyStatus = handle;
			BattleClientController.GetInstance ().battleClient.SendPlayerHandle (ph);
		}

		public void OnFire ()
		{
			OnHandled ((int)KeyCode.F, true);
		}

		public void OnJump(){
			OnHandled ((int)KeyCode.U, true);
		}

	}
}