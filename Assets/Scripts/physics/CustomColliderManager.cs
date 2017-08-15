using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public class CustomColliderManager : SingleMonoBehaviour<CustomColliderManager>
	{

		public List<CustomCollider2D> colliders;

		protected override void Awake ()
		{
			base.Awake ();
		}

		void FixedUpdate ()
		{
			for (int i = 0; i < colliders.Count; i++) {
				CustomCollider2D col = colliders [i];
				for (int j = 0; j < colliders.Count; j++) {
					if (col != colliders [j]) {
						col.CheckCollision (colliders [j]);
					}
				}
			}
		}
	}
}
