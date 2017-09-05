using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public class TestCustomLineLineIntersect : MonoBehaviour
	{

		public Vector2 pos0;
		public Vector2 pos1;

		public Vector2 pos2;
		public Vector2 pos3;

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.F)) {
				Vector2 hit = Vector2.zero;
				bool isHit = MathUtility.IsLineSegmentAndLineSegment (pos0,pos1,pos2,pos3,out hit);
				Debug.Log ("isHit:" + isHit + ";hit:" + hit);
			}
		}
	}
}
