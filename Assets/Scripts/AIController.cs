using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public AIAnimationController aiAnimationController { get; private set; }
    public StateMachine StateMachine { get; private set; }
    private Rigidbody physicsBody;
    // public Animator Animator { get; private set; } // Not needed since we're not using animations
    public Transform[] Waypoints;
    public Transform Player;

    public float AttackRange = 2f; // New attack range variable
    public LayerMask PlayerLayer;
    public StateType currentState;
    public Vector3 LastKnownPlayerPosition { get; private set; }
    private bool hasLastKnownPlayerPosition;

    [Header("Attack Settings")]
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    [Header("Vision Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float eyeHeight = 1.6f; // where the AI "looks" from
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("Vision Behavior")]
    public bool useVisionPersistence = false;
    public float visionPersistence = 0.5f; // seconds to keep seeing after losing sight
    private float lastSeenTime = -999f;

    [Header("Stuck Handling")]
    public float stuckSpeedThreshold = 0.05f;
    public float stuckDuration = 2f;
    private float stuckTimer;


    // Add State Machine code Here
    void Awake()
    {
        physicsBody = GetComponent<Rigidbody>();
        if (physicsBody != null)
        {
            physicsBody.isKinematic = true;
            physicsBody.useGravity = false;
            physicsBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        aiAnimationController = GetComponent<AIAnimationController>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations

        StateMachine = new StateMachine();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new PatrolState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this)); // Add the new AttackState
        StateMachine.AddState(new SearchState(this));

        StateMachine.TransitionToState(StateType.Idle);
    }

    void Update()
    {
        StateMachine.Update();
        currentState = StateMachine.GetCurrentStateType();
        MonitorStuck();
    }



    // 
    public bool CanSeePlayer()
    {
        if (Player == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPosition = Player.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (targetPosition - eyePosition).normalized;
        float distanceToPlayer = Vector3.Distance(eyePosition, targetPosition);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check field of view
        if (angleToPlayer > viewAngle / 2f)
        {
            return useVisionPersistence && Time.time - lastSeenTime < visionPersistence;
        }

        // Check distance
        if (distanceToPlayer > viewDistance)
        {
            return useVisionPersistence && Time.time - lastSeenTime < visionPersistence;
        }

        // Perform raycast
        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, viewDistance))
        {
            // If hit the player
            if (hit.transform == Player)
            {
                lastSeenTime = Time.time;
                RememberPlayerPosition(hit.point);
                return true;
            }
        }
    

        // If recently seen, still count as visible
        if (useVisionPersistence)
        {
            bool recentlySeen = Time.time - lastSeenTime < visionPersistence;
            return recentlySeen;
        }

        return false;
    }

    public bool CheckHandsCollision(out GameObject collidedObject, string Tag)
    {
        // You can define these in AIController (leftHandPoint, rightHandPoint)
        Transform[] handTransforms = { leftHandTransform, rightHandTransform };

        foreach (Transform hand in handTransforms)
        {
            // Overlap check — sphere or capsule works well for melee hitboxes
            Collider[] hits = Physics.OverlapSphere(hand.position, 0.5f, PlayerLayer);

            foreach (var hit in hits)
            {
                if (hit.CompareTag(Tag))
                {
                    collidedObject = hit.gameObject;
                    return true;
                }
            }
        }
        collidedObject = null;
        return false;
    }
    


    // New method to check if the AI is within attack range
    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        return distanceToPlayer <= AttackRange;
    }

    public void RememberPlayerPosition(Vector3 position)
    {
        LastKnownPlayerPosition = position;
        hasLastKnownPlayerPosition = true;
    }

    public bool TryGetLastKnownPlayerPosition(out Vector3 position)
    {
        position = LastKnownPlayerPosition;
        return hasLastKnownPlayerPosition;
    }

    public void ClearLastKnownPlayerPosition()
    {
        hasLastKnownPlayerPosition = false;
    }

    private void MonitorStuck()
    {
        if (Agent == null || Agent.pathPending)
        {
            stuckTimer = 0f;
            return;
        }

        bool barelyMoving = Agent.velocity.magnitude <= stuckSpeedThreshold;
        bool stillFarFromGoal = Agent.remainingDistance > Agent.stoppingDistance + 0.1f;

        if (barelyMoving && stillFarFromGoal)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckDuration)
            {
                HandleStuck();
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    private void HandleStuck()
    {
        stuckTimer = 0f;
        Agent.ResetPath();
        transform.Rotate(0f, 180f, 0f);

        if (StateMachine == null)
        {
            return;
        }

        StateType current = StateMachine.GetCurrentStateType();
        if (current == StateType.Chase || current == StateType.Search)
        {
            if (TryGetLastKnownPlayerPosition(out Vector3 lastPosition))
            {
                Agent.destination = lastPosition;
                StateMachine.TransitionToState(StateType.Search);
            }
            else
            {
                StateMachine.TransitionToState(StateType.Patrol);
            }
        }
        else if (current == StateType.Patrol)
        {
            StateMachine.TransitionToState(StateType.Patrol);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }

}
