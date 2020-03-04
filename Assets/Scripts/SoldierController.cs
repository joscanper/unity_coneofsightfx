using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SoldierController : MonoBehaviour
{
    [Serializable]
    public class PatrollingPoint
    {
        public Transform Point;
        public float WaitTime;
    }

    public List<PatrollingPoint> PatrollingPoints;
    private float mTimeWaited;
    private PatrollingPoint mCurrentPoint;
    private int mCurrentPointIndex;

    void Start()
    {
        Assert.IsTrue(PatrollingPoints.Count > 0, "The soldier doesn't have any patrolling points");

        mCurrentPoint = PatrollingPoints[0];
        mCurrentPointIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mTimeWaited += Time.deltaTime;
        if (mTimeWaited > mCurrentPoint.WaitTime)
        {
            ++mCurrentPointIndex;
            if (mCurrentPointIndex >= PatrollingPoints.Count)
                mCurrentPointIndex = 0;

            mCurrentPoint = PatrollingPoints[mCurrentPointIndex];


        }
    }
}
