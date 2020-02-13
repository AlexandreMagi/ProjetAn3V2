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

    void Start()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();
        instancedMaterial = meshRenderer.material;

        timerBeforeDisapear = Random.Range(minTimerBeforeDisapear, maxTimerBeforeDisapear);
    }

    void Update()
    {
        if (gameObject != null)
        {
            if (phosphoValue >= 0)
            {
                phosphoValue = phosphoValue - Time.deltaTime;

                instancedMaterial.SetFloat("_RevealLightEnabled", phosphoValue);
            }

            if (timerBeforeDisapearIncrement >= timerBeforeDisapear)
            {
                if (transform.localScale.x >= 0 && transform.localScale.y >= 0 && transform.localScale.z >= 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y - Time.deltaTime, transform.localScale.z - Time.deltaTime);
                }else
                {
                    transform.localScale = new Vector3(0, 0, 0);
                    gameObject.SetActive(false);
                }

            }else
            {
                timerBeforeDisapearIncrement += Time.deltaTime;
            }
        }
    }
}
