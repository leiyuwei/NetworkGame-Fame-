using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace MultipleBattle
{
	public class BattleClient : SingleMonoBehaviour<BattleClient>
	{
		public string defaultIP;
		public int defaultPort;
		public bool isBattleBegin;
		NetworkClient client;
		//一時受信したのフラームを保存される処（临时保存收到的帧）
		Dictionary<int,ServerMessage> mCachedMessages;
		//按序储存的帧,这个的作用在于，当网络出现异常，或者机器出现异常，
		//挤压在这里有多帧的话，可以加快客户端游戏逻辑来追上实际进度。
		Dictionary<int,ServerMessage> mRunableMessages;
		List<ServerMessage> mCachedMessageList;


		//TODO:重新同步数据。（1.请求同步，2.服务器获取主机数据，3.同步，4.补帧）
		//同步对服务器和主机的压力可能会比较大，必要的場合同步时可以暂停游戏，等待数据发送过后再继续进行。
		//考虑到战斗服务器上可能同时运行上百场战斗，为了不影响其他战斗的进行，可以单独一台服务器作为同步服务器。
		void Start ()
		{
			mCachedMessages = new Dictionary<int, ServerMessage> ();
			mRunableMessages = new Dictionary<int, ServerMessage> ();
			mCachedMessageList = new List<ServerMessage> ();
			client = new NetworkClient ();
			client.RegisterHandler (MessageConstant.SERVER_TO_CLIENT_MSG, OnFrameMessage);
			client.RegisterHandler (MessageConstant.SERVER_TO_CLIENT_LIST_MSG, OnFrameMessages);
			client.RegisterHandler (MessageConstant.SERVER_CLIENT_STATUS, OnPlayerStatus);
			client.RegisterHandler (MessageConstant.CLIENT_READY, OnCreatePlayer);
			client.RegisterHandler (MsgType.Connect, OnConnected);
			client.RegisterHandler (MsgType.Disconnect, OnDisconnect);
		}

		float mStartTime;
		bool mTestStop = false;

		public void Reset ()
		{
			mCurrentServerMessage = null;
			mFrame = 0;
			mMaxRunableFrame = 0;
			mMaxFrame = 0;
			mRecievedFrameCount = 0;
			mStartTime = 0;
		}

		int mRecievedFrameCount = 0;
		//当前执行中的关键帧
		ServerMessage mCurrentServerMessage;
		//当前执行的关键帧番号
		public int mFrame = 0;
		//可执行的最大帧番号
		int mMaxRunableFrame = 0;
		//接收到到最大帧番号
		int mMaxFrame = 0;
		public const float mMaxFrameWaitingTime = 0.5f;

		//服务端30FPS的发送频率。客户端60FPS的执行频率。
		//也就是说服务器1帧客户端2帧
		void FixedUpdate ()
		{
			#region 模拟网络阻塞
			if (Input.GetKeyDown (KeyCode.N)) {
				mTestStop = true;
			}
			if (Input.GetKeyDown (KeyCode.M)) {
				mTestStop = false;
			}
			if (mTestStop) {
				return;
			}
			#endregion
			if (!isBattleBegin)
				return;
			//論理が実行する(执行逻辑)
			if (mPhysicFrameRemain == 0) {
				UpdateFrame ();
			}
			//物理フレームを実行する(执行物理帧)
			if (mPhysicFrameRemain > 0) {
				mPhysicFrameRemain--;
				UpdateFixedFrame ();
			}
		}

		int mPhysicFrameRemain = 0;
		float mLastFrameTime;

		void UpdateFrame ()
		{
			if (mRunableMessages.ContainsKey (mFrame)) {
				mCurrentServerMessage = mRunableMessages [mFrame];
				if (mCurrentServerMessage.playerHandles.Length > 0)
					RecordMessage (mCurrentServerMessage);
				mRunableMessages.Remove (mFrame);
				BattleClientController.Instance.UpdateFrame (mCurrentServerMessage);
				mFrame++;
				BattleClientUIManager.Instance.txt_frame1.text = mFrame.ToString ();
				mPhysicFrameRemain = 33;
				Time.timeScale = 1;
			} else {
//				Debug.Log (mPhysicFrameRemain);
				Time.timeScale = 0;
			}
		}

		void UpdateFixedFrame(){
			BattleClientController.Instance.UpdateFixedFrame ();
		}

		#region 1.Send

		public void Connect (string ip, int port)
		{
			client.Connect (ip, port);
		}

		public void Disconnect ()
		{
			client.Disconnect ();
		}

		public void SendReadyToServer ()
		{
			Debug.Log ("SendReadyToServer");
			ClientMessage cm = new ClientMessage ();
			cm.clientReady = true;
			if (client.isConnected)
				client.Send (MessageConstant.CLIENT_READY, cm);
		}

		public void SendPlayerHandle (PlayerHandle ph)
		{
			if (client.isConnected && isBattleBegin) {
				client.Send (MessageConstant.CLIENT_PLAYER_HANDLE, ph);
			}
		}

		#endregion

		#region 2.Recieve

		void OnConnected (NetworkMessage nm)
		{
			Debuger.Log ("<color=green>Connect</color>");
			//			Debug.Log (BattleClientController.Instance.playerId);
			BattleClientController.Instance.playerId = nm.conn.connectionId;
			BattleClientUIManager.Instance.OnConnected ();
		}

		void OnDisconnect (NetworkMessage nm)
		{
			Debuger.Log ("<color=red>Disconnect</color>");
			BattleClientUIManager.Instance.OnDisconnected ();
			BattleClientController.Instance.Reset ();
		}

		void OnPlayerStatus (NetworkMessage nm)
		{
			Debug.Log ("<color=greed>OnPlayerStatus</color>");
			PlayerStatusArray psa = nm.ReadMessage<PlayerStatusArray> ();
			BattleClientUIManager.Instance.OnPlayerStatus (psa);
		}

		void OnCreatePlayer (NetworkMessage nm)
		{
			Debug.Log ("OnCreatePlayer");
			CreatePlayer cp = nm.ReadMessage<CreatePlayer> ();
			BattleClientController.Instance.CreatePlayers (cp);
			BattleClientController.Instance.Begin ();
			BattleClientUIManager.Instance.OnBattleBegin ();
			isBattleBegin = true;
		}

		//得到丢失的帧
		LostFrameIdsMessage GetLostFrameIds ()
		{
			LostFrameIdsMessage messages = new LostFrameIdsMessage ();
//			List<LostFrameIdAToBMessage> abMessages = new List<LostFrameIdAToBMessage> ();
//			int fromFrame = 0;
//			int toFrame = 0;
//			for(int i = mFrame;i< mMaxFrame;i++){
//				if (!mCachedMessages.ContainsKey (i)) {
//					LostFrameIdAToBMessage abMessage = new LostFrameIdAToBMessage ();
//					abMessage.fromFrame = i;
//					abMessage.toFrame = i;
//					while(true){
//						i++;
//						if(i > mMaxFrameGetInstance ()）
//							break;
//						}
//						if (!mCachedMessages.ContainsKey (i)) {
//							abMessage.toFrame = i;
//						} else {
//							break;
//						}
//					}
//					abMessages.Add (abMessage);
//				}
//			}
//			messages.frameAToB = abMessages.ToArray ();
			return messages;
		}

		void OnFrameMessage (NetworkMessage mb)
		{
			if (mStartTime == 0)
				mStartTime = Time.realtimeSinceStartup;
			mRecievedFrameCount++;
			ServerMessage sm = mb.ReadMessage<ServerMessage> ();
			AddServerMessage (sm);
			while (mCachedMessages.Count > 0 && mCachedMessages.ContainsKey (mMaxRunableFrame)) {
				mRunableMessages.Add (mMaxRunableFrame, mCachedMessages [mMaxRunableFrame]);
				mCachedMessages.Remove (mMaxRunableFrame);
				mMaxRunableFrame++;
				Time.timeScale = 1;
			}
		}

		void OnFrameMessages (NetworkMessage mb)
		{
			if (mStartTime == 0)
				mStartTime = Time.realtimeSinceStartup;
			mRecievedFrameCount++;
			Debug.Log (((mRecievedFrameCount - 1) / (Time.realtimeSinceStartup - mStartTime)).ToString ());
			CachedServerMessage cachedServerMessage = mb.ReadMessage<CachedServerMessage> ();
			for (int i = 0; i < cachedServerMessage.serverMessages.Length; i++) {
				AddServerMessage (cachedServerMessage.serverMessages [i]);
			}
		}

		public void AddServerMessage (ServerMessage tm)
		{
			//丢弃重复帧（防止受到数据被截获后的重复发送攻击）
			if (!mCachedMessages.ContainsKey (tm.frame)) {
				mCachedMessages.Add (tm.frame, tm);
				if (mMaxFrame < tm.frame) {
					mMaxFrame = tm.frame;
				}
				//ordered list
				for (int i = mCachedMessageList.Count - 1; i > 0; i--) {
					if (mCachedMessageList [i].frame < tm.frame) {
						mCachedMessageList.Insert (i, tm);
						break;
					}
				}
			}
		}

		//记录操作便于回放
		void RecordMessage (ServerMessage tm)
		{
//			BattleClientReplayManager.Instance.record.records.Add (tm);
		}

		#endregion
	}

	[Serializable]
	public class RecordMessage
	{
		public int[] playerIds = new int[0];
		public List<ServerMessage> records = new List<ServerMessage> ();
	}


	[System.Serializable]
	public class PlayerKeys
	{
		public bool KeyA;
		public bool KeyS;
		public bool KeyD;
		public bool KeyW;
	}

}
