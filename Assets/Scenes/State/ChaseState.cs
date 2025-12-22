using UnityEngine;
using UnityEngine.AI;
public class ChaseState : IState
{
    private AIController aiController;
    private float chasePersistenceDuration = 8f; // seconds to keep chasing after losing sight (increased for more persistence)
    private float lastSeenTime = -999f;
    private bool wasSeeingPlayer = false;
    private Vector3 lastChasePosition;

    public StateType Type => StateType.Chase;

    public ChaseState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        var agent = aiController.Agent;
        if (agent == null) return;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(agent.transform.position, out var hit, 5f, NavMesh.AllAreas))
                agent.Warp(hit.position);
            else
                return;
        }

        aiController.Agent.isStopped = false;

        if (aiController.Player != null)
            agent.SetDestination(aiController.Player.position);
    }

    public void Execute()
    {
        var agent = aiController.Agent;
        if (agent == null || !agent.isOnNavMesh) return;
        if (aiController.Agent == null)
        {
            return;
        }

        bool canSeePlayer = aiController.CanSeePlayer();
        
        // Update last seen time if we can see the player
        if (canSeePlayer && aiController.Player != null)
        {
            lastSeenTime = Time.time;
            wasSeeingPlayer = true;
            // Update last chase position while we can see the player
            lastChasePosition = aiController.Player.position;
        }

        // Check if we should continue chasing after losing sight
        float timeSinceLastSeen = Time.time - lastSeenTime;
        bool shouldContinueChasing = wasSeeingPlayer && (timeSinceLastSeen < chasePersistenceDuration);
        
        // If we can't see the player and persistence time has expired, transition to search
        if (!canSeePlayer && !shouldContinueChasing)
        {
            if (aiController.TryGetLastKnownPlayerPosition(out _))
            {
                aiController.StateMachine.TransitionToState(StateType.Search);
            }
            else
            {
                aiController.StateMachine.TransitionToState(StateType.Patrol);
            }
            return;
        }

        // If player is in attack range, attack
        if (canSeePlayer && aiController.IsPlayerInAttackRange())
        {
            aiController.StateMachine.TransitionToState(StateType.Attack);
            return;
        }

        // Continue chasing - either directly to player or to last known position
        if (canSeePlayer && aiController.Player != null)
        {
            // Chase directly to player
            aiController.Agent.isStopped = false;
            aiController.Agent.SetDestination(aiController.Player.position);
        }
        else if (shouldContinueChasing)
        {
            // Continue chasing to last known position during persistence period
            aiController.Agent.isStopped = false;
            
            // Try to get last known position, otherwise use the last chase position we tracked
            Vector3 targetPosition;
            if (aiController.TryGetLastKnownPlayerPosition(out targetPosition))
            {
                aiController.Agent.destination = targetPosition;
            }
            else if (lastChasePosition != Vector3.zero)
            {
                // Fallback to the position we were chasing when we lost sight
                aiController.Agent.destination = lastChasePosition;
            }
        }
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

