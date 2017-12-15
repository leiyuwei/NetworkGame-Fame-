using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Text;
using UnityEngine.Events;

namespace MMO
{
	public class MMOServer : NetworkManager
	{

		public bool isBattleBegin;

		public UnityAction onClientConnect;
		public UnityAction onClientDisconnect;

		float mNextFrameTime;
//		float mFrameInterval;
		Dictionary<int,PlayerData> dic_player_data;
		Dictionary<int,int> connectionIds;
		int mCurrentMaxId = 0;
		//10回per1秒。mmorpgは30FrameRate以内、FPSは60FrameRate.
		public const int FRAME_RATE = 30;

		void Awake ()
		{
			this.networkPort = NetConstant.LISTENE_PORT;
			this.StartServer ();
			dic_player_data = new Dictionary<int, PlayerData> ();
			connectionIds = new Dictionary<int, int> ();
			connectionConfig.SendDelay = 1;
			NetworkServer.RegisterHandler (MsgType.Connect, OnClientConnect);
			NetworkServer.RegisterHandler (MsgType.Disconnect, OnClientDisconnect);
			NetworkServer.RegisterHandler (MessageConstant.CLIENT_TO_SERVER_MSG,OnRecievePlayerMessage);
			NetworkServer.maxDelay = 0;
//			mFrameInterval = 1f / FRAME_RATE;
		}

		#region 1.Recieve
		void OnClientConnect (NetworkMessage nm)
		{
			Debug.logger.Log ("OnClientConnect");
			PlayerInfo playerInfo = new PlayerInfo ();
			playerInfo.playerId = mCurrentMaxId;
			playerInfo.attribute = MMOBattleServerManager.Instance.InitUnit (1);
//			playerInfo.attribute = 
			connectionIds.Add (nm.conn.connectionId,mCurrentMaxId);
			if(onClientConnect!=null){
				onClientConnect ();
			}
			NetworkServer.SendToClient (nm.conn.connectionId,MessageConstant.SERVER_TO_CLIENT_PLAYER_INFO,playerInfo);
			mCurrentMaxId++;
		}

		void OnClientDisconnect (NetworkMessage nm)
		{
			Debug.logger.Log ("OnClientDisconnect");
			int playerId = connectionIds [nm.conn.connectionId];
			connectionIds.Remove (nm.conn.connectionId);
			dic_player_data.Remove (playerId);//診察
			TransferData data = GetTransferData ();
			if (onClientDisconnect != null)
				onClientDisconnect ();
			NetworkServer.SendUnreliableToAll (MessageConstant.SERVER_TO_CLIENT_MSG, data);
		}

		//收到用户准备准备完毕
		//ユーザーを準備できたメセージを受信する
		void OnRecieveClientReady(NetworkMessage msg){
			Debug.logger.Log ("OnRecieveClientReady");
			//Debug.logger.Log("OnRecieveClientReady");
		}

		void OnRecievePlayerMessage(NetworkMessage msg){
			PlayerData playerHandle = msg.ReadMessage<PlayerData> ();
			if (!dic_player_data.ContainsKey (playerHandle.playerId)) {
				dic_player_data.Add (playerHandle.playerId, playerHandle);
			} 
			dic_player_data [playerHandle.playerId] = playerHandle;
			TransferData data = GetTransferData ();
			NetworkServer.SendUnreliableToAll (MessageConstant.SERVER_TO_CLIENT_MSG, data);
		}

		TransferData GetTransferData(){
			TransferData data = new TransferData ();
			data.playerDatas = new PlayerData[dic_player_data.Count];
			int i = 0;
			List<PlayerData> playerDataList = new List<PlayerData> ();
			foreach(int id in dic_player_data.Keys){
				playerDataList.Add (dic_player_data [id]);
				data.playerDatas [i] = dic_player_data [id];
				i++;
			}
			data.playerDatas = playerDataList.ToArray ();
			return data;
		}

		#endregion
	}

}

