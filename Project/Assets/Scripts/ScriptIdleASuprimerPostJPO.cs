using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptIdleASuprimerPostJPO : MonoBehaviour
{

    public bool scaleIdle = false;
    float currentScaleModifier = 0;
    [SerializeField] float amplitude = 1;
    [SerializeField] float speed = 1;
    [SerializeField] float delay = 1;
    [SerializeField] float speedGoBack = 3;
    public float refScale = 1;

    // Update is called once per frame
    void Update()
    {
        currentScaleModifier = scaleIdle ? (Mathf.Sin((Time.unscaledTime + delay) * speed) * amplitude) : (Mathf.Lerp(currentScaleModifier, 0, Time.unscaledDeltaTime * speedGoBack));
        transform.localScale = Vector3.one * refScale + Vector3.one * currentScaleModifier;
    }
}
