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

    void Start()
    {
        if (forceNoSpawn != true)
        {
            BodyPartsManager.Instance.NotifyApparition(this);

            if (isPhophoAffected)
            {
                meshRenderer = gameObject.GetComponent<Renderer>();
                instancedMaterial = meshRenderer.materials[1];
            }

            timerBeforeDisapear = Random.Range(minTimerBeforeDisapear, maxTimerBeforeDisapear);

            prop = GetComponent<Prop>();

            transform.rotation = Random.rotation;

            if (!isAlwaysSpawning && isEnemyPart)
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
                    StartCoroutine(AddExplosionEffect(rb));
                }
            }
            else if (isAlwaysSpawning && isEnemyPart)
            {
                Rigidbody rb;
                rb = GetComponent<Rigidbody>();
                StartCoroutine(AddExplosionEffect(rb));
            }


        }
    }

    IEnumerator AddExplosionEffect(Rigidbody rb)
    {
        yield return new WaitForEndOfFrame();

        rb.AddExplosionForce(explosionForceOnSpawn * explosionForceOnSpawn, transform.position, 3f);

        yield break;
    }

    void Update()
    {
        if (gameObject != null && isActiveAndVisible && forceNoSpawn != true)
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
