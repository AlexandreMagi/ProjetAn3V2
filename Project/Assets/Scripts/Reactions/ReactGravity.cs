using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactGravity
{

    public static void DoFreeze(Entity obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
    }

    public static void DoUnfreeze(Entity obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = false;
    }

    public static void DoPull(Entity obj, Vector3 pullOrigin, float pullForce, bool isAirbone)
    {
        //StopCoroutine("Float");

        DoUnfreeze(obj);

        Vector3 v3DirectionToGo = (pullOrigin - obj.transform.position).normalized;
        Vector3 v3UpperAngle = isAirbone ? new Vector3(0, -.2f, 0) : new Vector3(0, .2f, 0);
        float fDistance = Vector3.Distance(pullOrigin, obj.transform.position);

        //Ça marche, c'est moche, mais on aime

        //En gros, ça attire en fonction de la distance par rapport au sol. Si la distance est inférieure à un seuil, on applique ce seuil au lieu de la distance. Pareil pour la distance max
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(v3DirectionToGo.x, v3UpperAngle.y + (v3DirectionToGo.y / 2), v3DirectionToGo.z) * pullForce * (fDistance < .2f ? Mathf.Pow(2, 1.8f) : fDistance > 10 ? Mathf.Pow(5, 1.8f) : Mathf.Pow(3, 1.8f)));
    }
}
