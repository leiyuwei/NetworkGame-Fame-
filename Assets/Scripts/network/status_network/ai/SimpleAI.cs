using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
	public class SimpleAI : MonoBehaviour
	{

		MMOUnit mMMOUnit;
		public List<Vector3> wayPoints;
		int mCurrentWayPointIndex = 0;
		Transform mTrans;
		float mMoveSpeed = 5;
		bool mIsMoving;

		void Awake(){
			mMMOUnit = GetComponent<MMOUnit> ();
			mTrans = transform;
		}

		void Update(){
			if(mIsMoving)
				LoopMove ();
		}

		float mMaxMoveDistance = 20;
		public void Move(int moveType){
			mIsMoving = true;
			mMMOUnit.unitInfo.animation.animSpeed = mMoveSpeed / 5;
			if (moveType == 1) {
				mMMOUnit.unitInfo.animation.action = AnimationConstant.UNIT_ANIMATION_CLIP_RUN;
			} else {
				mMMOUnit.unitInfo.animation.action = AnimationConstant.UNIT_ANIMATION_CLIP_WALK;
				mMoveSpeed = mMoveSpeed / 6f;
			}
			wayPoints.Add (mTrans.position + new Vector3(Random.Range(-mMaxMoveDistance,mMaxMoveDistance),0,0));
			wayPoints.Add (mTrans.position + new Vector3(0,0,Random.Range(-mMaxMoveDistance,mMaxMoveDistance)));
			wayPoints.Add (mTrans.position + new Vector3(Random.Range(-mMaxMoveDistance,mMaxMoveDistance),0,Random.Range(-mMaxMoveDistance,mMaxMoveDistance)));
			wayPoints.Add (mTrans.position );
		}

		//TODO 最適化が必要だ
		void LoopMove(){
			float movedCurrentFrame = mMoveSpeed * Time.deltaTime;
			if(wayPoints!=null ){
				float moved = 0;
				while(moved < movedCurrentFrame){
					Vector3 pos = wayPoints[mCurrentWayPointIndex];
					mTrans.LookAt (pos);
					float dis = Vector3.Distance (mTrans.position,pos);
					if (movedCurrentFrame - moved > dis) {
						moved += dis;
						mCurrentWayPointIndex++;
						mCurrentWayPointIndex = mCurrentWayPointIndex % wayPoints.Count;
						mTrans.position = pos;
					} else {
						mTrans.position += mTrans.forward * (movedCurrentFrame - moved);
						moved = movedCurrentFrame;
					}
				}
			}
		}

	}
}
