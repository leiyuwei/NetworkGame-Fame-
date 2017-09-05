using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebuger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public Vector2 pos0;//circle center
	public Vector2 pos1;//line start
	public Vector2 pos2;//line end
	public Vector2 pos3;//vertical pos
	public Vector2 pos4;
	public Vector2 pos5;
	public Vector2 pos6;
	public Vector2 pos7;

	void OnDrawGizmos(){
		Gizmos.DrawLine (pos0,pos3);
		Gizmos.DrawLine (pos1,pos2);
//		Gizmos.DrawLine (pos0,pos1);
//		Gizmos.DrawLine (pos0,pos1);
//		Gizmos.DrawLine (pos0,pos1);
//		Gizmos.DrawLine (pos0,pos1);
//		Gizmos.DrawLine (pos0,pos1);

	}

}
