using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactGravity<T> where T : DataEntity
{

    //Freezes the game object
    public static void DoFreeze(Entity<T> obj)
    {
        Rigidbody body = obj.GetComponent<Rigidbody>();

        body.constraints = RigidbodyConstraints.FreezeAll;
    }

    //Unfreezes the game object
    public static void DoUnfreeze(Entity<T> obj)
    {
        Rigidbody body = obj.GetComponent<Rigidbody>();

        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }


    //Pulling mechanic
    public static void DoPull(Entity<T> obj, Vector3 pullOrigin, float pullForce, bool isAirbone)
    {
        DoUnfreeze(obj);

        Vector3 v3DirectionToGo = (pullOrigin - obj.transform.position).normalized;
        Vector3 v3UpperAngle = isAirbone ? new Vector3(0, -.2f, 0) : new Vector3(0, .2f, 0);
        float fDistance = Vector3.Distance(pullOrigin, obj.transform.position);

        //Ça marche, c'est moche, mais on aime

        //En gros, ça attire en fonction de la distance par rapport au sol. Si la distance est inférieure à un seuil, on applique ce seuil au lieu de la distance. Pareil pour la distance max
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(v3DirectionToGo.x, v3UpperAngle.y + (v3DirectionToGo.y / 2), v3DirectionToGo.z) * pullForce * (fDistance < .2f ? Mathf.Pow(2, 1.8f) : fDistance > 10 ? Mathf.Pow(5, 1.8f) : Mathf.Pow(3, 1.8f)));
    }

    //Pulling mechanic
    public static void DoPullAsObject(GameObject obj, Vector3 pullOrigin, float pullForce, bool isAirbone)
    {

        Vector3 v3DirectionToGo = (pullOrigin - obj.transform.position).normalized;
        Vector3 v3UpperAngle = isAirbone ? new Vector3(0, -.2f, 0) : new Vector3(0, .2f, 0);
        float fDistance = Vector3.Distance(pullOrigin, obj.transform.position);

        //Ça marche, c'est moche, mais on aime

        //En gros, ça attire en fonction de la distance par rapport au sol. Si la distance est inférieure à un seuil, on applique ce seuil au lieu de la distance. Pareil pour la distance max
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(v3DirectionToGo.x, v3UpperAngle.y + (v3DirectionToGo.y / 2), v3DirectionToGo.z) * pullForce * (fDistance < .2f ? Mathf.Pow(2, 1.8f) : fDistance > 10 ? Mathf.Pow(5, 1.8f) : Mathf.Pow(3, 1.8f)));
    }

    //Items spin while airbone
    public static void DoSpin(Entity<T> obj)
    {
        Vector3 v3SpinRandom = new Vector3(
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20
                    );

        obj.GetComponent<Rigidbody>().AddTorque(v3SpinRandom);
    }

    //Fonction qui enclenche la coroutine de flottaison
    public static void DoFloat(Entity<T> obj, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        Main.Instance.StartCoroutine(Float(obj, tTimeBeforeFloat, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale));
    }

    //Coroutine de flottaison
    private static IEnumerator Float(Entity<T> obj, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        //Attnete avant de démarrer
        yield return new WaitForSecondsRealtime(tTimeBeforeFloat);
        if (obj == null) yield break;
        //Récupération du rigidbody
        Rigidbody rbBody = obj.GetComponent<Rigidbody>();

        float tETime = 0;

        //Reset vélo avant de float
        if (isSlowedDownOnFloat)
            rbBody.velocity /= 50;


        // A chaque passe de gravité, pousse les objets dans la même force que la gravité mais dans le sens inverse
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (rbBody == null) yield break;

            if (!bIndependantFromTimeScale)
                tETime += Time.fixedDeltaTime;
            else
                tETime += Time.fixedDeltaTime / Time.timeScale;

            //rbBody.AddForce(new Vector3(0, -rbBody.mass * (Physics.gravity.y*Time.timeScale), 0));
            rbBody.useGravity = false;


            //Ecrase les objets au sol à la fin de la zero G
            if (tETime >= tFloatTime)
            {
                rbBody.useGravity = true;
                rbBody.AddForce(new Vector3(0, -2000, 0));
                
                yield break;
            }
        }
    }
}
