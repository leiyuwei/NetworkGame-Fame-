using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics2D;

public class TestPointOnLineSegment : MonoBehaviour {

	public Vector2 pos0;
	public Vector2 pos1;
	public Vector2 pos2;

	void Update () {
		if(Input.GetKeyDown(KeyCode.F)){
			Debug.Log(MathUtility.IsPointOnLineSegment (pos0,pos1,pos2));
		}
	}
}
