using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPhysics2D;

namespace CustomPhysics2D
{
	public class TestCustomRotate : MonoBehaviour
	{
		public float degree = 10f;
		void Start ()
		{
		
		}
	
		void Update ()
		{
			transform.position = MathUtility.Rotate ((Vector2)transform.position,degree * Time.deltaTime);
		}
	}
}
