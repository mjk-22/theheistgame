using UnityEngine;

public class SearchState : IState
{
    private readonly AIController aiController;
    private readonly float searchDuration = 4f;
    private readonly float lookAroundSpeed = 120f;
    private float searchTimer;
    private bool movingToLastKnownPoint;

    public StateType Type => StateType.Search;

    public SearchState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        searchTimer = 0f;

        if (aiController.TryGetLastKnownPlayerPosition(out Vector3 lastKnown))
        {
            aiController.Agent.isStopped = false;
            aiController.Agent.destination = lastKnown;
            movingToLastKnownPoint = true;
        }
        else
        {
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        if (movingToLastKnownPoint)
        {
            if (!aiController.Agent.pathPending && aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
            {
                movingToLastKnownPoint = false;
                aiController.Agent.isStopped = true;
            }
            return;
        }

        searchTimer += Time.deltaTime;
        aiController.transform.Rotate(Vector3.up, lookAroundSpeed * Time.deltaTime);

        if (searchTimer >= searchDuration)
        {
            aiController.Agent.isStopped = false;
            aiController.ClearLastKnownPlayerPosition();
            aiController.StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}

