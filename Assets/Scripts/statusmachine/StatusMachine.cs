using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StatusMachines
{
	public class StatusMachine
	{
		string mPreStatus;
		public string mCurrentStatus;
		public GameObject GO;
	 	public List<string> statusList;
		List<StatusAction> mCurrentStatusActions;

		Dictionary<string,List<StatusAction>> mStatusDic;
		Dictionary<string,List<StatusAction>> StatusDic {
			get { 
				if (mStatusDic == null) {
					mStatusDic = new Dictionary<string, List<StatusAction>> ();
					statusList = new List<string> ();
				}
				return mStatusDic;
			}
		}

		public StatusMachine(GameObject go){
			GO = go;
		}

		public void AddStatus (string statusName)
		{
			if (string.IsNullOrEmpty (statusName) || StatusDic.ContainsKey (statusName))
				return;
			StatusDic.Add (statusName, new List<StatusAction> ());
			statusList.Add (statusName);
		}

		public void AddAction (string statusName, StatusAction action)
		{
			if (string.IsNullOrEmpty (statusName) || action == null)
				return;
			AddStatus (statusName);
			StatusDic [statusName].Add (action);
			action.statusMachine = this;
			action.GO = GO;
			action.OnAwake ();
		}

		public void AddActions (string statusName, List<StatusAction> actions)
		{
			if (string.IsNullOrEmpty (statusName) || actions == null)
				return;
			AddStatus (statusName);
			for (int i = 0; i < actions.Count; i++) {
				AddAction (statusName, actions [i]);
			}
		}

		public void ChangeStatus (string status)
		{
			if (string.IsNullOrEmpty (status)) {
				Debug.Log ("<color=red>状態\"" + status + "\"はない</color>");
				return;
			}
			//status あるし、そして今のstatusはparameter-statusじゃない
			if (StatusDic.ContainsKey (status) && mCurrentStatus != status) {
				mPreStatus = mCurrentStatus;
				mCurrentStatus = status;
				mCurrentStatusActions = StatusDic [mCurrentStatus];
				//OnExit
				if (!string.IsNullOrEmpty (mPreStatus) && StatusDic.ContainsKey (mPreStatus)) {
					List<StatusAction> exitActions = StatusDic [mPreStatus];
					for (int i = 0; i < exitActions.Count; i++) {
						exitActions [i].OnExit ();
					}
				}
				//OnEnter
				if (!string.IsNullOrEmpty (mCurrentStatus) && StatusDic.ContainsKey (mCurrentStatus)) {
					List<StatusAction> enterActions = StatusDic [mCurrentStatus];
					for (int i = 0; i < enterActions.Count; i++) {
						enterActions [i].OnEnter ();
					}
				}
			}
		}

		public string GetPreStatus ()
		{
			return mPreStatus;
		}

		public string GetCurrentStatus ()
		{
			return mCurrentStatus;
		}

		public void OnFrameUpdate ()
		{
			if (mCurrentStatusActions != null) {
				for (int i = 0; i < mCurrentStatusActions.Count; i++) {
					mCurrentStatusActions [i].OnUpdate ();
				}
			}
		}
	}
}