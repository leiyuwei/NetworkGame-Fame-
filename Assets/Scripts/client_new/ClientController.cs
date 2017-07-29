using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
	public class ClientController : SingleMonoBehaviour<ClientController>
	{
		public Camera gameCamera;
		public Ball ball;
		public Button btn_start;

		public List<Transform> plants;
		Dictionary<int,Transform> playerPlants;

		public int playerId;

		protected override void Awake ()
		{
			base.Awake ();
		}

		public bool isAuto;
		void Update(){
			if(isAuto){
//				plant.transform.position = new Vector3 (ball.transform.position.x,plant.transform.position.y,0);
			}

			if(Input.GetMouseButton(0)){
				Move ();
			}
		}

		void Move(){
			Vector3 pos = gameCamera.ScreenToWorldPoint (Input.mousePosition);

//			plant.transform.position = new Vector3 (pos.x,plant.transform.position.y,0);
		}
	}
}
