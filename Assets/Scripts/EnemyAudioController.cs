using System;
using System.Collections;
using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource loopCalm;     // walking/patrol loop
    public AudioSource loopIntense;  // intense loop when close
    public AudioSource sfx;          // roar + attack one-shots

    [Header("Clips")]
    public AudioClip patrolLoop;
    public AudioClip intenseLoop;
    public AudioClip roarClip;
    public AudioClip attackClip;

    [Header("Distance Intensity")]
    public Transform player;
    public float farDistance = 18f;
    public float nearDistance = 6f;
    public float crossfadeSpeed = 5f;

    private bool foundTriggered = false;
    private float lastAttackTime = -999f;
    public float attackCooldown = 0.35f;

    void Awake()
    {
        // Assign clips to the loop sources
        if (loopCalm != null && patrolLoop != null) loopCalm.clip = patrolLoop;
        if (loopIntense != null && intenseLoop != null) loopIntense.clip = intenseLoop;

        // Start both loops (we crossfade volumes)
        if (loopCalm != null)
        {
            loopCalm.playOnAwake = false;
            loopCalm.loop = true;
            if (!loopCalm.isPlaying) loopCalm.Play();
            loopCalm.volume = 1f;
        }

        if (loopIntense != null)
        {
            loopIntense.playOnAwake = false;
            loopIntense.loop = true;
            if (!loopIntense.isPlaying) loopIntense.Play();
            loopIntense.volume = 0f;
        }

        if (sfx != null)
        {
            sfx.playOnAwake = false;
            sfx.loop = false;
        }
    }

    void Update()
    {
        if (player == null || loopCalm == null || loopIntense == null) return;

        float d = Vector3.Distance(transform.position, player.position);
        float t = Mathf.InverseLerp(farDistance, nearDistance, d); // far->0, near->1
        t = Mathf.Clamp01(t);

        float targetIntense = t;
        float targetCalm = 1f - t;

        loopIntense.volume = Mathf.MoveTowards(loopIntense.volume, targetIntense, Time.deltaTime * crossfadeSpeed);
        loopCalm.volume = Mathf.MoveTowards(loopCalm.volume, targetCalm, Time.deltaTime * crossfadeSpeed);
    }

    // Call this when the enemy FIRST finds the player:
    // It will roar, then run your "start chase" action.
    public void RoarThenChase(Action startChase)
    {
        if (foundTriggered)
        {
            startChase?.Invoke();
            return;
        }

        foundTriggered = true;

        if (roarClip == null || sfx == null)
        {
            startChase?.Invoke();
            return;
        }

        StartCoroutine(RoarRoutine(startChase));
    }

    private IEnumerator RoarRoutine(Action startChase)
    {
        sfx.PlayOneShot(roarClip);
        yield return new WaitForSeconds(roarClip.length);
        startChase?.Invoke();
    }

    // Call this on attack hit
    public void PlayAttackSfx()
    {
        if (attackClip == null || sfx == null) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        sfx.PlayOneShot(attackClip);
    }

    // Optional: if player escapes and you want roar to happen again later
    public void ResetFound()
    {
        foundTriggered = false;
    }
}