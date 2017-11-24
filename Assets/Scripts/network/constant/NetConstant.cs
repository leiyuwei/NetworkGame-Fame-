using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetConstant {

	public static int listene_port = 8080;
	public static int player_count = 1;

	public const int FRAME_RATE = 30;
	public const string MAX_PLAYER = "MAX_PLAYER";
	public static int MaxNum{

		get{
			int max = 1;
			if(PlayerPrefs.HasKey(MAX_PLAYER)){
				return Mathf.Min(max,PlayerPrefs.GetInt (MAX_PLAYER));
			}
			return max;
		}
		set{

			PlayerPrefs.SetInt (MAX_PLAYER, value);
			PlayerPrefs.Save ();
			Debug.Log (PlayerPrefs.GetInt (MAX_PLAYER));
		}
	}
}
