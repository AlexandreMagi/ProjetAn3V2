using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBodyPart : MonoBehaviour
{

    Renderer meshRenderer;
    Material instancedMaterial;

    float phosphoValue = 1;

    [SerializeField]
    bool forceNoSpawn = false;

    [SerializeField]
    float minTimerBeforeDisapear = 2;

    [SerializeField]
    float maxTimerBeforeDisapear = 5;

    float timerBeforeDisapear;
    float timerBeforeDisapearIncrement;

    Prop prop;

    [SerializeField]
    float explosionForceOnSpawn = 300f;

    [SerializeField]
    bool isAlwaysSpawning = true;

    [SerializeField]
    bool isPhophoAffected = false;

    [SerializeField]
    bool isEnemyPart = false;

    bool isActiveAndVisible = true;

    float currSize = 1;


    [SerializeField]
    float scaleMultiplier = 1;

    [SerializeField]
    Collider collid19 = null;


    void Start()
    {
        if (isPhophoAffected)
        {
            meshRenderer = gameObject.GetComponent<Renderer>();
            instancedMaterial = meshRenderer.materials[1];
        }
        prop = GetComponent<Prop>();
        collid19 = GetComponent<Collider>();
    }

    public void CheckIfMustPop(Vector3 pos, Vector3 decalAllPos)
    {
        if (!forceNoSpawn)
        {
            //if (BodyPartsManager.Instance != null)
            //    BodyPartsManager.Instance.NotifyApparition(this);

            if (!isAlwaysSpawning)
            {
                int rand;
                rand = Random.Range(0, 2);

                if (rand == 0) gameObject.SetActive(false);
                else Pop(pos, decalAllPos);
            }
            else Pop(pos, decalAllPos);
        }
    }

    void Pop(Vector3 posInit, Vector3 decalAllPos)
    {
        gameObject.SetActive(true);
        timerBeforeDisapear = Random.Range(minTimerBeforeDisapear, maxTimerBeforeDisapear);
        transform.rotation = Random.rotation;
        transform.position = posInit + Random.insideUnitSphere + decalAllPos;
        Rigidbody rb;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(AddExplosionEffect(rb, posInit));
        StartCoroutine(ActivateCollider());
        transform.localScale = Vector3.one;
        isActiveAndVisible = true;
        timerBeforeDisapearIncrement = 0;
        phosphoValue = 1;
        if (collid19 != null) collid19.enabled = false;
    }

    public void Depop()
    {
        transform.localScale = new Vector3(0, 0, 0);
        isActiveAndVisible = false;
        gameObject.SetActive(false);
        if (collid19 != null) collid19.enabled = false;
    }

    IEnumerator ActivateCollider()
    {
        yield return new WaitForEndOfFrame();
        if (collid19 != null) collid19.enabled = true;
        yield break;
    }

    IEnumerator AddExplosionEffect(Rigidbody rb, Vector3 posInit)
    {
        yield return new WaitForEndOfFrame();

        rb.AddTorque(Random.onUnitSphere * 100000);
        rb.AddForce((transform.position - posInit).normalized * explosionForceOnSpawn);

        //rb.AddExplosionForce(explosionForceOnSpawn, posInit, 10f);

        yield break;
    }

    void Update()
    {
        if (gameObject != null && isActiveAndVisible && forceNoSpawn != true)
        {
            //// --- Baisse la phospho jusqu'à zero
            //if (phosphoValue >= 0 && isPhophoAffected)
            //{
            //    phosphoValue = phosphoValue - 0.005f - Time.deltaTime;

            //    instancedMaterial.SetFloat("_RevealLightEnabled", phosphoValue);
            //}

            // --- Si le timer depasse la valeur d'inactivité
            if (timerBeforeDisapearIncrement >= timerBeforeDisapear)
            {
                if (currSize > 0 && prop != null && !prop.isAffectedByGravity) // Si il est pas désactivé et qu'il n'est pas affecté par la gravité
                {
                    prop.enabled = false; // On desactive son script prop
                    currSize -= Time.deltaTime;

                    if (currSize < 0) Depop();
                    else transform.localScale = Vector3.one * currSize * scaleMultiplier;
                }

            }
            else
            {
                timerBeforeDisapearIncrement += Time.deltaTime;
                transform.localScale = Vector3.one * scaleMultiplier;
            }
        }
    }
}
