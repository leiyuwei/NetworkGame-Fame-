using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics2D;

public class TestCustomVerticalIntersect : MonoBehaviour
{

	public Vector2 pos0;
	public Vector2 pos1;
	public Vector2 pos2;

	Vector2 hit;
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			hit = MathUtility.GetVerticalForPointAndLine (pos0, pos1, pos2);
			Debug.Log ("hit:" + hit);
		}
	}

	void OnDrawGizmos(){
		if (hit != Vector2.zero) {
			Gizmos.DrawLine (pos0, hit);
			Gizmos.DrawLine (pos1, pos2);
		}
	}

}
