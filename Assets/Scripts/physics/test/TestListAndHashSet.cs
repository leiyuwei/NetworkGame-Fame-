using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListAndHashSet : MonoBehaviour {

	HashSet<int> mSet;
	List<int> mList;
	public int count = 1000;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F)){
			float t = Time.realtimeSinceStartup;
			foreach(int i in mSet){
				
			}
			Debug.Log (string.Format("HashSet {0} foreach time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
			t = Time.realtimeSinceStartup;
			foreach(int i in mList){

			}
			Debug.Log (string.Format("HashSet {0} foreach time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
			t = Time.realtimeSinceStartup;
			for(int i=0;i<mList.Count;i++){
				
			}
			Debug.Log (string.Format("HashSet {0} for time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
		}

		if(Input.GetKeyDown(KeyCode.G)){
			mSet = new HashSet<int> ();
			mList = new List<int> ();
			float t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mSet.Add (i);
			}
			Debug.Log (string.Format("HashSet {0} add time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
			t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mList.Add (i);
			}
			Debug.Log (string.Format( "List {0} add time:{1} ms" ,count,(Time.realtimeSinceStartup - t ) * 1000) );
		}

		if(Input.GetKeyDown(KeyCode.J)){
			float t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mSet.Contains (i);
			}
			Debug.Log (string.Format("HashSet {0} contains time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
			t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mList.Contains (i);
			}
			Debug.Log (string.Format("HashSet {0} contains time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
		}

		if(Input.GetKeyDown(KeyCode.H)){
			float t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mSet.Remove (i);
			}
			Debug.Log (string.Format("HashSet {0} remove time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
			t = Time.realtimeSinceStartup;
			for(int i=0;i<count;i++){
				mList.Remove (i);
			}
			Debug.Log (string.Format("HashSet {0} remove time:{1} ms",count, (Time.realtimeSinceStartup - t) * 1000));
		}
	}



}
