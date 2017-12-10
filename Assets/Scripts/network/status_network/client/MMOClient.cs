using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace MMO
{
	public class MMOClient : SingleMonoBehaviour<MMOClient>
	{
		NetworkClient client;
		UnityAction<NetworkMessage> onConnect;

		void Start ()
		{
			client = new NetworkClient ();
			client.RegisterHandler (MsgType.Connect, OnConnect);
			client.RegisterHandler (MsgType.Disconnect, OnDisconnect);
			client.RegisterHandler (MessageConstant.SERVER_TO_CLIENT_MSG,OnRecieveMessage);
		}
	
		void Update(){
			if(Input.GetKeyDown(KeyCode.H)){
//				client.Connect ();
				Connect("127.0.0.1",NetConstant.LISTENE_PORT,null);
			}
		}

		public void SendMessage(){
		
		}

		public void Connect (string ip, int port, UnityAction<NetworkMessage> onConnect)
		{
			Debug.Log (string.Format ("{0},{1}", ip, port));
			this.onConnect = onConnect;
			client.Connect (ip, port);
		}

		void OnConnect (NetworkMessage nm)
		{
			Debug.logger.Log ("<color=green>Connect</color>");
			if (onConnect != null)
				onConnect (nm);
		}

		void OnDisconnect (NetworkMessage nm)
		{
			Debug.logger.Log ("<color=red>Disconnect</color>");
			//			BattleClientController.Instance.Reset ();
		}

		void OnRecieveMessage(NetworkMessage nm){
			
		}

	}
}