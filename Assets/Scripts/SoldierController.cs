using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class SoldierController : MonoBehaviour
{
    private static readonly int sSpeedHash = Animator.StringToHash("Speed");

    [Serializable]
    public class PatrollingPoint
    {
        public Transform Point;
        public float WaitTime;
    }

    public List<PatrollingPoint> PatrollingPoints;

    private float mWaitTime;
    private PatrollingPoint mCurrentPoint;
    private int mCurrentPointIndex;
    private NavMeshAgent mNavMeshAgent;
    private Animator mAnimator;

    // --------------------------------------------------------------------

    void Start()
    {
        Assert.IsTrue(PatrollingPoints.Count > 1, "The soldier doesn't have enough patrolling points");

        mAnimator = GetComponent<Animator>();
        mNavMeshAgent = GetComponent<NavMeshAgent>();

        mCurrentPoint = PatrollingPoints[0];
        mCurrentPointIndex = 0;

        transform.position = PatrollingPoints[0].Point.position;
        GoToNextPoint();
    }

    // --------------------------------------------------------------------

    void Update()
    {
        if (mWaitTime > 0)
        {
            Wait();
            return;
        }

        if (!mNavMeshAgent.pathPending)
        {
            if (mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance)
            {
                if (!mNavMeshAgent.hasPath || mNavMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    mWaitTime = mCurrentPoint.WaitTime;
                }
            }
        }

        mAnimator.SetFloat(sSpeedHash, mNavMeshAgent.velocity.magnitude);
    }

    // --------------------------------------------------------------------

    private void Wait()
    {
        mWaitTime -= Time.deltaTime;
        if (mWaitTime < 0)
        {
            GoToNextPoint();
        }
    }

    // --------------------------------------------------------------------

    private void GoToNextPoint()
    {
        ++mCurrentPointIndex;
        if (mCurrentPointIndex >= PatrollingPoints.Count)
            mCurrentPointIndex = 0;

        mCurrentPoint = PatrollingPoints[mCurrentPointIndex];
        mNavMeshAgent.destination = mCurrentPoint.Point.position;
    }
}
