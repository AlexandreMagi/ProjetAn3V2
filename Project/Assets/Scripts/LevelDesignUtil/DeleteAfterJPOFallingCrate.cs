using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterJPOFallingCrate : MonoBehaviour
{
    [SerializeField]
    GameObject trigger = null;

    CapsuleCollider col;

    Rigidbody rb;

    bool safe = false;

    private void Start()
    {
        col = trigger.GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (col.enabled == false && !safe)
        {
            rb.isKinematic = false;
            safe = true;
        }
    }
}
