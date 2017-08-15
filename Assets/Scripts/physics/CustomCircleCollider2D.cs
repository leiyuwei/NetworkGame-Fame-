using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	[RequireComponent(typeof(UnityEngine.CircleCollider2D))]
	public class CustomCircleCollider2D : CustomCollider2D
	{
		public UnityEngine.CircleCollider2D circle;

		protected override void Awake ()
		{
			base.Awake ();
			circle = GetComponent<UnityEngine.CircleCollider2D> ();
		}

		public override bool CheckCollision (CustomCollider2D other)
		{
			bool isColl = false;
			switch(other.type){
			case Collider2DType.CircleCollider2D:
				Debug.Log (other);
				isColl = CustomCollisionUtility.IsCircleAndCircle (this,(CustomCircleCollider2D)other);
				break;
			case Collider2DType.BoxCollider2D:
				Vector2 hit;
				isColl = CustomCollisionUtility.IsCircleAndBox (this, (CustomBoxCollider2D)other,out hit);
				if(isColl){
					Debug.Log ("hit:" + hit);
				}
				break;
			}
			return isColl;
		}




	}

}
