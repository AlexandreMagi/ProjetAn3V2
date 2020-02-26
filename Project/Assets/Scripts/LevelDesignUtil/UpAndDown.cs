using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDown : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Vector3 displacement = new Vector3(0, Mathf.Cos(Time.time * 0.7f) * 0.2f, 0);
        this.transform.Translate(displacement * Time.deltaTime, Space.World);
    }
}
