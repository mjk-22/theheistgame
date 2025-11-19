using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public AIAnimationController aiAnimationController { get; private set; }
    public StateMachine StateMachine { get; private set; }
    // public Animator Animator { get; private set; } // Not needed since we're not using animations
    public Transform[] Waypoints;
    public Transform Player;

    public float AttackRange = 2f; // New attack range variable
    public LayerMask PlayerLayer;
    public StateType currentState;

    public Transform leftHandTransform;
    public Transform rightHandTransform;

    public float viewDistance = 10f;
    public float viewAngle = 360f; // 360 degrees - full circle vision
    public float eyeHeight = 1.6f; // where the AI looks from
    public LayerMask obstacleMask;
    public LayerMask playerMask;

<<<<<<< HEAD
    [Header("Vision Stability")]
    public float visionPersistence = 0.5f; // seconds to keep seeing after losing sight
=======
    [Header("Vision Behavior")]
    public bool useVisionPersistence = true;
    public float visionPersistence = 5f; // seconds to keep seeing after losing sight (increased for more persistence)
>>>>>>> b175fb3 (Pulled from main)
    private float lastSeenTime = -999f;


    // Add State Machine code Here
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        aiAnimationController = GetComponent<AIAnimationController>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations

        // Auto-find player if not assigned
        if (Player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Player = playerObj.transform;
            }
        }

        StateMachine = new StateMachine();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new PatrolState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this)); // Add the new AttackState

        StateMachine.TransitionToState(StateType.Idle);
    }

    void Update()
    {
        StateMachine.Update();
        currentState = StateMachine.GetCurrentStateType();
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
<<<<<<< HEAD
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check field of view
        if (angleToPlayer > viewAngle / 2f)
        {
            return Time.time - lastSeenTime < visionPersistence;
        }
=======
        
        // With 360 degree vision, we don't need to check angle - just distance and line of sight
>>>>>>> b175fb3 (Pulled from main)

        // Check distance
        if (distanceToPlayer > viewDistance)
        {
            return Time.time - lastSeenTime < visionPersistence;
        }

        // Perform raycast to check line of sight
        RaycastHit hit;
        bool hasLineOfSight = false;
        
        if (Physics.Raycast(eyePosition, directionToPlayer, out hit, viewDistance))
        {
            // Check if we hit the player or a child of the player
            Transform hitTransform = hit.transform;
            
            // Check if hit is the player or any child of the player
            bool hitPlayer = (hitTransform == Player || hitTransform.IsChildOf(Player));
            
            if (hitPlayer)
            {
                lastSeenTime = Time.time;
                return true;
            }
            
            // If we hit something else, check if it's an obstacle blocking our view
            if (obstacleMask.value != 0)
            {
                int hitLayer = hitTransform.gameObject.layer;
                if ((obstacleMask.value & (1 << hitLayer)) != 0)
                {
                    // Hit an obstacle before the player, can't see player
                    return useVisionPersistence && Time.time - lastSeenTime < visionPersistence;
                }
            }
            
            // If we hit something that's not the player and not an obstacle,
            // check if the hit distance is close to player distance (might be player's collider)
            float hitDistance = Vector3.Distance(eyePosition, hit.point);
            float playerDistance = distanceToPlayer;
            
            // If hit is very close to where player should be, assume we can see them
            if (Mathf.Abs(hitDistance - playerDistance) < 0.5f)
            {
                hasLineOfSight = true;
            }
        }
        else
        {
            // Raycast didn't hit anything - if player is in range and FOV, assume we can see them
            // (might happen if player collider is on a child object or disabled)
            hasLineOfSight = true;
        }
        
        // If we have line of sight (no obstacles), player is visible
        if (hasLineOfSight)
        {
            lastSeenTime = Time.time;
            RememberPlayerPosition(targetPosition);
            return true;
        }

        // If recently seen, still count as visible
        bool recentlySeen = Time.time - lastSeenTime < visionPersistence;

        return recentlySeen;
    }

    public bool CheckHandsCollision(out GameObject collidedObject, string Tag)
    {
        collidedObject = null;
        
        // First, check if player is in attack range using simple distance check
        if (Player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            if (distanceToPlayer <= AttackRange + 0.5f) // Add small buffer
            {
                // Player is close enough, check if they have the correct tag
                if (Player.CompareTag(Tag))
                {
                    collidedObject = Player.gameObject;
                    return true;
                }
            }
        }
        
        // Check if hand transforms are assigned
        if (leftHandTransform == null || rightHandTransform == null)
        {
            return false;
        }

        // You can define these in AIController (leftHandPoint, rightHandPoint)
        Transform[] handTransforms = { leftHandTransform, rightHandTransform };
        float checkRadius = 0.8f; // Increased radius for better detection

        foreach (Transform hand in handTransforms)
        {
            if (hand == null) continue;

            // Overlap check — sphere or capsule works well for melee hitboxes
            Collider[] hits;
            
            // If PlayerLayer is set, use it; otherwise check all colliders
            if (PlayerLayer.value != 0)
            {
                hits = Physics.OverlapSphere(hand.position, checkRadius, PlayerLayer);
            }
            else
            {
                // Fallback: check all colliders and filter by tag
                hits = Physics.OverlapSphere(hand.position, checkRadius);
            }

            foreach (var hit in hits)
            {
                if (hit != null && hit.CompareTag(Tag))
                {
                    collidedObject = hit.gameObject;
                    return true;
                }
            }
        }
        
        return false;
    }
    


    // New method to check if the AI is within attack range
    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        return distanceToPlayer <= AttackRange;
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
