using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmerMeshAnimator : MonoBehaviour
{

    [SerializeField] Transform baseBox = null;
    [SerializeField] Transform upBox = null;
    [SerializeField] Transform refForUpBox = null;
    [SerializeField] float maxDist = 0.3f;

    [SerializeField] Transform head = null;
    [SerializeField] Transform lookAt = null;
    [SerializeField] float headMaxTurnAngle = 60;
    [SerializeField] float headTrackingSpeed = 3;

    [SerializeField] Transform[] legs = new Transform[0];
    [SerializeField] Leg[] legHandlers = new Leg[0];
    [SerializeField] Transform[] legsRefs = new Transform[0];

    [SerializeField] float legDistForStep = 0.3f;
    [SerializeField] float legStepTime = 0.3f;

    [SerializeField] AnimationCurve stepRotateX;
    [SerializeField] float stepRotateMultiplier = 90;

    [SerializeField] float purcentageFollowHead = 0;
    [SerializeField] float purcentageFollowBottomBox = 0;

    [SerializeField] float randomAddedPos = 0.5f;

    Quaternion trueRotation;

    // Start is called before the first frame update
    void Start()
    {
        upBox.parent= null;
        legHandlers = new Leg[legs.Length];
        for (int i = 0; i < legs.Length; i++)
        {
            legHandlers[i] = new Leg(legsRefs[i].position);
        }
        trueRotation = refForUpBox.rotation;

        if (lookAt == null) lookAt = Player.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(upBox.position.y - refForUpBox.position.y) < maxDist)
            upBox.position = new Vector3(refForUpBox.position.x, Mathf.Lerp(upBox.position.y, refForUpBox.position.y, Time.deltaTime * 5), refForUpBox.position.z);
        else
            upBox.position = new Vector3(refForUpBox.position.x, refForUpBox.position.y + (maxDist * Mathf.Sign(upBox.position.y - refForUpBox.position.y)), refForUpBox.position.z);

        baseBox.localRotation = Quaternion.identity;
        baseBox.rotation = Quaternion.Lerp(baseBox.rotation, head.rotation, purcentageFollowBottomBox);
        trueRotation = Quaternion.Lerp(trueRotation, refForUpBox.rotation, Time.deltaTime * 5);
        upBox.rotation = Quaternion.Lerp(trueRotation, head.rotation, purcentageFollowHead);


        HeadRotation();
        LegsRotation();

    }

    void HeadRotation()
    {
        Quaternion currentLocalRotation = head.localRotation;
        head.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = lookAt.position - head.position;
        Vector3 targetLocalLookDir = head.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * headMaxTurnAngle,0);

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        head.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
    }

    void LegsRotation()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            if (!legHandlers[i].onStep && Vector3.Distance (legHandlers[i].posActual, legsRefs[i].position) > legDistForStep)
            {
                legHandlers[i].onStep = true;
                legHandlers[i].currentStepPurcentage = 0;
                legHandlers[i].pastPos = legHandlers[i].posActual;
                legHandlers[i].posActual = legsRefs[i].position + Vector3.Normalize(legsRefs[i].position - legHandlers[i].posActual) * 0.08f + new Vector3(Random.Range(-randomAddedPos, randomAddedPos),Random.Range(-randomAddedPos, randomAddedPos),Random.Range(-randomAddedPos, randomAddedPos));
            }
            else
            {
                if (legHandlers[i].onStep)
                {
                    if (legHandlers[i].currentStepPurcentage > 1 - Time.deltaTime)
                    {
                        legHandlers[i].onStep = false;
                    }
                    else
                    {
                        Vector3 targetLookDir = legHandlers[i].posActual - legs[i].position;
                        Quaternion targetRotation = Quaternion.LookRotation(targetLookDir, Vector3.up);
                        Vector3 _targetLookDir = legHandlers[i].pastPos - legs[i].position;
                        Quaternion _targetRotation = Quaternion.LookRotation(_targetLookDir, Vector3.up);

                        legs[i].rotation = Quaternion.Lerp(_targetRotation, targetRotation, legHandlers[i].currentStepPurcentage);
                        legs[i].Rotate(Vector3.right * stepRotateX.Evaluate(legHandlers[i].currentStepPurcentage) * stepRotateMultiplier, Space.Self);

                        legHandlers[i].currentStepPurcentage += Time.deltaTime / legStepTime;
                    }
                }
                else
                {
                    legs[i].LookAt(legHandlers[i].posActual, Vector3.up);
                }
            }
        }
    }
}

public class Leg
{
    public bool onStep = false;
    public float currentStepPurcentage = 0;
    public Vector3 posActual = Vector3.zero;
    public Vector3 pastPos = Vector3.zero;

    public Leg(Vector3 posInit)
    {
        posActual = posInit;
    }
}
