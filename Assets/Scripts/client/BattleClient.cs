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
		//缓存收到的帧
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
//			var config = new ConnectionConfig();
//			config.ConnectTimeout = 1000;
//			client.Configure (config, 2);
			client.RegisterHandler (MessageConstant.SERVER_TO_CLIENT_MSG, OnFrameMessage);
			client.RegisterHandler (MessageConstant.SERVER_TO_CLIENT_LIST_MSG, OnFrameMessages);
			client.RegisterHandler (MessageConstant.SERVER_CLIENT_STATUS, OnPlayerStatus);
			client.RegisterHandler (MessageConstant.CLIENT_READY, OnCreatePlayer);
			client.RegisterHandler (MsgType.Connect, OnConnected);
			client.RegisterHandler (MsgType.Disconnect, OnDisconnect);
//			mFrameInterval = 1f / this.mFrameRate;
		}

		float mStartTime;

		public void Connect(string ip,int port){
			client.Connect (ip,port);
		}

		public void Disconnect(){
			client.Disconnect ();
		}

		void OnConnected(NetworkMessage nm){
			Debuger.Log ("<color=green>Connect</color>");
			Debug.Log (BattleClientController.GetInstance ().playerId);
			BattleClientController.GetInstance ().playerId = nm.conn.connectionId;
			BattleClientUIManager.GetInstance ().OnConnected ();
		}

		void OnDisconnect(NetworkMessage nm){
			Debuger.Log ("<color=red>Disconnect</color>");
			BattleClientUIManager.GetInstance ().OnDisconnected ();
			BattleClientController.GetInstance ().Reset ();
		}

		void OnPlayerStatus(NetworkMessage nm){
			Debug.Log ("<color=greed>OnPlayerStatus</color>");
			PlayerStatusArray psa = nm.ReadMessage<PlayerStatusArray> ();
			BattleClientUIManager.GetInstance ().OnPlayerStatus (psa);
		}

		void OnCreatePlayer(NetworkMessage nm){
			Debug.Log ("OnCreatePlayer");
			CreatePlayer cp = nm.ReadMessage<CreatePlayer> ();
			BattleClientController.GetInstance ().CreatePlayers(cp);
			BattleClientReplayManager.GetInstance ().record.playerIds = cp.playerIds;
			BattleClientUIManager.GetInstance ().OnBattleBegin ();
		}

		bool mTestStop = false;

		public void Reset(){
			mCurrentServerMessage = null;
			mFrame = 0;
			mMaxRunableFrame = 0;
			mMaxFrame = 0;
			mRunSpeed = 0;
			mLastLogicFrameTime = 0;
			mNextMaxFrameRequestTime = 0;
			mRecievedFrameCount = 0;
			mStartTime = 0;
		}

		int mRecievedFrameCount = 0;
		//当前执行中的关键帧
		ServerMessage mCurrentServerMessage;
		//当前执行的关键帧番号
		int mFrame = 0;
		//可执行的最大帧番号
		int mMaxRunableFrame = 0;
		//接收到到最大帧番号
		int mMaxFrame = 0;
		//执行速度
		int mRunSpeed = 1;
		//上一次逻辑执行时间
		float mLastLogicFrameTime;
		float mNextMaxFrameRequestTime;
		public const float mMaxFrameWaitingTime = 0.5f;

		//服务端30FPS的发送频率。客户端60FPS的执行频率。
		//也就是说服务器1帧客户端2帧
		void Update(){
			#region 模拟网络阻塞
			if(Input.GetKeyDown(KeyCode.N)){
				mTestStop = true;
			}
			if(Input.GetKeyDown(KeyCode.M)){
				mTestStop = false;
			}
			if(mTestStop){
				return;
			}
			#endregion

			//把收到到消息放入可执行队列。
			PrepareRunableMessages ();
			//执行逻辑
			UpdateFrame ();
		}

		void UpdateFrame(){
			int count = 0;
			while(count < mRunSpeed){
				if (mCurrentServerMessage == null) {
					if (mRunableMessages.ContainsKey (mFrame)) {
						mCurrentServerMessage = mRunableMessages [mFrame];
						if(mCurrentServerMessage.playerHandles.Length>0)
							RecordMessage (mCurrentServerMessage);
						mRunableMessages.Remove (mFrame);
						BattleClientController.GetInstance ().LogicUpdate (mCurrentServerMessage);
						mFrame++;
					} else {
//						if(mFrame>0)
//							Debug.LogError ("Waiting key:" + mFrame);
					}
				} else {
					BattleClientController.GetInstance ().LogicUpdate (mCurrentServerMessage);
					mCurrentServerMessage = null;
				}
				count++;
				mLastLogicFrameTime = Time.realtimeSinceStartup;
			}

			//两帧等待间隔超过指定时间，认为是帧已经无法收到（这个跟网络延迟是有区别的）
			if(mLastLogicFrameTime < Time.realtimeSinceStartup - 1 && mNextMaxFrameRequestTime < Time.realtimeSinceStartup){

				mNextMaxFrameRequestTime = Time.realtimeSinceStartup + mMaxFrameWaitingTime;
				if (client.isConnected) {
					LostFrameIdsMessage lostFrameIdsMessage = GetLostFrameIds ();
					client.Send (MessageConstant.CLIENT_REQUEST_FRAMES, lostFrameIdsMessage);
				}
			}

			//如果遇到网络等待的場合
			if (mRunableMessages.Count >= 10) {
				mRunSpeed = 4;
			} else if (mRunableMessages.Count >= 3) {
				mRunSpeed = 2;
			} else if (mRunableMessages.Count <= 1){
				mRunSpeed = 1;
			}

		}

		//得到丢失的帧
		LostFrameIdsMessage GetLostFrameIds(){
			LostFrameIdsMessage messages = new LostFrameIdsMessage ();
			List<LostFrameIdAToBMessage> abMessages = new List<LostFrameIdAToBMessage> ();
			int fromFrame = 0;
			int toFrame = 0;
			for(int i = mFrame;i< mMaxFrame;i++){
				if (!mCachedMessages.ContainsKey (i)) {
					LostFrameIdAToBMessage abMessage = new LostFrameIdAToBMessage ();
					abMessage.fromFrame = i;
					abMessage.toFrame = i;
					while(true){
						i++;
						if(i > mMaxFrame){
							break;
						}
						if (!mCachedMessages.ContainsKey (i)) {
							abMessage.toFrame = i;
						} else {
							break;
						}
					}
					abMessages.Add (abMessage);
				}
			}
			messages.frameAToB = abMessages.ToArray ();
			return messages;

		}

		void OnFrameMessage (NetworkMessage mb)
		{
			if (mStartTime == 0)
				mStartTime = Time.realtimeSinceStartup;
			mRecievedFrameCount++;
//			Debug.Log (((mRecievedFrameCount-1) / (Time.realtimeSinceStartup - mStartTime)).ToString());
			ServerMessage sm = mb.ReadMessage<ServerMessage> ();
			AddServerMessage (sm);
		}

		void OnFrameMessages (NetworkMessage mb)
		{
			if (mStartTime == 0)
				mStartTime = Time.realtimeSinceStartup;
			mRecievedFrameCount++;
			Debug.Log (((mRecievedFrameCount-1) / (Time.realtimeSinceStartup - mStartTime)).ToString());
			CachedServerMessage cachedServerMessage = mb.ReadMessage<CachedServerMessage> ();
			for(int i=0;i<cachedServerMessage.serverMessages.Length;i++){
				AddServerMessage (cachedServerMessage.serverMessages[i]);
			}
		}

		public void AddServerMessage (ServerMessage tm)
		{
			//丢弃重复帧（防止受到数据被截获后的重复发送攻击）
			if (!mCachedMessages.ContainsKey (tm.frame)) {
				mCachedMessages.Add (tm.frame, tm);
				if(mMaxFrame < tm.frame){
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

		//准备可执行的网络数据
		void PrepareRunableMessages(){
			while(true){
				if (mCachedMessages.Count > 0 && mCachedMessages.ContainsKey (mMaxRunableFrame)) {
					mRunableMessages.Add (mMaxRunableFrame, mCachedMessages [mMaxRunableFrame]);
					mCachedMessages.Remove (mMaxRunableFrame);
					mMaxRunableFrame++;
				} else {
					break;
				}
			}
		}

		//记录操作便于回放
		void RecordMessage(ServerMessage tm){
			BattleClientReplayManager.GetInstance ().record.records.Add (tm);
		}

		public void SendReadyToServer(){
			Debug.Log ("SendReadyToServer");
			ClientMessage cm = new ClientMessage ();
			cm.clientReady = true;
			client.Send (MessageConstant.CLIENT_READY,cm);
		}

		public void SendPlayerHandle(PlayerHandle ph){
			client.Send (MessageConstant.CLIENT_PLAYER_HANDLE, ph);
		}
	}

	[Serializable]
	public class RecordMessage{
		public int[] playerIds = new int[0];
		public List<ServerMessage> records = new List<ServerMessage>();
	}


	[System.Serializable]
	public class PlayerKeys{
		public bool KeyA;
		public bool KeyS;
		public bool KeyD;
		public bool KeyW;
	}

}
