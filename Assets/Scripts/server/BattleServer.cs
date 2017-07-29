using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

namespace MultipleBattle
{
	public class BattleServer : NetworkManager
	{
		public Text txt_ip;
		public Text txt_port;
		public Text txt_maxPlayer;
		public Text txt_debug;
		public int port = 8080;
		public bool isBattleBegin;
		public int MAX_Player = 1;
		public const string MAX_PLAYER = "MAX_PLAYER";
		int mCurrentPlayer = 0;
		int mFrame = 0;
		float mStartTime;
		float t;
		float mNextFrameTime;
		BattleServerController mBattleServerController;
		//服务器缓存的操作
		public Dictionary<int,ServerMessage> mCachedServerMessageDic;
		[HideInInspector]
		public ServerMessage currentMessage;
		[HideInInspector]
		public List<PlayerHandle> playerHandleList;
		Dictionary<int,PlayerStatus> mConnections;

		void Awake ()
		{
			if(PlayerPrefs.HasKey(MAX_PLAYER)){
				MAX_Player = PlayerPrefs.GetInt (MAX_PLAYER);
			}
			txt_maxPlayer.text = MAX_Player.ToString ();
			this.networkPort = port;
			if(txt_port!=null)
			txt_port.text = " Port:" + port.ToString ();
			if(txt_ip!=null)
			txt_ip.text =" IP:" + Network.player.ipAddress;
			mBattleServerController = GetComponent<BattleServerController> ();
			Reset ();
			this.StartServer ();
			connectionConfig.SendDelay = 1;
			NetworkServer.maxDelay = 0;
			NetworkServer.RegisterHandler (MessageConstant.CLIENT_READY,OnRecieveClientReady);
			NetworkServer.RegisterHandler (MessageConstant.CLIENT_PLAYER_HANDLE,OnRecievePlayerHandle);
			NetworkServer.RegisterHandler (MessageConstant.CLIENT_REQUEST_FRAMES, OnRecievePlayerFrameRequest);
			NetworkServer.RegisterHandler (MsgType.Connect, OnClientConnect);
			NetworkServer.RegisterHandler (MsgType.Disconnect, OnClientDisconnect);
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for(int i=0;i<commandLineArgs.Length;i++){
				if(commandLineArgs[i].ToLower().IndexOf("playercount")!=-1){
					string[] countStrs = commandLineArgs[i].Split(new char[]{'='});
					if(countStrs.Length>1){
						int playerCount = 0;
						if(int.TryParse(countStrs[1],out playerCount)){
							if (playerCount > 0) {
								MAX_Player = playerCount;
								txt_maxPlayer.text = MAX_Player.ToString ();
							}
						}
					}
				}
			}
			mFrameInterval = 1f / mFrameRate;
		}

		void Reset(){
			isBattleBegin = false;
			currentMessage = new ServerMessage ();
			playerHandleList = new List<PlayerHandle> ();
			mCachedServerMessageDic = new Dictionary<int, ServerMessage> ();
			mConnections = new Dictionary<int, PlayerStatus> ();
			mCurrentPlayer = 0;
			mStartTime = 0;
			mFrame = 0;
			t = 0;
			mNextFrameTime = 0;
		}

		int mFrameRate = 30;
		float mFrameInterval;
		void Update ()
		{
			if(Input.GetKeyDown(KeyCode.P)){
				MAX_Player++;
				MAX_Player = Mathf.Min (10, MAX_Player);
				txt_maxPlayer.text = MAX_Player.ToString ();
				PlayerPrefs.SetInt (MAX_PLAYER, MAX_Player);
				PlayerPrefs.Save ();
			}
			if(Input.GetKeyDown(KeyCode.O)){
				MAX_Player--;
				MAX_Player = Mathf.Max (1, MAX_Player);
				txt_maxPlayer.text = MAX_Player.ToString ();
				PlayerPrefs.SetInt (MAX_PLAYER, MAX_Player);
				PlayerPrefs.Save ();
			}

			if (!isBattleBegin) {
				return;
			}
			SendFrame ();
			if(this.mConnections.Count == 0){
				isBattleBegin = false;
				mFrame = 0;
			}
		}

		void OnGUI(){
			GUILayout.Label ((mFrame / (Time.realtimeSinceStartup - mStartTime)).ToString());
		}

		#region 1.Send
		//当有玩家加入或者退出或者准备的場合
		void SendPlayerStatus(){
			List<PlayerStatus> pss = new List<PlayerStatus> ();
			foreach(PlayerStatus ps in mConnections.Values){
				pss.Add (ps);
			}
			Debug.Log (pss.Count);
			PlayerStatusArray psa = new PlayerStatusArray ();
			psa.playerStatus = pss.ToArray ();
			NetworkServer.SendToAll (MessageConstant.SERVER_CLIENT_STATUS,psa);
		}

		//告诉客户端创建人物
		void SendBattleBegin(){
			CreatePlayer cp = new CreatePlayer ();
			List<int> playerIds = new List<int> ();
			foreach(NetworkConnection nc in NetworkServer.connections){
				Debug.Log (nc);
				if(nc!=null)
				playerIds.Add (nc.connectionId);
			}
			cp.playerIds = playerIds.ToArray ();
			NetworkServer.SendToAll (MessageConstant.CLIENT_READY,cp);
		}

		//发送Frame消息
		void SendFrame(){
			while(mNextFrameTime <= Time.fixedUnscaledTime){
				mNextFrameTime += mFrameInterval;
				SendFrameMessage ();
				mFrame++;
			}
		}

		//广播消息
		void SendFrameMessage(){
			SetFrameMessage ();
			NetworkServer.SendUnreliableToAll (MessageConstant.SERVER_TO_CLIENT_MSG, currentMessage);
			SaveMessage (currentMessage);
			currentMessage = new ServerMessage ();
		}

		//组装数据
		void SetFrameMessage(){
			currentMessage.frame = mFrame;
			currentMessage.playerHandles = playerHandleList.ToArray();
			playerHandleList = new List<PlayerHandle> ();
		}
		#endregion

		#region 2.Recieve
		void OnClientConnect (NetworkMessage nm)
		{
			Debug.Log ("OnClientConnect");
			NetworkConnection conn = nm.conn;
			if (isBattleBegin || mConnections.Count >= MAX_Player) {
				conn.Disconnect ();
			}else {
				PlayerStatus ps = new PlayerStatus ();
				ps.playerId = conn.connectionId;
				ps.isReady = false;
				mConnections.Add (conn.connectionId,ps);
				SendPlayerStatus ();
			}
		}

		void OnClientDisconnect (NetworkMessage nm)
		{
			Debug.Log ("OnClientDisconnect");
			NetworkConnection conn = nm.conn;
			mConnections.Remove(conn.connectionId);

			if (mConnections.Count == 0) {
				Reset ();
			} else {
				SendPlayerStatus ();
			}
		}

		//收到用户准备
		void OnRecieveClientReady(NetworkMessage msg){
			Debuger.Log ("OnRecieveClientReady");
			if(mConnections.ContainsKey(msg.conn.connectionId)){
				mConnections [msg.conn.connectionId].isReady = true;
			}
			int count = 0;
			foreach(PlayerStatus ps in mConnections.Values){
				if (ps.isReady) {
					count++;
				} 
			}
			if (count >= MAX_Player) {
				isBattleBegin = true;
				SendBattleBegin ();
				mBattleServerController.BattleBegin ();
				mStartTime = Time.realtimeSinceStartup;
				mNextFrameTime = Time.realtimeSinceStartup;
			} 
			SendPlayerStatus ();
		}

		//收到用户请求丢失的帧
		void OnRecievePlayerFrameRequest(NetworkMessage msg){
		
		}

		//收到用户操作
		void OnRecievePlayerHandle(NetworkMessage msg){
			PlayerHandle playerHandle = msg.ReadMessage<PlayerHandle> ();
			playerHandle.playerId = msg.conn.connectionId;
			playerHandleList.Add (playerHandle);//TODO
		}
		#endregion

		//保存操作，用于补帧。TODO:用于回放
		void SaveMessage(ServerMessage sm){
			//只保存有操作的数据
			if(currentMessage.playerHandles.Length > 0 )
				mCachedServerMessageDic.Add (sm.frame,sm);
		}

	}
}
