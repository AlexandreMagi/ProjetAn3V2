using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmerProceduralAnimation : MonoBehaviour
{

    [Header("Torso")]
    [SerializeField] Transform pelvisAnim = null;
    [SerializeField] Transform pelvis = null;
    Transform pelvisRef = null;


    [Header("Legs")]

    [SerializeField] float distanceDetectReplace = 0.5f;
    [SerializeField] float distanceReplace = 0.3f;

    [SerializeField] Transform[] bones = new Transform[0];
    [SerializeField] Transform[] bonesCustoms = new Transform[0];
    [SerializeField] Transform[] bonesCustomsTarget = new Transform[0];
    Transform[] bonesCustomsCurrTarget = new Transform[0];


    [Header("Head")]

    [SerializeField] Transform head = null;
    [SerializeField] Transform lookAt = null;
    [SerializeField] float headMaxTurnAngle = 60;
    [SerializeField] float headTrackingSpeed = 3;

    [Header("Teeth")]

    [SerializeField] Transform[] allTeethsBones = new Transform[0];
    float[] currTeethsRotations = new float[0];
    [SerializeField] Vector3 InitTeethEulerAngle = new Vector3(0, 0, -90);
    [SerializeField] Vector2 randomSpeedTeeth = new Vector2(180, 360);
    float[] teethSpeeds = new float[0];


    void Start()
    {
        pelvisRef = pelvisAnim.transform;
        pelvisRef.position = pelvis.position;

        bonesCustomsCurrTarget = new Transform[bonesCustoms.Length];
        for (int i = 0; i < bonesCustoms.Length; i++)
        {
            bonesCustomsCurrTarget[i] = Instantiate(new GameObject()).transform;
            bonesCustomsCurrTarget[i].name = "LegRefSwarmer";
            bonesCustomsCurrTarget[i].position = bonesCustomsTarget[i].position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized * Random.Range (0f,1f);
        }

        currTeethsRotations = new float[allTeethsBones.Length];
        teethSpeeds = new float[allTeethsBones.Length];
        for (int i = 0; i < allTeethsBones.Length; i++)
        {
            allTeethsBones[i].localEulerAngles = InitTeethEulerAngle;
            currTeethsRotations[i] = Random.Range(randomSpeedTeeth.x, randomSpeedTeeth.y);
            teethSpeeds[i] = Random.Range(0, 360);
            if (i % 2 == 0) teethSpeeds[i] *= -1;
        }

    }

    void Update()
    {
        //HeadRotation();

        distanceReplace = Mathf.Clamp(distanceReplace, 0, distanceDetectReplace);

        LegRotation();

        TeethsRotations();

        pelvisRef.position = pelvis.position;
        pelvisRef.rotation = pelvis.rotation;
    }

    void HeadRotation()
    {
        Quaternion currentLocalRotation = head.localRotation;
        head.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = lookAt.position - head.position;
        Vector3 targetLocalLookDir = head.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * headMaxTurnAngle, 0);

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        head.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
    }

    void LegRotation()
    {
        for (int i = 0; i < bonesCustoms.Length; i++)
        {
            if (Vector3.Distance(bonesCustomsCurrTarget[i].position, bonesCustomsTarget[i].position) > distanceDetectReplace)
            {
                bonesCustomsCurrTarget[i].position = bonesCustomsTarget[i].position + (bonesCustomsTarget[i].position - bonesCustomsCurrTarget[i].position).normalized * distanceReplace;
            }
            bonesCustoms[i].LookAt(bonesCustomsCurrTarget[i].transform.position);

            bones[i].transform.position = bonesCustoms[i].GetChild(0).position;
            bones[i].transform.rotation = bonesCustoms[i].GetChild(0).rotation;
        }
    }
    void TeethsRotations()
    {
        for (int i = 0; i < allTeethsBones.Length; i++)
        {
            allTeethsBones[i].localEulerAngles = InitTeethEulerAngle;

            currTeethsRotations[i] += teethSpeeds[i] * Time.deltaTime;
            if (currTeethsRotations[i] < -360) currTeethsRotations[i] += 360;
            if (currTeethsRotations[i] > 360) currTeethsRotations[i] -= 360;

            allTeethsBones[i].Rotate(Vector3.right * currTeethsRotations[i], Space.Self);

        }
    }



}
