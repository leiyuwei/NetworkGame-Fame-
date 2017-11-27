using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics2D
{
	public static class MathUtility
	{
		//求点到直线的垂线交点
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

			Vector2 rotated = Rotate (new Vector2(start.x - end.x,start.y - end.y),90f);
			float a0 = rotated.y / rotated.x;//    (start.x - end.x) / (start.y - end.y);
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
			float distance = (end - start).magnitude;// (start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y);
			float dis0 = (point - end).magnitude;// (point.x - end.x) * (point.x - end.x) + (point.y - end.y) * (point.y - end.y);
			float dis1 = (point - start).magnitude;// (point.x - start.x) * (point.x - start.x) + (point.y - start.y) * (point.y - start.y);
//			Debug.Log (string.Format("{0},{1},{2}",distance,dis0,dis1));
			return Mathf.RoundToInt(distance * 1000) == Mathf.RoundToInt((dis0 + dis1) * 1000);
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

		//角度を回し
		public static Vector2 Rotate (Vector2 pos, float degrees)
		{
			float angle = degrees / 180f * Mathf.PI;
			float newX = pos.x * Mathf.Cos (angle) - pos.y * Mathf.Sin (angle);
			float newY = pos.x * Mathf.Sin (angle) + pos.y * Mathf.Cos (angle);
			return new Vector2 (newX, newY);
		}

		//点で相対的回し
		public static Vector2 RotateAround (Vector2 pos, Vector2 point, float degrees)
		{
			float angle = 0;
			Vector2 relativePos = pos - point;
			Vector2 rotatePos = Rotate (relativePos, degrees);
			return rotatePos;
		}

		//ポイントが三角形に中にかどうかチェックする（判断点是否在三角形里面）
		public static bool IsPointInTri(Vector2 pt,  Vector2 v1,  Vector2 v2,  Vector2 v3)
		{
			float TotalArea = CalcTriArea(v1, v2, v3);
			float Area1 = CalcTriArea(pt, v2, v3);
			float Area2 = CalcTriArea(pt, v1, v3);
			float Area3 = CalcTriArea(pt, v1, v2);
			if((Area1 + Area2 + Area3) > TotalArea)
				return false;
			else
				return true;
		}

		public static float CalcTriArea(Vector2 v1, Vector2 v2, Vector2 v3)
		{
			float det = 0.0f;
			det = ((v1.x - v3.x) * (v2.y - v3.y)) - ((v2.x - v3.x) * (v1.y - v3.y));
			return (det / 2.0f);
		}
	}
}