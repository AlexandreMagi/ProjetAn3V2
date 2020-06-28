using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactGravity<T> where T : DataEntity
{

    //Freezes the game object
    public static void DoFreeze(Rigidbody rb)
    {
        if (rb != null)
           rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    //Unfreezes the game object
    public static void DoUnfreeze(Rigidbody rb)
    {
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }


    //Pulling mechanic
    public static void DoPull(Rigidbody rb, Vector3 pullOrigin, float pullForce, bool isAirbone, bool isReppelForce = false, Vector3? normalRepel = null)
    { 
        //DoUnfreeze(rb);
        if(rb != null)
        {
            Vector3 v3DirectionToGo = (pullOrigin - rb.transform.position).normalized;

            //Normalisation de la trajectoire
            if (isReppelForce)
            {
                
                if(normalRepel == null)
                {
                    normalRepel = Vector3.up;
                }

                Vector3 trueNormal = (Vector3)normalRepel;

                v3DirectionToGo = new Vector3(Random.Range(-1f, 1f), Random.Range(.2f, .8f) * (Random.Range(0,2) == 0 ? -1 : 1), Random.Range(-1f, 1f));

                //Trouver le vecteur perpendiculaire à l'axe de la normale
                Vector3 perpendicularVector = Vector3.ProjectOnPlane(v3DirectionToGo, trueNormal);

                //Calcul de la nouvelle direction en fonction de l'axe
                v3DirectionToGo = (trueNormal + perpendicularVector).normalized;

                //Sécurité pour éviter d'envoyer les swarmers trop proches du player
                Vector3 playerPosition = CameraHandler.Instance.GetCurrentCam().transform.position;

                if(Vector2.Angle(new Vector2(v3DirectionToGo.x, v3DirectionToGo.z), new Vector2(playerPosition.x, playerPosition.z)) < 30 && Vector3.Distance(playerPosition, rb.transform.position) <= 10)
                {
                    v3DirectionToGo = new Vector3(-v3DirectionToGo.x, v3DirectionToGo.y, -v3DirectionToGo.z);
                }

                //Debug.Log($"Nom : {rb.gameObject.name} -- Direction {v3DirectionToGo} -- Normal {trueNormal}");
                //Debug.DrawRay(rb.transform.position, v3DirectionToGo, Color.green);
                //Debug.Break();

                //v3DirectionToGo = new Vector3(v3DirectionToGo.x * 1.2f, v3DirectionToGo.y * .8f, v3DirectionToGo.z * 1.2f).normalized;

                //if (v3DirectionToGo.y < 0) pullForce *= 1.5f;

                if (rb.GetComponent<Mannequin>() != null)
                    Debug.Log($"Direction : {v3DirectionToGo}");

                rb.velocity = (v3DirectionToGo * pullForce * Mathf.Pow(2f, 1.9f) / rb.mass);

                if (rb.GetComponent<Mannequin>() != null)
                    Debug.Log($"Velocity : {rb.velocity}");

                return;
            }

            //float deltaY = Mathf.Abs(rb.transform.position.y - pullOrigin.y);

            float fDistance = Vector3.Distance(pullOrigin, rb.transform.position);

            //Debug.Log($"Pullorigin : {pullOrigin} -- Pullforce = {pullForce} -- isAirbone = {isAirbone} -- Direction calculated : {v3DirectionToGo}");

            //Debug.DrawRay(rb.transform.position, v3DirectionToGo, Color.green);

            //Debug.Break();

            //(v3DirectionToGo.y / 2) * .15f

            //Ça marche, c'est moche, mais on aime
            //En gros, ça attire en fonction de la distance par rapport au sol.
            rb.velocity = (v3DirectionToGo * pullForce * (fDistance > 8 ? Mathf.Pow(3, 1.9f) : fDistance <= .5f ? Mathf.Pow(1.2f, 1.9f) : Mathf.Pow(2.5f, 1.9f))) / rb.mass;
        }

    }

    //Items spin while airbone
    public static void DoSpin(Rigidbody rb)
    {
        if (rb != null)
        {
            Vector3 v3SpinRandom = new Vector3(
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20,
                        Random.Range(-1f, 1f) * 20
                    );

            rb.AddTorque(v3SpinRandom);
        }
            
    }

    //Fonction qui enclenche la coroutine de flottaison
    public static void DoFloat(Rigidbody rb, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        if (rb != null)
            Main.Instance.StartCoroutine(Float(rb, tTimeBeforeFloat, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale));
    }

    //Coroutine de flottaison
    private static IEnumerator Float(Rigidbody rb, float tTimeBeforeFloat, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        //Attnete avant de démarrer
        yield return new WaitForSecondsRealtime(tTimeBeforeFloat);

        if (rb == null) yield break;

        float tETime = 0;

        //Reset vélo avant de float
        if (isSlowedDownOnFloat)
            rb.velocity /= 50;


        // A chaque passe de gravité, pousse les objets dans la même force que la gravité mais dans le sens inverse
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (rb == null) yield break;

            if (!bIndependantFromTimeScale)
                tETime += Time.fixedDeltaTime;
            else
                tETime += Time.fixedDeltaTime / Time.timeScale;

            //rbBody.AddForce(new Vector3(0, -rbBody.mass * (Physics.gravity.y*Time.timeScale), 0));
            rb.useGravity = false;


            //Ecrase les objets au sol à la fin de la zero G
            if (tETime >= floatTime)
            {
                rb.useGravity = true;
                rb.AddForce(new Vector3(0, -2000, 0));
                
                
                yield break;
            }
        }
    }
}
