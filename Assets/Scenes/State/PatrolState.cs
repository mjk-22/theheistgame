using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PatrolState : IState
{
    private AIController aiController;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    public StateType Type => StateType.Patrol;

    public PatrolState(AIController aiController)
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

        agent.isStopped = false;
        MoveToNextWaypoint();
    }


    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        var agent = aiController.Agent;
        if (agent == null || !agent.isOnNavMesh) return;

        if (!isWaiting && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            aiController.StartCoroutine(WaitAndAnimate());
        }
    }

    public void Exit()
    {
        var agent = aiController.Agent;
        if (agent == null || !agent.isOnNavMesh) return;
        agent.isStopped = false;
    }

    private IEnumerator WaitAndAnimate()
    {
        isWaiting = true;
        var agent = aiController.Agent;
        if (agent == null || !agent.isOnNavMesh)
        {
            isWaiting = false;
            yield break;
        }
        agent.isStopped = true;

        // Play patrol point animation
        aiController.aiAnimationController.animator.SetTrigger("doScream");

        // Wait for animation duration (1.5 sec here, adjust to your animation length)
        yield return new WaitForSeconds(5);

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = false;
        MoveToNextWaypoint();
        isWaiting = false;
    }

    private void MoveToNextWaypoint()
    {
        var agent = aiController.Agent;
        if (agent == null || !agent.isOnNavMesh) return;

        if (aiController.Waypoints == null || aiController.Waypoints.Length == 0)
            return;

        agent.SetDestination(aiController.Waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % aiController.Waypoints.Length;
    }
}
