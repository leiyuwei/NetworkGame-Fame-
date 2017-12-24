﻿using System.Collections;
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
		public UnityAction<PlayerInfo> onRecievePlayerMessage;
		float mNextFrameTime;
		Dictionary<int,PlayerInfo> dic_player_data;
		Dictionary<int,int> connectionIds;
		int mCurrentMaxId = 0;
		//10回per1秒。mmorpg は 30 FrameRate 以内、FPS は 60 FrameRate 以内.
		public const int FRAME_RATE = 30;

		void Awake ()
		{
			this.networkPort = NetConstant.LISTENE_PORT;
			this.StartServer ();
			dic_player_data = new Dictionary<int, PlayerInfo> ();
			connectionIds = new Dictionary<int, int> ();
			connectionConfig.SendDelay = 1;
			NetworkServer.RegisterHandler (MsgType.Connect, OnClientConnect);
			NetworkServer.RegisterHandler (MsgType.Disconnect, OnClientDisconnect);
			NetworkServer.RegisterHandler (MessageConstant.CLIENT_TO_SERVER_MSG, OnRecievePlayerMessage);
			NetworkServer.maxDelay = 0;
//			mFrameInterval = 1f / FRAME_RATE;
		}

		public void UpdateMonsters (UnitInfo[] monsters)
		{
			TransferData data = new TransferData ();
			data.monsterDatas = monsters;
			NetworkServer.SendToAll (MessageConstant.SERVER_TO_CLIENT_MONSTER_INFO, data);
			for (int i = 0; i < monsters.Length; i++) {
				monsters [i].action.attackType = -1;
				monsters [i].action.targetPos = Vector3.zero;
			}
		}

		#region 1.Recieve

		void OnClientConnect (NetworkMessage nm)
		{
			Debug.logger.Log ("OnClientConnect");
			if (nm.channelId == 99)
				return;
			PlayerInfo playerInfo = new PlayerInfo ();
			playerInfo.playerId = mCurrentMaxId;
			playerInfo.unitInfo = MMOBattleServerManager.Instance.AddPlayer().unitInfo;
			connectionIds.Add (nm.conn.connectionId, mCurrentMaxId);
			if (!dic_player_data.ContainsKey (playerInfo.playerId)) {
				dic_player_data.Add (playerInfo.playerId, playerInfo);
			} 
			if (onClientConnect != null) {
				onClientConnect ();
			}
			NetworkServer.SendToClient (nm.conn.connectionId, MessageConstant.SERVER_TO_CLIENT_PLAYER_INFO, playerInfo);
			mCurrentMaxId++;
		}

		void OnClientDisconnect (NetworkMessage nm)
		{
			Debug.logger.Log ("OnClientDisconnect");
			int playerId = connectionIds [nm.conn.connectionId];
			connectionIds.Remove (nm.conn.connectionId);
			dic_player_data.Remove (playerId);
			TransferData data = GetTransferData ();
			if (onClientDisconnect != null)
				onClientDisconnect ();
			NetworkServer.SendUnreliableToAll (MessageConstant.SERVER_TO_CLIENT_MSG, data);
		}

		//收到用户准备准备完毕
		//ユーザーを準備できたメセージを受信する
		void OnRecieveClientReady (NetworkMessage msg)
		{
			Debug.logger.Log ("OnRecieveClientReady");
			//Debug.logger.Log("OnRecieveClientReady");
		}

		//分成三个方法，分别更新transform，animation，attribute
		void OnRecievePlayerMessage (NetworkMessage msg)
		{
			Debug.logger.Log ("OnRecievePlayerMessage");
			PlayerInfo playerHandle = msg.ReadMessage<PlayerInfo> ();
			dic_player_data [playerHandle.playerId] = playerHandle;
			TransferData data = GetTransferData ();
			NetworkServer.SendUnreliableToAll (MessageConstant.SERVER_TO_CLIENT_MSG, data);
			playerHandle.chat = "";
			if (onRecievePlayerMessage != null)
				onRecievePlayerMessage (playerHandle);
		}

		TransferData GetTransferData ()
		{
			TransferData data = new TransferData ();
			data.playerDatas = new PlayerInfo[dic_player_data.Count];
			int i = 0;
			List<PlayerInfo> playerDataList = new List<PlayerInfo> ();
			foreach (int id in dic_player_data.Keys) {
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

