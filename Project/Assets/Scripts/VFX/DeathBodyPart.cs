using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBodyPart : MonoBehaviour
{

    Renderer meshRenderer;
    Material instancedMaterial;

    float phosphoValue = 1;

    [SerializeField]
    float minTimerBeforeDisapear = 2;

    [SerializeField]
    float maxTimerBeforeDisapear = 5;

    float timerBeforeDisapear;
    float timerBeforeDisapearIncrement;

    Prop prop;

    float explosionForceOnSpawn = 10000f;

    [SerializeField]
    bool isAlwaysSpawning = true;

    [SerializeField]
    bool isPhophoAffected = false;

    [SerializeField]
    bool isSwarmerPart = false;

    bool isActiveAndVisible = true;

    void Start()
    {
        if (isPhophoAffected)
        {
            meshRenderer = gameObject.GetComponent<Renderer>();
            instancedMaterial = meshRenderer.materials[1];
        }

        timerBeforeDisapear = Random.Range(minTimerBeforeDisapear, maxTimerBeforeDisapear);

        prop = GetComponent<Prop>();

        if (!isAlwaysSpawning && isSwarmerPart)
        {
            int rand;
            rand = Random.Range(0, 2);

            if (rand == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Rigidbody rb;
                rb = GetComponent<Rigidbody>();
                rb.AddExplosionForce(explosionForceOnSpawn, transform.position, 100f);
            }
        }
        else if (isSwarmerPart)
        {
            Rigidbody rb;
            rb = GetComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForceOnSpawn, transform.position, 100f);
        }
    }

    void Update()
    {
        if (gameObject != null && isActiveAndVisible)
        {
            if (phosphoValue >= 0 && isPhophoAffected)
            {
                phosphoValue = phosphoValue - 0.005f - Time.deltaTime;

                instancedMaterial.SetFloat("_RevealLightEnabled", phosphoValue);
            }

            if (timerBeforeDisapearIncrement >= timerBeforeDisapear)
            {
                if (transform.localScale.x >= 0 && transform.localScale.y >= 0 && transform.localScale.z >= 0 && !prop.isAffectedByGravity)
                {
                    transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y - Time.deltaTime, transform.localScale.z - Time.deltaTime);
                    prop.enabled = false;

                }else if (!prop.isAffectedByGravity)
                {
                    transform.localScale = new Vector3(0, 0, 0);
                    isActiveAndVisible = false;
                    gameObject.SetActive(false);
                }

            }else
            {
                timerBeforeDisapearIncrement += Time.deltaTime;
            }
        }
    }
}
