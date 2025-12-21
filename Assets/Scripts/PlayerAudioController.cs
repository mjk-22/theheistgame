using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource runSource;      // loop only
    public AudioSource oneShotSource;  // gasp only

    [Header("Running Loop")]
    public AudioClip runLoop;

    [Header("Walking Loop")]
    public AudioClip walkLoop;
    public float minMoveSpeed = 0.2f;

    [Header("Reaction")]
    public AudioClip gaspClip;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Auto-grab if not assigned
        if (runSource == null || oneShotSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length >= 1 && runSource == null) runSource = sources[0];
            if (sources.Length >= 2 && oneShotSource == null) oneShotSource = sources[1];
        }

        if (runSource != null)
        {
            runSource.playOnAwake = false;
            runSource.loop = true;
        }

        if (oneShotSource != null)
        {
            oneShotSource.playOnAwake = false;
            oneShotSource.loop = false;
        }
    }

    void Update()
    {
        if (rb == null || runSource == null) return;

        Vector3 v = rb.velocity;
        v.y = 0f;
        bool isMoving = v.magnitude > minMoveSpeed;

        bool isForwardInput = Input.GetKey(KeyCode.W);
        bool isRunningInput =
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            isForwardInput;
        bool isWalkingInput = isForwardInput && !isRunningInput;

        AudioClip desiredClip = null;
        if (isMoving && isRunningInput && runLoop != null)
            desiredClip = runLoop;
        else if (isMoving && isWalkingInput && walkLoop != null)
            desiredClip = walkLoop;

        if (desiredClip != null)
        {
            if (runSource.clip != desiredClip)
            {
                runSource.Stop();
                runSource.clip = desiredClip;
            }
            if (!runSource.isPlaying) runSource.Play();
        }
        else
        {
            if (runSource.isPlaying) runSource.Stop(); // stop immediately when not moving
        }
    }

    public void PlayGasp()
    {
        if (oneShotSource != null && gaspClip != null)
            oneShotSource.PlayOneShot(gaspClip);
    }
}