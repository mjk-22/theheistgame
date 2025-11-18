using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxManager : MonoBehaviour
{
    public Collider[] attackColliders;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Collider attackColliders in attackColliders)
        {
            attackColliders.enabled = false; //disable collider at start
        }
    }

    public void EnableHitbox()
    {
        foreach (Collider attackColliders in attackColliders)
        {
            attackColliders.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        foreach (Collider attackColliders in attackColliders)
        {
            attackColliders.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
