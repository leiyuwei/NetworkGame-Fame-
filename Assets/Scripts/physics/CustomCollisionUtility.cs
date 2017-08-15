using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public static class CustomCollisionUtility
	{

		//optimized.
		public static bool IsCircleAndCircle (CustomCircleCollider2D circle0, CustomCircleCollider2D circle1)
		{
			Vector2 pos0 = circle0.GetCenter ();
			Vector2 pos1 = circle1.GetCenter ();
			float radius = circle0.circle.radius + circle1.circle.radius;
			radius *= radius;
			float x = pos1.x - pos0.y;
			float y = pos1.y - pos0.y;
			float distance = x * x + y * y;
			return distance <= radius;
		}

		//optimized.
		public static bool IsCircleAndBox (CustomPhysics2D.CustomCircleCollider2D circle, CustomPhysics2D.CustomBoxCollider2D box, out Vector2 hit)
		{
			Vector2 min = box.mBoxCollider2D.bounds.min;
			Vector2 max = box.mBoxCollider2D.bounds.max;
			hit = Vector2.zero;
			Vector2 verticalPos = GetVerticalForPointAndLine (circle.GetCenter (), min, new Vector2 (min.x, max.y));
			if (IsPointOnLineSegment (verticalPos, min, new Vector2 (min.x, max.y))) {
				hit = verticalPos;
				return true;
			}
			verticalPos = GetVerticalForPointAndLine (circle.GetCenter (), new Vector2 (min.x, max.y), max);
			if (IsPointOnLineSegment (verticalPos, min, new Vector2 (min.x, max.y))) {
				hit = verticalPos;
				return true;
			}
			verticalPos = GetVerticalForPointAndLine (circle.GetCenter (), max, new Vector2 (max.x, min.y));
			if (IsPointOnLineSegment (verticalPos, min, new Vector2 (min.x, max.y))) {
				hit = verticalPos;
				return true;
			}
			verticalPos = GetVerticalForPointAndLine (circle.GetCenter (), new Vector2 (max.x, min.y), min);
			if (IsPointOnLineSegment (verticalPos, min, new Vector2 (min.x, max.y))) {
				hit = verticalPos;
				return true;
			}
			hit = Vector2.zero;
			return false;
		}

		static bool CheckIntersects(Bounds box1,Bounds box2){
			//1.Bounds.Intersects (bounds1)
			//2.
			if (box1.min.x > box2.max.x) return false;
			if (box1.max.x < box2.min.x) return false;
			if (box1.min.y > box2.max.y) return false;
			if (box1.max.y < box2.min.y) return false;
			return true;
		}

		//optimized.
		public static bool IsCollideBoxAndBox (CustomBoxCollider2D box0, CustomBoxCollider2D box1)
		{
			//AABB
			if(!CheckIntersects(box0.mBoxCollider2D.bounds,box1.mBoxCollider2D.bounds)){//  !.Intersects(box1.mBoxCollider2D.bounds)){
				return false;
			}
			Vector2 pos0 = box0.transform.TransformPoint (box0.GetCenter () + new Vector2 (-box0.mBoxCollider2D.size.x, -box0.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos1 = box0.transform.TransformPoint (box0.GetCenter () + new Vector2 (box0.mBoxCollider2D.size.x, -box0.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos2 = box0.transform.TransformPoint (box0.GetCenter () + new Vector2 (box0.mBoxCollider2D.size.x, box0.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos3 = box0.transform.TransformPoint (box0.GetCenter () + new Vector2 (-box0.mBoxCollider2D.size.x, box0.mBoxCollider2D.size.y) * 0.5f);
			Vector2[] boxVerts = new Vector2[]{ pos0, pos1, pos2, pos3 };
			Vector2 pos4 = box1.transform.TransformPoint (box1.GetCenter () + new Vector2 (-box1.mBoxCollider2D.size.x, -box1.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos5 = box1.transform.TransformPoint (box1.GetCenter () + new Vector2 (box1.mBoxCollider2D.size.x, -box1.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos6 = box1.transform.TransformPoint (box1.GetCenter () + new Vector2 (box1.mBoxCollider2D.size.x, box1.mBoxCollider2D.size.y) * 0.5f);
			Vector2 pos7 = box1.transform.TransformPoint (box1.GetCenter () + new Vector2 (-box1.mBoxCollider2D.size.x, box1.mBoxCollider2D.size.y) * 0.5f);
			Vector2[] boxVerts1 = new Vector2[]{ pos4, pos5, pos6, pos7 };
			Vector2 hit;
			for (int i = 0; i < boxVerts.Length; i++) {
				for (int j = 0; j < boxVerts1.Length; j++) {
					bool isIntersect = IsLineSegmentAndLineSegment (boxVerts [i], boxVerts [(i + 1) % boxVerts.Length], boxVerts1 [j], boxVerts1 [(j + 1) / boxVerts1.Length], out hit);
					if(isIntersect){
						return true;
					}
				}
			}
			return false;
		}

		public static List<Vector2> GetBoxAndBoxCollisions (CustomBoxCollider2D box0, CustomBoxCollider2D box1)
		{
			return null;
		}

		public static List<Vector2> GetCircleAndCircleCollisions (CustomCircleCollider2D circle0, CustomCircleCollider2D circle1)
		{
			return null;
		}

		public static List<Vector2> GetCircleAndBoxCollisions (CustomCircleCollider2D circle, CustomBoxCollider2D box)
		{
			return null;
		}

		//求点到直线的交点
		//注释处为推理过程（代数法）
		//先求出斜率a，再求出常量b
		public static Vector2 GetVerticalForPointAndLine (Vector2 point, Vector2 start, Vector2 end)
		{
			//二個特別な場合
			if (start.x - end.x == 0) {
				return new Vector2 (start.x, point.y);
			}

			if (start.y - end.y == 0) {
				return new Vector2 (point.x, start.y);
			}

			//y = ax + b;
			//start.y = start.x * a + b;
			//end.y = end.x * a + b;
			float a = (start.y - end.y) / (start.x - end.x);
			float b = start.y - start.x * a;
			//y = a * x + b；

			float a0 = (start.x - end.x) / (start.y - end.y);
			float b0 = point.y - point.x * a0;
			//y = a0 * x + b0

			//0 = (a0 - a) * x + b0 -b;
			float x0 = (b - b0) / (a0 - a);
			float y0 = a * x0 + b;
			return new Vector2 (x0, y0);
		}

		//点是否在线段上
		public static bool IsPointOnLineSegment (Vector2 point, Vector2 start, Vector2 end)
		{
			float distance = (start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y);
			float dis0 = (point.x - end.x) * (point.x - end.x) + (point.y - end.y) * (point.y - end.y);
			float dis1 = (point.x - start.x) * (point.x - start.x) + (point.y - start.y) * (point.y - start.y);
			return distance == dis0 + dis1;
		}

		//二つ線の交点（Algebraic）
		public static bool IsLineSegmentAndLineSegment (Vector2 start0, Vector2 end0, Vector2 start1, Vector2 end1, out Vector2 hit)
		{
			Vector2 delta0 = start0 - end0;
			/**
			 * 推理には
			 * y = a0x + b0;
			**/
			float a0 = delta0.y / delta0.x;
			float b0 = start0.y - start0.x * a0;
			Vector2 delta1 = start1 - end1;
			/**
			 * 推理には
			 * y = a1x + b1;
			**/
			float a1 = (start1.y - end1.y) / (start1.x - end1.x);
			float b1 = start1.y - start1.x * a1;
			//平行線
			if (a0 == a1) {
				hit = Vector2.zero;
				return false;
			}
			/**
			 * 推理には
			 * y = a0 * x + b0;
			 * y = a1 * x + b1;
			 * 0 = (a0-a1) * x + (b0 - b1);
			 * x = (b1 - b0) / (a0 - a1);
			 * y = x * a0 + b0;
			**/	
			float x0 = (b1 - b0) / (a0 - a1);
			float y0 = x0 * a0 + b0;
			hit = new Vector2 (x0, y0);
			return true;
		}

	}
}