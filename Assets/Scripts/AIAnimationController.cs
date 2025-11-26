using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Animator))]

public class AIAnimationController : MonoBehaviour
{
    public Animator animator { get; private set; }
    private AIController aiController;
    private NavMeshAgent agent;

    void Awake()
    {
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIController>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        // Update animation parameters
        UpdateAnimations();
    }
    void UpdateAnimations()
    {
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat("CharacterSpeed", speed);
    }

    void HitPlayer() // Attack Animation Event to check if the player his hit
    {
        if (aiController == null)
        {
            return;
        }

        GameObject objectHit;
        if (aiController.CheckHandsCollision(out objectHit, "Player"))
        {
            // Try to respawn using GameManager if available
            GameManager gm = GameManager.Instance != null ? GameManager.Instance : FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.GameOver();
            }
            else
            {
                // Fallback: reload scene (same as reference code)
                SceneManager.LoadScene("LoseScreen");
            }
        }
    }

}
