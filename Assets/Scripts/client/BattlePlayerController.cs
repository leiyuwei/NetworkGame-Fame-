using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class BattlePlayerController : SingleMonoBehaviour<BattlePlayerController>
	{
		float mPreX;
		void Update ()
		{
			if (Input.GetMouseButton (0)) {
				PlayerHandle ph = new PlayerHandle ();
				float x = Mathf.Round(Input.mousePosition.x);
				if (mPreX != x) {
					mPreX = x;
					ph.mousePos = new Vector2 (x,0);
					ph.playerId = BattleClientController.Instance.playerId;
					BattleClient.Instance.SendPlayerHandle (ph);
				}
			}
		}
	}
}
