using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public class TestCustomCircleAndBox : MonoBehaviour
	{

		public CustomCircleCollider2D mCircle;
		public CustomBoxCollider2D mBox;
		List<Vector2> mPosList;

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.H)) {
				Vector2 hit;
				Vector2 normal;
				CustomCollisionUtility.IsCircleAndBox (mCircle, mBox, out hit,out normal);
				Debug.Log (hit);
			}
		}

		void OnDrawGizmos ()
		{
			if (mPosList == null)
				return;
			for (int i = 0; i < mPosList.Count; i++) {
				Gizmos.DrawLine (mCircle.GetCenter(),mPosList[i]);
			}
		}

	}
}
