using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultipleBattle
{
	public class BattlePlayerController : SingleMonoBehaviour<BattlePlayerController>
	{
		Vector2 mPrePos;
		void Update ()
		{
			if (Input.GetMouseButton (0)) {
				PlayerHandle ph = new PlayerHandle ();
				if (mPrePos != (Vector2)Input.mousePosition) {
					mPrePos = (Vector2)Input.mousePosition;
					ph.mousePos = (Vector2)Input.mousePosition;
					ph.playerId = BattleClientController.Instance.playerId;
					BattleClient.Instance.SendPlayerHandle (ph);
				}
			}
		}
	}
}
