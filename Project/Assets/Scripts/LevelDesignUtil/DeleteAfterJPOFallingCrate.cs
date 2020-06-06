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
            AudioSource keskitombSon = CustomSoundManager.Instance.PlaySound("SE_Snap_Cable", "Effect", 1.3f);
            if (keskitombSon != null)
            {
                keskitombSon.spatialBlend = 1;
                keskitombSon.minDistance = 8;
                keskitombSon.transform.position = transform.position;

            }
        }
    }
}
