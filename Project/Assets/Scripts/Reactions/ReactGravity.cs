using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactGravity
{

    //Freezes the game object
    public static void DoFreeze(Entity obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
    }

    //Unfreezes the game object
    public static void DoUnfreeze(Entity obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = false;
    }


    //Pulling mechanic
    public static void DoPull(Entity obj, Vector3 pullOrigin, float pullForce, bool isAirbone)
    {
        DoUnfreeze(obj);

        Vector3 v3DirectionToGo = (pullOrigin - obj.transform.position).normalized;
        Vector3 v3UpperAngle = isAirbone ? new Vector3(0, -.2f, 0) : new Vector3(0, .2f, 0);
        float fDistance = Vector3.Distance(pullOrigin, obj.transform.position);

        //Ça marche, c'est moche, mais on aime

        //En gros, ça attire en fonction de la distance par rapport au sol. Si la distance est inférieure à un seuil, on applique ce seuil au lieu de la distance. Pareil pour la distance max
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(v3DirectionToGo.x, v3UpperAngle.y + (v3DirectionToGo.y / 2), v3DirectionToGo.z) * pullForce * (fDistance < .2f ? Mathf.Pow(2, 1.8f) : fDistance > 10 ? Mathf.Pow(5, 1.8f) : Mathf.Pow(3, 1.8f)));
    }

    //Items spin while airbone
    public static void DoSpin(Entity obj)
    {
        Vector3 v3SpinRandom = new Vector3(
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20
                    );

        obj.GetComponent<Rigidbody>().AddTorque(v3SpinRandom);
    }

    public static void DoFloat(Entity obj, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        Main.Instance.StartCoroutine(Float(obj, tTimeBeforeFloat, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale));
    }

    private static IEnumerator Float(Entity obj, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeFloat);

        Rigidbody rbBody = obj.GetComponent<Rigidbody>();

        float tETime = 0;

        //Reset vélo avant de float
        if (isSlowedDownOnFloat)
            rbBody.velocity /= 50;

        while (true)
        {
            yield return new WaitForFixedUpdate();


            if (!bIndependantFromTimeScale)
                tETime += Time.fixedDeltaTime;
            else
                tETime += Time.fixedDeltaTime / Time.timeScale;

            //rbBody.AddForce(new Vector3(0, -rbBody.mass * (Physics.gravity.y*Time.timeScale), 0));
            rbBody.useGravity = false;

            if (tETime >= tFloatTime)
            {
                rbBody.useGravity = true;
                rbBody.AddForce(new Vector3(0, -2000, 0));
                
                yield break;
            }
        }
    }
}
