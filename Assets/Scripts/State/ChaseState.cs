using UnityEngine;
public class ChaseState : IState
{
    private AIController aiController;

    public StateType Type => StateType.Chase;

    public ChaseState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // No animations, so no need to set any animator parameters
    }

    public void Execute()
    {
        if (!aiController.CanSeePlayer())
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
        if (aiController.IsPlayerInAttackRange())
        {
            aiController.StateMachine.TransitionToState(StateType.Attack);
            return;
        }

        aiController.Agent.destination = aiController.Player.position;
    }

    public void Exit()
    {
        // No cleanup necessary
    }
}

