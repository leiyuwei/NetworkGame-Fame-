using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public class TestCustomCircleAndBox : MonoBehaviour
	{

		public CustomCircleCollider2D mCircle;
		public CustomBoxCollider2D mBox;

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.H)) {
				Vector2 hit;
				Vector2 normal;
				CustomCollisionUtility.IsCircleAndBox (mCircle, mBox, out hit,out normal);
				Debug.Log (hit);
			}
		}

	}
}
