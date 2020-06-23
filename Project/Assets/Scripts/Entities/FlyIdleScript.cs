using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyIdleScript : MonoBehaviour
{

    [SerializeField] Swarmer swarmerParent = null;
    [SerializeField] float timeGoToFly = 0.5f;
    [SerializeField] float timeGoToNormal = 0.5f;

    float purcentageFlying = 1;

    [SerializeField] float flyAmplitude = 0.4f;
    [SerializeField] float flySpeed = 3f;
    float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (swarmerParent == null) this.enabled = false;

        delay = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        purcentageFlying = Mathf.MoveTowards(purcentageFlying, swarmerParent.IsGravityAffected ? 0 : 1, Time.deltaTime / (swarmerParent.IsGravityAffected ? timeGoToNormal : timeGoToFly));
        float valueFly = (Mathf.Sin((Time.time + delay) * flySpeed) / 2 + 1) * flyAmplitude;
        float valueFluidified = valueFly * AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(purcentageFlying);
        transform.localPosition = Vector3.up * valueFluidified;
    }
}
