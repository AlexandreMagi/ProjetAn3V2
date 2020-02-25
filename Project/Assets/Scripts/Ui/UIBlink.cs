using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlink : MonoBehaviour
{

    Image img = null;
    [SerializeField] Vector2 randomTimeOff = Vector2.zero;
    [SerializeField] Vector2 randomTimeOn = Vector2.zero;
    float currentTimeRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeRemaining -= Time.unscaledDeltaTime;
        if (currentTimeRemaining < 0)
        {
            if (img.enabled)
                currentTimeRemaining = Random.Range(randomTimeOff.x, randomTimeOff.y);
            else
                currentTimeRemaining = Random.Range(randomTimeOn.x, randomTimeOn.y);

            img.enabled = !img.enabled;
        }
    }
}
