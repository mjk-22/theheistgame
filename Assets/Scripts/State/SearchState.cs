using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SearchState : IState
{
    private readonly AIController aiController;
    private readonly float searchPointWaitDuration = 3f; // Time to wait at each search point
    private float searchTimer;
    private bool movingToLastKnownPoint;
    private bool searchingWaypoints;
    private int currentSearchPointIndex = 0;
    private List<Transform> searchPoints = new List<Transform>();
    private int searchPointsVisited = 0;
    private readonly int maxSearchPointsToVisit = 3; // Visit up to 3 search points before giving up

    public StateType Type => StateType.Search;

    public SearchState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        searchTimer = 0f;
        searchingWaypoints = false;
        searchPointsVisited = 0;
        currentSearchPointIndex = 0;

        // Get waypoints as search points
        if (aiController.Waypoints != null && aiController.Waypoints.Length > 0)
        {
            searchPoints = aiController.Waypoints.ToList();
        }
        else
        {
            searchPoints.Clear();
        }

        // First, go to last known player position
        if (aiController.TryGetLastKnownPlayerPosition(out Vector3 lastKnown))
        {
            aiController.Agent.isStopped = false;
            aiController.Agent.destination = lastKnown;
            movingToLastKnownPoint = true;
        }
        else
        {
            // No last known position, start searching waypoints immediately
            if (searchPoints.Count > 0)
            {
                StartSearchingWaypoints();
            }
            else
            {
                // No waypoints either, go back to patrol
                aiController.StateMachine.TransitionToState(StateType.Patrol);
            }
        }
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        // First phase: Move to last known player position
        if (movingToLastKnownPoint)
        {
            if (!aiController.Agent.pathPending && aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                movingToLastKnownPoint = false;
                // Reached last known position, now start searching waypoints
                if (searchPoints.Count > 0)
                {
                    StartSearchingWaypoints();
                }
                else
                {
                    // No waypoints, wait briefly then go to patrol
                    searchTimer = 0f;
                }
            }
            return;
        }

        // Second phase: Search through waypoints
        if (searchingWaypoints)
        {
            // Check if we've reached the current search point
            if (!aiController.Agent.pathPending && aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                // Wait at this search point
                if (searchTimer == 0f)
                {
                    // Just arrived, start waiting
                    aiController.Agent.isStopped = true;
                    searchTimer = 0f;
                }

                searchTimer += Time.deltaTime;

                // After waiting, move to next search point
                if (searchTimer >= searchPointWaitDuration)
                {
                    searchPointsVisited++;
                    searchTimer = 0f;

                    // If we've visited enough search points, give up and go to patrol
                    if (searchPointsVisited >= maxSearchPointsToVisit)
                    {
                        aiController.Agent.isStopped = false;
                        aiController.ClearLastKnownPlayerPosition();
                        aiController.StateMachine.TransitionToState(StateType.Patrol);
                        return;
                    }

                    // Move to next search point
                    MoveToNextSearchPoint();
                }
            }
            return;
        }

        // Fallback: If we're not moving to last known and not searching waypoints, wait briefly then patrol
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchPointWaitDuration)
        {
            aiController.Agent.isStopped = false;
            aiController.ClearLastKnownPlayerPosition();
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    private void StartSearchingWaypoints()
    {
        if (searchPoints.Count == 0)
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
            return;
        }

        searchingWaypoints = true;
        searchTimer = 0f;
        currentSearchPointIndex = 0;
        
        // Find the closest waypoint to start searching from
        if (aiController.TryGetLastKnownPlayerPosition(out Vector3 lastKnown))
        {
            float closestDistance = float.MaxValue;
            int closestIndex = 0;
            
            for (int i = 0; i < searchPoints.Count; i++)
            {
                if (searchPoints[i] == null) continue;
                float distance = Vector3.Distance(lastKnown, searchPoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
            currentSearchPointIndex = closestIndex;
        }

        MoveToNextSearchPoint();
    }

    private void MoveToNextSearchPoint()
    {
        if (searchPoints.Count == 0)
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
            return;
        }

        aiController.Agent.isStopped = false;
        
        // Move to current search point
        if (searchPoints[currentSearchPointIndex] != null)
        {
            aiController.Agent.destination = searchPoints[currentSearchPointIndex].position;
        }

        // Move to next search point (cycle through)
        currentSearchPointIndex = (currentSearchPointIndex + 1) % searchPoints.Count;
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}

