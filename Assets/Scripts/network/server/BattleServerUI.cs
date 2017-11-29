using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace MultipleBattle
{
	//Debug用Text.
	public class BattleServerUI : SingleMonoBehaviour<BattleServerUI>
	{
		public BattleServer battleServer;
		public Text txt_ip;
		public Text txt_port;
		public Text txt_maxPlayer;
		public Text txt_debug;
		public Button btn_reset;

		protected override void Awake ()
		{
			base.Awake ();
			if(battleServer==null){
				battleServer = GetComponent<BattleServer> ();
			}
			if(txt_port!=null)
				txt_port.text = " Port:" + battleServer.networkPort.ToString ();
			if(txt_ip!=null)
				txt_ip.text =" IP:" + Network.player.ipAddress;
			if (btn_reset != null) {
				btn_reset.onClick.AddListener (()=>{
					battleServer.Reset();
				});
			}
			SetPlayer ();
		}

		int mCurrentConnetCount;
		void SetPlayer(){
			txt_maxPlayer.text = string.Format (" Player: {0}/{1}",NetworkServer.connections.Count,NetConstant.max_player_count);
		}

		void Update(){
			if(mCurrentConnetCount != NetworkServer.connections.Count){
				mCurrentConnetCount = NetworkServer.connections.Count;
				SetPlayer ();
			}
		}

	}
}
