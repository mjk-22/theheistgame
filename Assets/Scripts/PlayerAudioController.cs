using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource runSource;      // loop only
    public AudioSource oneShotSource;  // gasp only

    [Header("Running Loop")]
    public AudioClip runLoop;
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

        bool isRunningInput =
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            Input.GetKey(KeyCode.W);

        bool shouldPlayRun = isMoving && isRunningInput;

        if (shouldPlayRun)
        {
            if (runLoop != null && runSource.clip != runLoop) runSource.clip = runLoop;
            if (!runSource.isPlaying) runSource.Play();
        }
        else
        {
            if (runSource.isPlaying) runSource.Stop(); // only stops the loop now
        }
    }

    public void PlayGasp()
    {
        if (oneShotSource != null && gaspClip != null)
            oneShotSource.PlayOneShot(gaspClip);
    }
}